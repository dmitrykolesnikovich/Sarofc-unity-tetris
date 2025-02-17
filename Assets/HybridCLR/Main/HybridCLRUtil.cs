﻿using System;
using System.Collections.Generic;
using System.Text;
using Saro.Core;
using UnityEngine;

namespace HybridCLR
{
    public static class HybridCLRUtil
    {
        public static bool IsHotFix { get; } =
#if ENABLE_HOTFIX
            true
#else
            false
#endif
        ;

        public static string s_HotFixDLL = "HotFix.dll";

        public static List<string> AOTMetaAssemblies { get; } = new List<string>()
        {
            "mscorlib.dll",
            "System.dll",
            "System.Core.dll", // 如果使用了Linq，需要这个
            "Newtonsoft.Json.dll",
            "UniTask.dll",
            "GameMain.dll",
            "Saro.MGF.dll",
            "Saro.MoonAsset.dll",
            "Saro.Entities.dll",
            "Saro.Entities.Extension.dll",
            //"Saro.MGF.HybirdCLR.dll",
            // add more
        };

        /// <summary>
        /// 为aot assembly加载原始metadata， 这个代码放aot或者热更新都行。
        /// 一旦加载后，如果AOT泛型函数对应native实现不存在，则自动替换为解释模式执行
        public static void LoadMetadataForAOTAssembly(List<string> aotDllList)
        {
#if !UNITY_EDITOR
            LoadMetadataForAOTAssembly_Internal(aotDllList);
#endif
        }

        private static async void LoadMetadataForAOTAssembly_Internal(List<string> aotDllList)
        {
            // 可以加载任意aot assembly的对应的dll。但要求dll必须与unity build过程中生成的裁剪后的dll一致，而不能直接使用原始dll。
            // 我们在BuildProcessor_xxx里添加了处理代码，这些裁剪后的dll在打包时自动被复制到 {项目目录}/HybridCLRData/AssembliesPostIl2CppStrip/{Target} 目录。

            /// 注意，补充元数据是给AOT dll补充元数据，而不是给热更新dll补充元数据。
            /// 热更新dll不缺元数据，不需要补充，如果调用LoadMetadataForAOTAssembly会返回错误

            var assetManager = IAssetManager.Current;

#if ENABLE_LOG
            var sb = new StringBuilder(1024);
            sb.AppendLine("LoadMetadataForAOTAssembly:");
#endif

            using (var vfile = await assetManager.OpenVFileAsync("Assets/ResRaw/hotfix"))
            {
                foreach (var aotDllName in aotDllList)
                {
                    byte[] dllBytes = vfile.ReadFile(aotDllName);
                    if (dllBytes == null)
                    {
                        Debug.LogError($"LoadMetadataForAOTAssembly failed. file not found: {aotDllName}.");
                        continue;
                    }

                    unsafe
                    {
                        fixed (byte* ptr = dllBytes)
                        {
                            // 加载assembly对应的dll，会自动为它hook。一旦aot泛型函数的native函数不存在，用解释器版本代码
                            int err = RuntimeApi.LoadMetadataForAOTAssembly((IntPtr)ptr, dllBytes.Length);
#if ENABLE_LOG
                            sb.AppendLine($"dll : {aotDllName}. ret : {err}");
#endif
                        }
                    }
                }
            }

#if ENABLE_LOG
            Debug.LogError(sb.ToString());
#endif
        }
    }
}
