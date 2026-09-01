# 工具索引 (Tools Index)

> 新工具安装/移除时必须登记本表。

## 本机已安装

| 工具 | 版本 | 路径 | 用途 |
|---|---|---|---|
| .NET SDK | 8.0.424 | `C:\Program Files\dotnet` | 构建 反编译工程/扫描工具 |
| .NET Runtime | 6.0.36 | `C:\Program Files\dotnet` | ilspycmd 运行依赖 |
| ilspycmd | 8.2.0.7535 | `%USERPROFILE%\.dotnet\tools` | DLL 反编译（⚠️ 最新版要求更高 SDK，锁定此版本） |
| UABEA | v8 | `modwork\tools\UABEA\UABEAvalonia.exe` | 资产文件交互式编辑（TextAsset/Texture2D），含 classdata.tpk |
| AssetScan | 1.0（AssetsTools.NET 3.0.5） | `modwork\tools\AssetScan\` | 资产容器扫描器：类型统计/TextAsset 清单/表格预览（7 容器实测 0 错误） |
| PocCsvRow | 1.0（AssetsTools.NET 3.0.5） | `modwork\tools\PocCsvRow\` | SampleF CSV 加行工具：`export` 导出 SampleF/Skill_FY 全文；`addrow <in> <out> <模板IndexName> <新IndexName> <新InfoKey>` 克隆行+覆盖列+写盘自验（poc-arcboomerang 实测 PASS） |
| AssetEdit | 1.0（AssetsTools.NET 3.0.5） | `modwork\tools\AssetEdit\` | 定点数值修改器（试改演练实测 PASS，见 DRILL-REPORT.md） |
| **SkillForge** | v1（AssetsTools.NET 3.0.5） | `modwork\tools\SkillForge\` | **spec 驱动加技能流水线**：`run <spec.json>` 克隆模板行+列覆盖+自验+报告；`verify --assets <f> <spec>` 断言产物。测试 A/B + 4 负向用例全 PASS。架构见 `docs/skill-spec.md` |
| **DescSync** | v1（AssetsTools.NET 3.0.5） | `modwork\tools\DescSync\` | **技能描述同步工具**（AGENTS.md 描述同步红线的执行器）：`set <in.assets> <out.assets> <ops.json>` 对 Skill_FY JSON（resources.assets path_id=433）做定点键值更新/新增，裸读往返校验+JSON 结构断言+重开自验。desc-sync 车道实测 PASS |

### 常用命令

```powershell
# PATH 前置（每个新 shell 需要）
$env:Path = "$env:ProgramFiles\dotnet;$env:USERPROFILE\.dotnet\tools;$env:Path"

# 反编译（已完成的工程无需重跑）
ilspycmd -p -o C:\GAME-AnYingDiLao\modwork\decompiled "<dll路径>"

# 构建反编译工程（部署前必须 0 error）
dotnet build C:\GAME-AnYingDiLao\modwork\decompiled\Assembly-CSharp.csproj -c Release -nologo

# 启动验证（≥35s 存活 + 日志零异常）
Start-Process "C:\GAME-AnYingDiLao\暗影地牢 Demo\Shadow Dungeon.exe"
Get-Content "$env:USERPROFILE\AppData\LocalLow\OO Cat\Shadow Dungeon\Player.log" -Tail 40
```

## 引入中（测试准备车道）

| 工具 | 目标位置 | 状态 |
|---|---|---|
| AssetEdit（AssetsTools.NET 3.0.5） | `modwork\tools\AssetEdit\` | 试改演练车道开发中 |

### AssetsTools.NET 3.0.5 已知坑（fix-12 实测）
- 必须显式 LoadClassPackage(classdata.tpk) + LoadClassDatabaseFromPackage(2019.4.39f1)（这些资产无内嵌 typetree）
- 字符串解析有损（非 UTF-8 字节变 U+FFFD）→ TextAsset 的 m_Script 用序列化布局裸读（len-prefixed + 4 字节对齐）
- AsString/AsByteArray 是属性不是方法；TypeId 是 int

## 外部工具储备（lib-1 推荐，未安装）

| 工具 | 链接 | 场景 |
|---|---|---|
| UABEANext | github.com/nesrak1/UABEANext | UABEA 活跃续作线（nightly） |
| AssetRipper | github.com/AssetRipper/AssetRipper | 只读检查器/素材导出（勿用于回写） |
| BepInEx 5.4.23.x | github.com/BepInEx/BepInEx/releases | 运行时注入路线 A2（win_x64 Mono 版；HarmonyBackend=cecil 备选） |
| FSB-BANK-Extractor-Rebuilder (IZH318) | GitHub | FMOD bank 浏览/等长重建（社区工具，成功率因 bank 而异） |
| FMOD Studio 2.2.x（免费版） | fmod.com | 自建 Mod.bank 路线 B3（对齐游戏 2.2.24） |

## 已知坑登记

- ilspycmd 最新版在 SDK 8 下报 `DotnetToolSettings.xml not found` → 锁 8.2.0.7535 + 装 .NET 6 Runtime。
- csproj 中文路径 HintPath 会乱码 → 一律指向 `modwork/refs` ASCII 路径。
- PowerShell `-LiteralPath` 不展开通配符，批量复制用 `-Path`。
