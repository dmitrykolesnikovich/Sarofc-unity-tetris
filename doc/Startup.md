
# 快速开始

使用了upm git功能，请确保能够顺畅使用github。如果无法自动下载，则需要手动去packages.json配置包。

可热更新hotfix程序集里的所有代码，打包流程大体上没什么大变化，新增了代码打包相关代码。

1. 启动场景 Main.unity
2. MGF Tools/Build 开启打包面板

### 配置Manifest
<img src="https://github.com/Sarofc/tetris-ecs-unity/blob/main/doc/manifest.jpg"><img>

### 配置BuildGroups
<img src="https://github.com/Sarofc/tetris-ecs-unity/blob/main/doc/buildgroup.jpg"><img>

### 打包操作面板
<img src="https://github.com/Sarofc/tetris-ecs-unity/blob/main/doc/build.jpg"><img>

### 宏

- ENABLE_HOTFIX 开启代码热更
- USE_INPUT_HUD 使用hud作为输入，android版必须要，否则无法进行游戏操作
- ENABLE_LOG 开启log

### 注意事项

1. 使用fileserver时，需要注意防火墙，server的url在GameTools的json里，要与manifest的url一样
2. 配置AotConfig.cs，生成link.xml