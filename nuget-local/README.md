# nuget-local

这里存放**没有公开发布**的 ClassIsland SDK 预发行包，供本地构建与 GitHub Actions 日常构建使用
（见仓库根目录 [`nuget.config`](../nuget.config) 里的 `LocalPackages` 源，路径相对于 `nuget.config`）。

当前版本：`2.1.1.1-misha-dev3225`

| 包 | 说明 |
| --- | --- |
| `ClassIsland.PluginSdk` | 插件 SDK，`SuperAutoIsland.csproj` 直接引用 |
| `ClassIsland.Core` | `SuperAutoIsland.Interface.csproj` 直接引用，也是 PluginSdk 的依赖 |
| `ClassIsland.Shared` | 传递依赖 |
| `ClassIsland.Shared.IPC` | 传递依赖 |
| `ClassIsland.Platforms.Abstractions` | 传递依赖 |

这些包来自 ClassIsland `develop/v2/misha-alpha` 分支的构建。公开 feed 上没有这个版本
（nuget.org 最高 `2.1.1.1`，myget 的 ClassIsland feed 最高 `2.1.1.1-misha-dev3203`），
所以必须随仓库自带，否则 CI 无法还原依赖。

> 本目录只放 SDK 包，不要放插件自身的产物。
