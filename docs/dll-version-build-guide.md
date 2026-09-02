# DLL 版本回退修复与构建指南

## 问题根因

原工程在 `Assembly-CSharp.csproj` 中设置了 `GenerateAssemblyInfo=False`，同时
`Properties/AssemblyInfo.cs` 又将 `AssemblyVersion` 固定为 `0.0.0.0`。因此每次重新编译时，
新的 MOD 代码会进入 DLL，但文件版本信息不会跟随 MOD 版本更新，看起来就像“DLL 退回旧版本”。

旧流程还依赖人工从 `bin`、历史升级包或游戏目录复制 DLL，没有强制校验“刚刚构建的 DLL”与
“最终安装的 DLL”是否完全相同，因此也存在被旧产物覆盖的风险。

本修复做了两层处理：

1. SDK 统一从 `Directory.Build.props` 生成版本元数据；默认 MOD 版本为 `1.35.0`。
2. 新增唯一构建入口 `MODworkv2/build-mod.ps1`：每次使用独立临时输出目录，执行 clean 和
   no-incremental 构建，检查生成时间、文件版本和 SHA256，再生成带自校验安装脚本的升级包。

> `AssemblyIdentityVersion` 仍保持 Unity 原始的 `0.0.0.0`，这是兼容性设计，不是版本回退。
> 用户可见的“文件版本”为 `1.35.0.0`，“产品版本”为 `1.35.0`。

## 首次准备

GitHub 仓库按 `.gitignore` 不分发游戏二进制，因此必须使用你本机的完整工作区，并保证：

- 已安装 .NET SDK 8.x；
- `MODworkv2/refs/` 中有游戏 `Shadow Dungeon_Data/Managed/` 的引用 DLL；
- 修改过的 MOD 源码已经保存在 `MODworkv2/decompiled/`；
- 游戏已经关闭。

如果 `refs` 不完整，新脚本会在构建前直接停止，并列出缺失引用，不会继续拿旧 DLL 打包。

## 合并修复文件

把修复压缩包解压到仓库根目录，保持目录结构并允许覆盖同名文件。不会覆盖你的 MOD 功能源码；
涉及反编译工程的文件只有项目版本配置和空的 AssemblyInfo 文件。

建议先提交或备份当前工作区，再执行覆盖。

## 构建与生成升级包

在仓库根目录打开 PowerShell：

```powershell
.\MODworkv2\build-mod.cmd -Version 1.35.0
```

也可以直接运行：

```powershell
powershell -NoProfile -ExecutionPolicy Bypass `
  -File .\MODworkv2\build-mod.ps1 -Version 1.35.0
```

成功后会生成：

```text
MODworkv2\builds\ShadowDungeon-MOD-V1.35.0_YYYY-MM-DD\
MODworkv2\builds\ShadowDungeon-MOD-V1.35.0_YYYY-MM-DD.zip
```

目录内包括：

- `Assembly-CSharp.dll`：本次唯一构建产生的 DLL；
- `install.ps1`：只安装同目录 DLL，并在安装前后校验 SHA256 和文件版本；
- `BUILD-INFO.txt`：版本、哈希、Git 提交和构建时间；
- `SHA256.txt`：DLL 校验值；
- `README.md`：升级包简要说明。

如果同版本包已经存在，脚本会拒绝覆盖。发布新构建时应递增版本号，避免同号 DLL 混用。

## 安装到游戏

解压刚生成的升级包，在该目录打开 PowerShell：

```powershell
powershell -ExecutionPolicy Bypass -File .\install.ps1 `
  -GameRoot "G:\SteamLibrary\steamapps\common\Shadow Dungeon"
```

`GameRoot` 要指向含 `Shadow Dungeon.exe` 和 `Shadow Dungeon_Data` 的游戏根目录。

安装脚本会：

1. 检查游戏没有运行；
2. 检查包内 DLL 的 SHA256 和文件版本；
3. 把当前 DLL 备份到游戏根目录的 `MOD-Backups\时间戳\`；
4. 安装包内 DLL；
5. 再次检查目标 DLL 的 SHA256 和文件版本；
6. 任何一步失败时自动恢复备份。

## 验证 DLL

验证构建包内 DLL：

```powershell
.\MODworkv2\verify-dll.ps1 `
  -Path ".\MODworkv2\builds\ShadowDungeon-MOD-V1.35.0_YYYY-MM-DD\Assembly-CSharp.dll" `
  -ExpectedFileVersion "1.35.0.0"
```

验证游戏实际加载位置的 DLL：

```powershell
.\MODworkv2\verify-dll.ps1 `
  -Path "G:\SteamLibrary\steamapps\common\Shadow Dungeon\Shadow Dungeon_Data\Managed\Assembly-CSharp.dll" `
  -ExpectedFileVersion "1.35.0.0"
```

两边的 `SHA256` 必须相同。只看文件名或修改时间不足以判断是否装对版本。

## 后续升级版本

推荐每次发布都显式传新版本号，例如：

```powershell
.\MODworkv2\build-mod.cmd -Version 1.36.0
```

如果仍要使用普通的 `dotnet build -c Release`，请先修改
`MODworkv2/decompiled/Directory.Build.props` 中唯一的默认值：

```xml
<ModVersion Condition="'$(ModVersion)' == ''">1.36.0</ModVersion>
```

普通构建的输出仍在 `bin\Release\netstandard2.0\`，但发布和安装应继续使用 `build-mod.ps1`，
因为它包含防旧产物、哈希校验、打包和备份恢复流程。

## 容易混淆的两个版本

- Windows 文件属性、`verify-dll.ps1` 显示的 `FileVersion/ProductVersion`：本修复负责更新。
- 游戏标题界面的 `Application.version`：它来自 Unity 的 `globalgamemanagers`，不是 DLL 元数据；
  仅重编译 `Assembly-CSharp.dll` 不会改变该显示。

如果文件版本正确但游戏标题版本未变，属于第二种情况，并不是旧 DLL 被装回去了。
