# Repository Atlas: 《暗影地牢》(Shadow Dungeon) 反编译研究与修改工作区

## Project Responsibility

Unity Mono 游戏《暗影地牢》的**编译成品 + 反编译研究/修改工作区**。游戏本体为 Shadow Dungeon.exe（Unity 2019.4.39f1，Mono 后端，FMOD 音频，Spine 动画，Steamworks）。工作区支持：反编译源码级修改 → 重编译 → 回填部署 → 启动验证的完整 mod 流程。

## System Entry Points

- `Game-Later/Shadow Dungeon.exe` — 新版完整游戏本体启动项（vanilla 基线 `92E0120F…2D52`）
- `Game-Later/Shadow Dungeon_Data/Managed/Assembly-CSharp.dll` — 新版游戏逻辑主程序集（修改目标，2,352,640 字节）
- `Game-Later/` 备份——与 `Game-Later` 同基线，用于部署验证
- `MODworkv2/decompiled/Assembly-CSharp.csproj` — 新版反编译工程构建入口（`dotnet build -c Release`）
- `%USERPROFILE%\AppData\LocalLow\OO Cat\Shadow Dungeon\Player.log` — 运行日志（崩溃验证依据）

## Workspace Top-Level Map

| 路径 | Responsibility |
|---|---|
| `Game-Later/` | **新版完整游戏本体**（exe + Unity 运行时 + 数据资产）。`Shadow Dungeon_Data/Managed/` 含全部运行时程序集（127 个 DLL）；`globalgamemanagers`/`level1`/`resources.assets`/`sharedassets1.assets` 为 Unity 资产主容器 |
| `Game-Later/` 备份 | **副本验证**（与 Game-Later 同基线，用于部署验证与冒烟测试） |
| `MODworkv2/decompiled/` | **新版 Assembly-CSharp.dll 的 ILSpy 8.2 反编译工程**（923 个 .cs，可构建，vanilla 0 error / MOD 0 error，含 V1.31 全部 MOD 代码） |
| `MODworkv2/refs/` | 新版构建引用 DLL（127 个，ASCII 路径副本，与 Game-Later/Managed 1:1 同步，供 csproj HintPath 使用） |
| `MODworkv2/backup/` | 新版原版 Assembly-CSharp.dll 备份（`92E0120F…2D52`，2,352,640 字节，与 Game-Later 一致） |
| `MODworkv2/builds/` | 升级包输出区（`Game-Later-MOD-Vx.x_YYYY-MM-DD.zip`，含 DLL + install.ps1 + README） |
| `_archive/` | **旧版归档区**：`modwork_archived/`（旧 MOD 工作区 V1.0-V1.5），仅回溯用 |
| `.opencode/loop-history/` | loop 自动化任务的历史记录（PASS/FAIL 判定） |
| `.slim/` | codemap 变更检测状态（codemap.json） |
| `AGENTS.md` | agent 工作须知（本 Atlas 入口注册处） |

> **重建说明（2026-09-01，最终版 82AF138C（Scheme A））**：完整版游戏目录 `Game-Later/` 为新 vanilla 基线（`92E0120F…2D52`，2,352,640 字节，127 Managed，`level1` 已更新）；MOD 工作区自该基线重新反编译（ILSpy 8.2 `-p`，`array[^^1]`×2 + `RefSafetyRules(11)`×1 已修复）并回灌 V1.31，并补全所有 Game-Later 新增序列化字段：`Hand.IsNewlyPickedItem`/`ItemScript.IsNewlyPicked`/`ContainerItemData.IsNewlyPicked`/`SK_FlySowrd.countedPrefabType`/`SettingBT.WZ`/`PlayerManager.Pick_*`及 `WeaponClass/WeaponSaveData.Enchanted` 双重，`dotnet build 0 error/121 warnings`，`different serialization layout` 与 `level1 is corrupted` 已修复。需同步 `Game-Later/level1` 至 G: 盘，否则 PlayerManager 6032→6040 差 8 字节仍崩。

## 修改工作流（已验证）

```
改 MODworkv2/decompiled 源码
  → dotnet build MODworkv2/decompiled/Assembly-CSharp.csproj -c Release
  → 产物覆盖到 Game-Later/Shadow Dungeon_Data/Managed/Assembly-CSharp.dll
    （同步到 Game-Later/ 副本；原版备份在 MODworkv2/backup）
  → 启动游戏 + 检查 Player.log 无异常
  （失败时从 MODworkv2/backup 还原）
```

已落地示例：V1.31 词条档位显示（`PoedbMod/AffixTierDisplay.cs` + `WeaponClass.cs` 四处注入）与星环总出口修复（`Gun.cs` 四攻击函数 switch 后总出口挂钩，V1.31 重建版 `2216612A…`；2026-09-01 新基线重建版 `358ACF51…`）。

## Code Tree Directory Map（聚合摘要）

新版反编译工程 `MODworkv2/decompiled/` 的命名空间文件夹按五大域组织。域级概要：

| 域 | 文件夹数 | 职责概要 |
|---|---|---|
| 根目录散落脚本 | ~431 文件 | 施法链路（Gun/ACTbar/ARC·MGC·SQS·DEAD）、SK_Fly* 投射物全家、TalentManager CSV 天赋树、SkillData 七子类、Buff/DOT、武器宝石；MOD 追加 FxSpriteFactory/InventorySortBar/InventorySortMode/SkillTagSystem/PoeItemMod |
| 玩家与技能数据 | 6 | PlayerActionManager 技能门控分流、同伴技能运行时快照、ISkillLevelData 契约 |
| 实体 / AI / 交互 | 13 | 同伴/敌人两套 FSM 状态机（A/B 两型）、CompanionBrain 与 EnemyBrain 决策模型、交互调度中枢、物品克隆图标 |
| UI / 输入 / 表现 | 17 | 14 功能面板（含武器三锻造）、输入中枢 20 类、光标子系统、本地化 24 语言、2D 后处理 |
| 关卡 / 框架 / 数据 | 20 | 三层关卡状态机（Global→Chapter→Level）、场景加载链、传送协议、秘境爬塔、存档模型、泛型容器背包 |
| 第三方库 / 残留 | 6 | SK.Framework 工具框架（9 子模块）、DOTween 封装、编辑器期工具残留、废弃脚本 |
| PoedbMod | 1（22 文件） | MOD 框架：AffixTierDisplay/CraftBenchOps+UI/PoedbSkillInjector/Registry/材映射等 |

> 反编译工程总计 **923 .cs**（vanilla 895 + MOD 28；含 PoedbMod 22 + 根目录 6 MOD 文件）。构建产物与 V31 功能等价，仅 SAN 哈希因新基线重编译不同。

## Key Call Chains（摘自 root-scripts.md）

1. **施法主链**：`PlayerActionManager.TryUseSkillDown → UseSkill → PlayerSP/CP → Gun.CreatSP → SK_FlyA.SetStart`
2. **召唤链**：`CreatCP → CompanionRuntimeData → SK_FSQ_comp.Init`
3. **天赋链**：`AddPoint → AddPointXxx → SetXiBuff`
4. **武器 SPC 链**：`AddWP_SPC → SK/HIT/DIE/HURT 字典 → ACTprefabFS/TakeBoomDie`
5. **DOT 链**：`SetDot → DOT_MG.AddDot → TakeBoomDie`
