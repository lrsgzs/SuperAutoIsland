# SuperAutoIsland

> 超级自动化岛~~（岛？）~~
> 
> 为 ClassIsland 带来可复用的行动组/规则集，以及 Blockly 积木式自动化编程。

![图片](https://livefile.xesimg.com/programme/python_assets/56095bcdbf1492a1602e0bb221aabebe.png) 

## 功能特性

- **可复用的行动组** – 将常用操作（如打开软件、发送按键）打包成组，多处调用，一改全改。
- **可复用的规则集** – 把复杂的触发条件（如“数学课且上午且希沃未运行”）保存下来，随时选用。
- **Blockly 积木编程** – 通过拖拽积木块可视化构建自动化逻辑，无需写代码也能实现复杂流程。
- **变量与状态记忆** – 记录运行次数、上次触发时间等，让自动化拥有“记忆”，实现智能场景。
- **行动序列与分支** – 支持按顺序执行多个动作，并根据上一步结果决定下一步走向。

## 使用方法

1. 在 ClassIsland 插件市场搜索 `SuperAutoIsland` 并安装；
2. 重启 ClassIsland，进入 **设置 → 自动化**，你会看到新增的 **SuperAuto** 选项卡；
3. 点击 **打开 Blockly 编辑器**，像搭积木一样拖拽组合你的自动化逻辑；
4. 保存规则，享受更智能的自动触发。

> 想复用已有的行动组或规则集？直接在自动化编辑界面选择“从库中插入”即可。

## 注意事项

- **本插件仅支持 ClassIsland 2.0**，旧版无法使用；
- 如果插件导致 ClassIsland 崩溃，请前往安装目录下的 `Plugins` 文件夹，删除 `lrs2187.sai` 文件夹（相当于卸载插件），然后重启 ClassIsland 并提交工单（附上崩溃信息）；
- 移动 ClassIsland 程序位置后，若发现自动化失效，重启软件即可（无需重新配置）。

## 致谢

- [ClassIsland.PluginSdk](https://github.com/ClassIsland/ClassIsland/)  
- [Blockly](https://developers.google.com/blockly)（通过接口库集成）

## 许可证

本项目基于 [GPL 3.0](LICENSE) 开源。
