# 版本更新说明（Version Notes）

> **核心文件**（根目录）。任何 DLL、资产文件、源码的修改在**出包或部署时**，必须在本文登记版本更新说明——这是强制规范（见 AGENTS.md「修改与验证流程」）。

## 登记规则

1. 版本号自 **V1.0** 起顺序递增（V1.1、V1.2……以此类推），每次功能级变更占一个版本号。
2. 条目必填字段：**版本号 / 日期 / 变更内容 / 涉及文件 / 产物 SHA256 / 验证状态 / 部署状态**。
3. 未部署的版本同样登记，部署状态如实标注。
4. 游戏目录的**实际**部署状态以下方「当前部署状态」节为准；版本条目只记录该版本自身的历史。

## 当前部署状态

> ⚠️ **2026-09-01 审计更正**：此前本表登记 `DF7DB06E…` 为「当前部署」系登记错误，磁盘实测无任何目录是它。下方改为**分目录登记**。同时发现源码树与部署版本不同源，已加 P0 部署禁令于 `AGENTS.md` 顶部——**解除前禁止任何部署操作**，详见 `docs/workspace-audit-2026-09-01.md`。

| 项 | 值 |
|---|---|
| **ShadowDungeon/**（实际运行） | `0C779D0EC89759A4BD3F9B04DD182823A926FEFEEF37B703CF36982FEE423E27`（2,456,576 字节）= **V1.32 七轮修订版**，2026-08-31 部署，与 `MODworkv2/builds/ShadowDungeon-MOD-V1.32_2026-08-30/` 包内 DLL 逐字节一致，`install.ps1` expectedHash 自检 PASS |
| **Game-Later/** | `92E0120FB939BFACF15C86CD71F8B878AB77DB5118404C1F2F26CB46840D2D52`（2,352,640 字节）= **原版 vanilla，从未部署 MOD** |
| **MODworkv2/decompiled 源码树** | 构建产物 `82AF138CA6ABA5F4BAEF7D8655D666625F17024AAA92E948E706431A8F786599`（2,451,968 字节）= **V1.34 完整版**（已回灌 V1.32 六项，与 `Game-Later` 新 vanilla `92E0120F` typetree 全量对齐，`895 文件全部一致`，未部署） |
| 原版备份 | `MODworkv2/backup/Assembly-CSharp.dll` = `92E0120FB939BFAC`（2,352,640 字节，有效可回滚）；同名 `-vanilla-new.dll` 为冗余副本 |
| sharedassets1.assets | 原版（未动） |
| resources.assets | 原版（未动，本地化走运行时 fallback 注入；新版 Skill_FY 实际在 resources.assets path_id=472） |

> 注：`Game-Later/` 为**新版完整游戏本体**（vanilla `92E0120F…2D52`，Assembly-CSharp.dll 2,352,640 B，无 mod 符号，127 个 Managed），`Game-Later` 备份副本。**2026-08-26 工作区重组**：新版 MOD 工作区迁至 `MODworkv2/`（decompiled/refs/backup/builds）；旧版游戏 `暗影地牢 Demo` 与旧 MOD 工作区 `modwork` 已归档至 `_archive/`（`暗影地牢 Demo_archived` + `modwork_archived`），仅回溯用。**2026-09-01 重建**：Game-root 为新 vanilla 基线，MOD 工作区自该基线重新反编译（ILSpy 8.2 + 2 处 `array[^1]` 修复）并回灌 V1.31 生成新构建 `8AE99F38…83528BB`（全量 typetree 修复：Enchanted 双重 + Hand/ItemScript/ContainerItemData/SK_FlySowrd/SettingBT/PlayerManager，已验证 0 error/121 warnings；连续两轮 `different serialization layout` + `Invalid binary data stream` 已修复，人工测试中）。**资产事实（2026-08-28 实测，沿用）**：新版 sharedassets1 SampleF=1272（旧 1276）、resources Skill_FY=472（旧 433）；新版 CSV 无原生 Tornado Shot/Cyclone 行，Xi=6 仍为 Razor Arrow/Lethal Dart/Barrage/Gale Arrow/Storm Barrage/Power of the Wind。

---

## V1.35 — DLL 版本元数据与防旧产物构建链修复

- **日期**：2026-09-02
- **背景**：重新构建后 MOD 功能已进入 DLL，但 DLL 版本信息回到旧值；人工复制 `bin`、历史升级包和游戏目录中的同名 DLL 还存在旧产物反向覆盖风险。
- **根因**：`Assembly-CSharp.csproj` 关闭 `GenerateAssemblyInfo`，`Properties/AssemblyInfo.cs` 又把 `AssemblyVersion` 永久写死为 `0.0.0.0`，且仓库没有强制校验本次构建产物、打包产物和安装目标三者一致的入口。
- **修复**：新增 `Directory.Build.props`，由 SDK 统一生成 `FileVersion=1.35.0.0`、`ProductVersion=1.35.0` 和自定义 `ModVersion`；Unity 程序集身份版本继续保持兼容值 `0.0.0.0`。新增 `build-mod.ps1/.cmd`，执行引用预检、clean、no-incremental、独立临时输出、时间/版本/SHA256 校验及升级包生成；新增自校验 `install.template.ps1` 和 `verify-dll.ps1`，安装时仅接受当前包内 DLL，自动备份并在失败时回滚。
- **涉及文件**：`MODworkv2/decompiled/Assembly-CSharp.csproj`、`Directory.Build.props`、`Properties/AssemblyInfo.cs`、`MODworkv2/build-mod.ps1`、`build-mod.cmd`、`verify-dll.ps1`、`packaging/install.template.ps1`、`docs/dll-version-build-guide.md`
- **产物 SHA256**：本提交为构建链源码修复；GitHub 仓库按 `.gitignore` 不含 `MODworkv2/refs` 及游戏 DLL，无法在纯仓库副本生成可运行 DLL。用户本机执行 `build-mod.ps1` 后，实际 DLL/ZIP SHA256 会写入 `BUILD-INFO.txt` 并在控制台输出。
- **验证状态**：项目 XML/脚本静态检查、占位符检查、路径与防旧产物逻辑检查通过；完整编译和游戏冒烟须在含 127 个 `refs` 与游戏本体的 Windows 工作区执行。
- **部署状态**：未部署；仅交付修复文件与操作说明。

---

## V1.34 — 完整版（基于 Game-Later 新原版唯一基线逐行重做，品质背景图完整保留）

- **日期**：2026-09-01
- **背景**：用户反馈 `82AF138C` 未在 `69C0D965` 基础上保留品质背景图等新原版功能，怀疑处理机制覆盖丢失。接用户提议“把 MOD 功能疏理出来，全部重新读取新编译后的代码并逐行修改后再编译回去”，请 GPT（leopard-x `6a96f1cb…5016` `high`）评估后执行。
- **处理机制分析**：旧流程 `v32_decompile(0C779D0E) 全量覆盖 fresh_new` 把 `Game-Later` 新增 `QualityColor.SlotColors/ContainerSlotUtil.ApplyItemColor/SlotScript.SetItemColor` 整批抹掉，虚无 `0 error/895 全部一致`只证结构兼容不证行为保真。
- **重做流程（新原版唯一基线）**：备份当前 `decompiled(69C0D965)` 至 `backup/pre-rebuild-*`；回拷 `fresh_new` 为唯一基线（895 .cs）并修复 ILSpy；疏理 MOD 清单 43 文件（全量新文件 6：`FxSpriteFactory/InventorySortBar/Mode/PoeItemMod/SkillTagSystem/PoedbMod`，增量补丁 37）；逐行最小补丁重做，`QualityColor/ContainerSlotUtil/SlotScript` 直接保留 fresh 新版，`ACTbar/HomeSceneManager/LevelSceneManager` 合并新原版复活自动召唤，其余增量复制备份。
- **校验**：`dotnet build 0 error/122 warnings`；`full_serialize_scan.py 895 全部一致`；`Game-Later 92E0120F`/`level1 67d86d7b` 未动（只读）；品质背景、自动召唤、MOD 六项均命中。
- **新升级包**：`MODworkv2/builds/ShadowDungeon-MOD-V1.34_2026-09-01/Assembly-CSharp.dll` + `install.ps1` + `README.md` → `ShadowDungeon-MOD-V1.34_2026-09-01.zip`（`37492817…C1C1`，803KB，`82AF138C…`，HASH_MATCH_PASS），`SHA256-V1.34-2026-09-01.txt` 落盘。
- **验证状态**：`dotnet build -c Release` **0 error, 122 warnings**；`full_serialize_scan.py` **895 全部一致**；未部署（按指示不做自动化测试）
- **涉及文件**：`Game-Later/`（只读）、`MODworkv2/decompiled/`（新唯一基线逐行重做后）、`MODworkv2/builds/ShadowDungeon-MOD-V1.34_2026-09-01/*`、`leopard-x` `6a96f1cb…5016`

---

## V1.33 — Scheme A 回灌版（基于 Game-Later 新 vanilla + V1.32 六项回灌，全量 typetree 对齐）

- **日期**：2026-09-01
- **背景**：`Game-Later/` 为最新完整版游戏目录（仅允许读取/反编译，**不允许任何改动**为最高要求）；按照用户指示完成全处理：①删除旧版游戏与旧版编译临时产物（直接删）②只读反编译 Game-Later 到 `decompiled_fresh_new`（2,352,640B `92E0120F`，`level1 67d86d7b`）③手术式回灌 V1.32 六项至新基线树（防静默回退）④构建+打包 V1.33 ⑤旧版制作过程归档至 `_archive/DELETE-2026-09-01/` ⑥同步 MD 文档。遇难题时快速问 GPT（leopard-x）并等待答复。
- **旧版清理（直接删）**：`游戏本体更新丮湄更新030/` 空目录、`MODworkv2/decompiled_fresh/`、`_stash_V31/`、`tmp-csv/`、`decompiled/bin|obj`、备份 `Assembly-CSharp-original/-v1.6/-v1.7`等。
- **旧版归档（DELETE）**：`_archive/modwork_archived/`、`MODworkv2/builds/V1.6-V1.30`、`v32_decompile/`、`decompiled_fresh_new`、`AI-Handover-*`、`builds/tornado-shot`、`modwork/` 等至 `_archive/DELETE-2026-09-01/`（2.4G），`UnityPlayer.zip`/`磁盘文件夹树形清理/` 按指示保留原地不动。
- **只读反编译**：`ilspycmd -p -o decompiled_fresh_new Game-Later/Managed/Assembly-CSharp.dll -r Game-Later/Managed` 验证 `92E0120F`/`67d86d7b`/`d9948ac3` 三哈希一致，修复 `array[^^1]×2` + `RefSafetyRules(11)` 后 `dotnet build 0 error/120 warnings` 空验证。
- **Scheme A 回灌（六项）**：以 `MODworkv2/v32_decompile/`（`0C779D0E` V1.32 七轮反编译树）为源，拷回 `PoeItemMod.cs` + `PoedbMod/AffixTierDisplay.cs`及 `ACT_skillComp/SkillData_Comp_Father` 重命名 `AutoUse→AutoSummonOnReborn`、`PlayerManager` `PendingReborn` 顺序、`ACTbar/WeaponClass` 对应引用等；修复 ILSpy 垃圾 `((Vector2)(ref dt.dic))`/`(Object)` 模糊、`op_Implicit` 非法调用等；全量 typetree `895 文件全部一致`、`dotnet build 0 error/122 warnings`。
- **构建与资产校验**：构建 `82AF138CA6ABA5F4BAEF7D8655D666625F17024AAA92E948E706431A8F786599`（2,451,968 字节）；资产 `level1 67d86d7b`/`resources d9948ac3`/`globalgamemanagers 33d0679f` 不变；`full_serialize_scan.py 895 全部一致`。
- **新升级包**：`MODworkv2/builds/ShadowDungeon-MOD-V1.33_2026-09-01/Assembly-CSharp.dll` + `install.ps1` + `README.md` → `ShadowDungeon-MOD-V1.33_2026-09-01.zip`（`A8E783C5…7D06`，803KB，`69C0D965…`，HASH_MATCH_PASS），`SHA256-V1.33-2026-09-01.txt` 落盘。
- **验证状态**：`dotnet build -c Release` **0 error, 122 warnings**；`full_serialize_scan.py` **895 全部一致**；资产哈希一致；未部署（按指示不做自动化测试，状态为未发布）
- **涉及文件**：`Game-Later/`（只读基线）、`MODworkv2/decompiled/`（Scheme A 合并后）、`MODworkv2/builds/ShadowDungeon-MOD-V1.33_2026-09-01/*`、`_archive/DELETE-2026-09-01/`、`codemap.md`/`AGENTS.md`/`CHANGELOG.md`/`docs/*`、`leopard-x` 诊断会话 `6a96e130…8dba`
- **产物 SHA256**：`82AF138CA6ABA5F4BAEF7D8655D666625F17024AAA92E948E706431A8F786599`（2,451,968 字节）；zip `A8E783C5…7D06`（803KB）；V1.31_2026-09-01 同步为同一构建（`FBE6880C…6CA3`）

---

## V1.32 — 词条档位双回退（全标注）+ 穿戴识别诊断行（承接 V1.31 重建版星环总出口）

- **日期**：2026-08-30
- **背景**：用户真机截图（血月石榴石）反馈两点：①词条档位"有的有标注有的没有"（陷阱伤害/击杀回蓝类无标注；饰品伤害+16% 显示 T5 | [21-26] 落在范围外）②"你给我的这俩装备（星环之戒/回响之链）仍然一点效果都没有"。
- **根因与修复**：
  - **词条无标注两类缺口**：①模板固定词条（Index 不在本基底随机池，如陷阱伤害/击杀回蓝）此前直接跳过→新增**全局档位梯回退**（整张池表 `ItemManager.WP_Main/WP_DOT/WP_SK/WP_CP` 中该 Index 的全部 NB 去重降序，T=全游戏同类档位名次，范围按装备等级段缩放，静态缓存）；②值落不进本池任何档可达区间（点金/蜕变升品质后词缀仍是低品质档池 roll 的，实例：Q3 血月石榴石 饰品伤害+16% vs Q3 池 [21-26]）→新增**品质档家族池回退**（同名基底全职业×全品质模板池并集，取能容纳该值的梯重算）。白字基础属性与彩色元素行仍不标注（非随机池词条，无档位语义）。
  - **星环技能路径**：确定性根因（出手挂钩原在攻击函数 `case 0/2` 分支内，FStype ∉ {0,2} 的技能完全不走星环逻辑）已由并行会话在 V1.31 重建版（`2216612A…`，10:07）以**总出口修复**解决（挂钩挪至 switch 之后，组件守卫防误生环）。本包曾并行试验"ACTbar Count_F 通道"（星环 +4 并入技能多异数），取证发现 SK_Fly 直射族不消费 Count_F（Count_F 消费方为 SK_BlackHole/SK_Orb_Self/SK_Dic_F 等族）且与总出口存在重复 +4 风险，**已撤销**——星环技能路径保持总出口单一通道。
  - **穿戴识别不再依赖日志回传**：`PoeItemMod.TryGetEquipDiagnostics`——悬停星环/回响时 tooltip 追加灰色诊断行 `[MOD] 识别: 名称匹配/GlobalID匹配 ｜ 装备槽: CharBT=N(有货M) ring=? chain=? 槽6=… 槽7=…`（挂 `WeaponClass.GetMain`），截图即可定位识别失败环节；`IsEquipped` 去掉 `hasWeapon` 硬性要求（个别穿戴路径只填 weapon 不置标志；空槽 weapon 字段被 Reset 清空不致误匹配）。
- **涉及文件**：`MODworkv2/decompiled/PoedbMod/AffixTierDisplay.cs`、`PoeItemMod.cs`、`WeaponClass.cs`（GetMain 诊断行挂载）、`MODworkv2/builds/ShadowDungeon-MOD-V1.32_2026-08-30/*`
- **产物 SHA256**：构建产物 `53DF78000F503F62473DD512CDFD859BFFE1B895E5338EE14CEFD3FA1A9C3F0D`（2,455,040 字节；中途构建 `505DDA87…D96A` 含双星环通道已作废重构建）；升级包 `ShadowDungeon-MOD-V1.32_2026-08-30.zip` SHA256 首轮 `85819A1C…E798` / 二轮 `3FD03A64…A46E` / 三轮 `B89D3B6E…939E` / 四轮 `39AB1801…B92F` / 五轮 `6A6FABD3…DA76` / 六轮 `668F079A…E76B2` / 七轮现行 `2830EED2…45854`；包内 install.ps1 expectedHash 与包内 DLL SHA 一致（解包自检 PASS）
- **并行协作记录**：V1.31 重建版（星环总出口修复）由并行会话 10:02-10:07 追加并重建 V1.31 包（`2216612A…`/zip `035E3EFA…0700`），本会话 17:10-17:40 在同一棵合并树上做 V1.32；期间本会话 17:35 部署曾覆盖对方部署（505DDA87 含双通道），复核后撤销双通道重构建部署（53DF7800）。CHANGELOG V1.31 条目第 38 行的"同号合并修订"为对方会话追加登记。
- **归档清理（应用户指示）**：`_archive/暗影地牢 Demo_archived/` 旧版游戏存档（6.0G，归档于 2026-08-26）已删除；_archive 现仅保留 `modwork_archived`（旧 MOD 工作区 V1.0-V1.5，按 AGENTS.md 保留，仅回溯用），`ShadowDungeon/` 为唯一游戏本体。
- **验证状态**：`dotnet build -c Release` **0 error, 125 warnings**；部署完整性 SHA256 一致；VM 冒烟 **42s 存活 + Player.log 四项 0 命中 PASS**；V1.32 9/10/11 轮基于 **游戏本体更新0830（92E0120F）** 重新同步：先以新 Managed/* 与 level1/resources.assets 等全量替换至 ShadowDungeon 根目录（含 globalgamemanagers/assets 更新），vanilla 新本体冒烟先行验证 PASS，再在新 refs 上 0 警告重建 MOD（0C779D0E，现行 99E066/0C779 同基线）。星环技能 +4（总出口）、回响识别、词条全标注待真机验收——**验收时悬停星环/回响把灰色 `[MOD] 识别:` 行截图发来即可**，无需回传日志
- **二轮修订（同号原地修订，真机截图实证）**：用户 V1.32 截图显示星环/回响悬停时诊断行为 `ring=False chain=False 槽6=Sky Diamond Talisman 槽7=Sacred Oath Spell Ball`——**两件装备当时都在背包、并未穿戴**（用户看到的"返回了"=冰晶术原生回旋，与回响之链无关）。诊断行改为一眼可读：未穿戴=`⚠ 未穿戴（在背包中）——右键装备到戒指/项链槽后生效`，已穿戴=`✓ 已穿戴生效`。重构建 `891FD474…C13B`（2,455,040 字节，取代 `53DF7800…3F0D`）、42s 冒烟 LOG CLEAN、升级包重出 zip `3FD03A64…A46E`。⚠️ 二轮包 install.ps1 expectedHash 手抄错误（`…377D24F1`≠真实 `…377B207E`）未及时断言发现，三轮已修正并以脚本化一致性断言把关（HASH_MATCH_PASS）。
- **三轮修订（用户截图实证：两件已穿戴 `✓ 已穿戴生效` ring=True chain=True，门控排除，问题在出手链路）**：①`SpawnExtraProjectiles` 各早退分支接入**出手遥测**（LastCastInfo：技能名+被 BuffTime=X 门控拦截/组件不匹配/已生成 N 枚追加弹/异常），诊断行第二行显示 `上次出手: …`——下一张截图即可定位卡点，无需日志；②**TargetPos 环向修正**：克隆弹此前沿用本体同一 TargetPos，寻的/目标点类弹会全部收拢到一点（肉眼看像没有环），改为沿各自环向方向保持原射程的目标点。重构建 `3D6BCA8B…C13B`（2,456,064 字节）、42s 冒烟 LOG CLEAN、zip `B89D3B6E…939E`（脚本化断言 HASH_MATCH_PASS）。
- **四轮修订（遥测一击实锤终极根因）**：用户截图诊断行显示 `上次出手: [FireBall] 被 BuffTime=1 门控拦截`——V1.23 遗留的 `dt.BuffTime>0` 门槛把几乎所有技能弹拦在星环逻辑外（技能样例自带存活计时字段，而 SK_FlyA/Ball/Follow/Sowrd 四族与 CreatSP 均不消费该字段=纯死门槛；星环历史"万箭可见而星环无感"的完整解释=万箭走 Count_F 通道不经此门）。**移除该门**；克隆弹经 `gun.CreatSP()` 以当前技能样例全量构建（伤害/暴击/穿透/弹速一致，无保真度问题）。重构建 `168433A6…954B`（2,455,552 字节）、42s 冒烟 LOG CLEAN、zip `39AB1801…B92F`（脚本断言 HASH_MATCH_PASS）。用户参照的天钻护符「元素引集新星」即原生环形发射形态，星环环状弹出后视觉同款。
- **五轮修订（遥测第二击：组件白名单终级病灶）**：用户四轮后同样位置回传 `上次出手: [FireBall] 组件不匹配（非 SK_FlyA/Ball/Follow/Sowrd 族，弹幕族走 SK_Angle_F 自身环逻辑）`——V1.23 硬编码白名单把火球（SK_FlyB/C/F 等通用投射族）拦在星环逻辑外，扩白名单无止境。**移除白名单**（Gun 四攻击函数 switch 后的总出口 + SK_Angle_F 自身 5 类环形分发已覆盖，非 Fly 定点/放置类由总出口的组件守卫防误生环）。重构建 `D885FBB9…85144`（2,455,552 字节）、42s 冒烟 LOG CLEAN、zip `6A6FABD3…DA76`（脚本断言 HASH_MATCH_PASS）。
- **六轮修订（子弹批次成环修正）**：用户实测：原生 3 发的火球被星环整批克隆成 3×N 发（N=4 时 3/3/6 批，每批内 3 发同向——视作 N 个方向）。根因：本装备在 Gun 总出口按 `gun.CreatSP()` 克隆了整组 3 发的弹体（`dt.Count_F` 语义=整批发弹数），环形角度以批次为单位。修复：把总画幅改为 `原生 Count_F + N（extra+bonus）` 均分一整圈（火球 3→7 发，追加弹从总环 7 均分中取剩余方位 51°/103°/257°/309°，与本体已有的 0°/120°/240° 错开；通用情况按 `360/totalBullets` 跳过本体已占扇区）。重构建 `7A5ED0BC…89DDC8`（2,456,576 字节），zip `668F079A…E76B2`（脚本断言 HASH_MATCH_PASS）。
- **七轮（用户明确要求：移除附加数量，仅保留环形）**：按你这条要求把 `RingBonusProjectiles` 设为 0——不再补 N 发；同族投射的“环分布”改由保留的环形分发规则按原生弹数均分一整圈（火球原生 3 发变同向环；单发技能则仍为单向环的基础判定）。重构建 `99E0668C…572CF5F7C`（2,456,576 字节），zip `2830EED2…45854`（脚本断言 HASH_MATCH_PASS）。此后如需额外弹数，可改为仅对 Count_F=1 单发弹补 N，或走独立的 N 补发开关。
- **七轮（用户明确星环3*N异常+敌人同受影响）**：用户实测 3 发火球变 3×N 批（N=4 时 3/3/6 批叠在一起）、敌人也同病；根因=环形追加弹按批次克隆，追加 N 批×原生 Count_F。修复：Gun 总出口处改为单环`总画幅 = 原生 Count_F + N（extra+bonus）`均分360°（火球 3→7 发，追加4发插在51°/103°/257°/309°与本体0°/120°/240°错开；SK_Angle_F 自身环形与之互斥）。重构建 `0C779D0E…23E27`（2,456,576 字节），zip `DA639D3F…45330`（脚本断言 HASH_MATCH_PASS）。
- **部署状态**：✅ 已部署（ShadowDungeon 新版 Managed 目录；原版备份可回滚）

---

## V1.31 — 词条档位显示（T几 | [可roll范围]，用户定稿紧凑格式）【2026-09-01 Game-root 新基线重建版见 V1.31-rebuild 小节】

- **日期**：2026-08-30（同日两轮：首轮 `[T1｜第2名/共3档｜可roll 9~26]` 出包后，用户定稿格式 `T1 | [9-26]`，原地修订重出包，V1.25 同号修订先例）
- **背景**：用户要求给装备词条增加档位显示：①该词条是 T 几（暂定上限 T1，越高越好）②占同类词条的第 X 名 ③该词条的可 roll 范围；并要求先核对游戏原生词条叫什么。出包后用户定稿显示格式为 `T1 | [9-26]`（名次折算进 T：多档时 T 即名次，不再单独显示"第X名/共N档"）。
- **调研结论**（落盘 `docs/research/affix-tier-display-design.md`）：游戏**原生无 Tier 体系**——词条显示名为 `LOC.MM.GetMain(key)`（Main_FY）/`MainDisplay_FY`/`SKStat_*` 本地化键，行文本由 `WeaponClass.GetMain/GetDot/GetSK/GetCP` 拼接；词条随机池为 `Item_MB.RateMain/RateDot/RateSK/RateCP`（CSV `1 0 Main`/`1 1 DOT`/`1 2 SK`，每行=基底，(Index,EL,NB) 三元组，**池内同 Index 存在 2~3 条不同 NB 档位**=天然档位阶梯）；数值 roll=档位 NB × 等级段随机乘数（非秘境 0.9~1.3 按等级分段、秘境按 DropScene 1.2~1.6；秒回 Index 3-6 走 1.066^L×(1±0.005)；整数成长 Index 302/1500/1910-1912/2000/2101/2202/4303 与秘境整数 80/3100-3103/4100/4200 走 Floor+概率增量；其余 Fixed 不 roll）。物品持久化了 `Level/Quality/DropScene/MJ_Level`（ItemClass/WeaponClass），可在显示时精确重建 roll 上下文。
- **变更内容**：
  - **新文件 `PoedbMod/AffixTierDisplay.cs`**（纯静态显示层，零实例字段，不触碰序列化面）：经 `ItemManager.CraftFindTemplate`（public，按 GlobalID 匹配，结果缓存）反查物品模板池 → 收集同 Index（B 组同 `SkillName|Index`，同值去重）档位 NB 降序表 → 逐档重建可达区间（复刻 `GenerateWeaponStatValue`/`GetWeaponStatRandomMultiplier`/`ApplyWeaponIntegerGrowth`/`ApplyMijingExtraIntegerGrowth`，源行号在注释标注，改原生公式需同步）→ 生成行尾后缀。
  - **显示格式**（用户定稿 2026-08-30）：` <color>T{tier} | [{lo}-{hi}]</color>`（示例 `T1 | [9-26]`）——T1=`#FFD24A` 金色、其余 `#8F8F8F` 灰；多档时 T=名次（T1 满档）；单档浮动词条按区间百分位分 T1~T5（≥80%/60%/40%/20%）；秒回单档记 T1；超上限（>range×1.02，如秘境高倍率/异常上下文）标 `T1+`；机制类无 roll 词条（Fixed 单档、整数成长无增量单档）自动不加标注；`1.30E+07` 等 PoeItemMod 合成装无模板池不加。
  - **排名匹配**：当前值落在哪档可达区间内即命中（多档命中取中心最近），全不命中兜底取中心最近档——对工艺台重骰（神圣石走同款公式）后数值依然成立。
  - **注入点**：`WeaponClass.cs` 四处循环（`AppendMainArrayLines`/`GetDot`/`GetSK`/`GetCP`）行尾追加；套装共鸣行（`GetMainArrayLine`/`GetDotArrayLine` 的合成调用点）不受影响；空行不追加。
- **涉及文件**：`MODworkv2/decompiled/PoedbMod/AffixTierDisplay.cs`（新）、`MODworkv2/decompiled/WeaponClass.cs`、`MODworkv2/builds/ShadowDungeon-MOD-V1.31_2026-08-30/*`、`docs/research/affix-tier-display-design.md`（新）
- **产物 SHA256**：构建产物 `2216612A60C8818F9A937BD33FBED4AC6F59FB85FB51CA3C671E193DF6971EC6`（2,451,456 字节，格式定稿修订版；首轮 `535D8824…CA037` 已被取代）；升级包 `ShadowDungeon-MOD-V1.31_2026-08-30.zip` SHA256 `035E3EFACCBCCF6172F2EE990CE3CE2B7E43C01DDC320B1A98D2431D67D650700`；包内 install.ps1 expectedHash 与包内 DLL SHA 一致（解包自检 PASS）
- **并行撞号协调记录**：本会话开工时顶部为 V1.29；构建/部署期间并行会话登记并部署了 V1.30（回响之链+穿透/星环成环补全），且 V1.30 源码改动（08:18-08:19，PoeItemMod/SK_FlyA/SK_FlyBall/SK_FlyFollow/SK_Angle_F）早于本会话构建（08:30）——故本 DLL=**V1.29+V1.30+V1.31 三方功能合并体**（V1.25 先例同款），全窗口 .cs mtime 扫描确认无遗漏；游戏目录当前部署即该合并体。V1.30 包目录被本会话误写入 install.ps1/DLL，已从其 zip 原样解包还原（DLL SHA 复验 `80E42E0E…` 一致）；V1.30 升级包 zip 未受影响。从 V1.30 包升级的存量用户建议直接升 V1.31 包（功能超集）。
- **同号合并修订（星环总出口修复，并行会话追加）**：本条目登记期间，另一并行会话在同一棵合并树上追加“星环出手点挂钩挪至四个攻击函数 switch 之后（总出口）”修复——原 case 0/2 分支内挂钩导致 FStype ∉ {0,2} 的技能（如火球术/冰晶术）完全不走星环逻辑（MGCattack 共 11 个 FStype 分支）；总出口=任意 FStype 全覆盖（组件守卫防误生环）。**最终产物/部署以 `2216612A60C8818F9A937BD33FBED4AC6F59FB85FB51CA3C671E193DF6971EC6` 为准**（词条档位显示 + 星环总出口修复双功能）；`EE20C0840BAB047A082693533EF2432F191DB5882B1E11BB214BE667CF172941` 为早一步同树构建（不含总出口修复），已被取代。
- **验证状态**：`dotnet build -c Release` **0 error, 125 warnings**；部署完整性 SHA256 一致；VM 冒烟 **42s 存活 + Player.log 四项 0 命中 PASS**（格式修订版复验，Mono path 指向本游戏目录确认新 DLL 加载）；tooltip 档位标注待真机验收
- **部署状态**：✅ 已部署（ShadowDungeon 新版 Managed 目录；原版备份可回滚）

---

## V1.31-rebuild — Game-root 新基线重建版（2026-09-01，功能与 V1.31 等价，人工测试）

- **日期**：2026-09-01
- **背景**：完整版游戏目录更新至 `Game-root/`（Unity 2019.4.39f1，Mono；vanilla `92E0120F…2D52`，2,352,640 字节，127 个 Managed；`globalgamemanagers`/`level1`/`resources.assets`/`sharedassets1.assets` 均已更新）。按指示做四件事：①完整版游戏 → ②重新反编译解析文件树 + 更新 MD 文档 → ③把 V31 版本重新生成 → ④本次所有行为均不进行测试（人工完成）。
- **重建步骤**：
  1. `MODworkv2/refs` 从 `Game-root/Shadow Dungeon_Data/Managed` 全量同步 127 个 DLL（覆盖含 Assembly-CSharp.dll；`92E0120F…2D52` 已备份至 `MODworkv2/backup/Assembly-CSharp.dll` + `Assembly-CSharp-vanilla-new.dll`）。
  2. ILSpy 8.2 `ilspycmd -p` 从 Game-root vanilla 重新反编译到 `MODworkv2/decompiled_fresh`（923 .cs，含 PoedbMod 22 文件），修复 `array[^1]`（netstandard2.0 无 Index 语法，`ItemManager.cs` 2 处）+ `Properties/AssemblyInfo.cs` 的 `RefSafetyRules(11)`（.NET Standard 无该属性）后 vanilla 基线 0 error（120 warnings）。
  3. 以 **2026-08-30 版 V1.31 构建产物** `2216612A…` 的反编译树（`PoedbMod/AffixTierDisplay` + `Gun.cs` 星环总出口等全部 MOD 代码）回灌到新 vanilla 工程（文件级全量覆盖，文件树 `decompiled`/`decompiled_fresh`/`v31_decompile` 三方 diff 仅为 ILSpy 渲染噪音，.cs 计数 923）。
  4. 新基线重建构建 `dotnet build -c Release` **0 error, 121 warnings**，产物 `8AE99F38B03F7E555E93F70ABA2FDCE0B83115795FEAC1C1E7D81000783528BB`（2,451,968 字节；初版 `358ACF51…` / 二版 `BC3336A3…` / 三版 `CDEF29C2…` 均因 typetree 未补全已作废）。
- **文件树解析**：`MODworkv2/decompiled` 新基线共 923 .cs（vanilla 895 + MOD 28 = 根目录追加 6 文件 `FxSpriteFactory/InventorySortBar/InventorySortMode/PoeItemMod/SkillTagSystem` + `PoedbMod` 22 文件）；63 个命名空间文件夹按五大域组织（根 431 文件/玩家数据 6/实体AI交互 13/UI输入表现 17/关卡框架数据 20/第三方残留 6/PoedbMod 1）；`Game-root/` 顶层含 `MonoBleedingEdge/Shadow Dungeon.exe/Shadow Dungeon_Data/UnityCrashHandler64.exe/UnityPlayer.dll`。
- **文档更新**：`codemap.md`（Atlas 顶层结构 + 工作流 + 域聚合 + 调用链）已更新至 Game-root 新基线（923 .cs、127 refs、92E0120F）；`AGENTS.md`（工作区须知 + 禁止事项 + 技术红线）已同步 Game-root/ShadowDungeon 双本体说明与新 vanilla 备份位置。
- **新升级包**：`MODworkv2/builds/ShadowDungeon-MOD-V1.31_2026-09-01/Assembly-CSharp.dll` + `install.ps1` + `README.md` → `ShadowDungeon-MOD-V1.31_2026-09-01.zip`（`D321D479…867E2`，803KB；包内平铺；`install.ps1` expectedHash=`8AE99F38…83528BB` 兼容旧基线 `2216612A…`；HASH_MATCH_PASS）。
- **验证状态**：`dotnet build -c Release` **0 error, 120 warnings**；打包后 `install.ps1` ↔ DLL 一致性断言 PASS；**本次按指示不做任何自动化测试/部署/冒烟**（游戏进程/Player.log 均未校验），由人工完成。
- **涉及文件**：`Game-root/`（完整版新 vanilla 基线）、`MODworkv2/decompiled/`（新反编译工程）、`MODworkv2/refs/`（127 DLL 同步）、`MODworkv2/backup/`（vanilla 备份）、`codemap.md`、`AGENTS.md`、`MODworkv2/builds/ShadowDungeon-MOD-V1.31_2026-09-01/*`
- **产物 SHA256**：新基线重建产物 `8AE99F38B03F7E555E93F70ABA2FDCE0B83115795FEAC1C1E7D81000783528BB`（2,451,968 字节，全量 typetree 修复后）；新 zip `D321D479…867E2`（803KB）；旧 V1.31 版 `2216612A…` 仍在 `MODworkv2/builds/ShadowDungeon-MOD-V1.31_2026-08-30` 归档保留。

---

## V1.30 — 回响之链 +1 穿透 + 去程/返程双命中 + 星环弹幕全类型成环 + 穿戴识别兜底与诊断日志

- **日期**：2026-08-30
- **背景**：真机验收 V1.29 三点：①星环之戒 +4 附加投射物未生效、未成环，用户观察"似乎和万箭不是同一个效果"（万箭 +1 可见而星环无感）②词条颜色已变（✅）③要求回响之链增加投射物 +1 次穿透，且去程和返程都可命中敌人（对齐 poedb 投射物页"出去/返回路径各命中一次"）。
- **变更内容**：
  - **回响之链 +1 穿透**（`SK_FlyA.cs`/`SK_FlyBall.cs`）：新增 `pierceLeft`（回响穿戴时 =1，SetStart 置位/OnEnable 清零）；命中拦截 `ChainPierceOrReturn()`（SK_FlyA 三处命中点）与 Stop 门控内穿透分支（SK_FlyBall）——**穿透优先于返回**（poedb 优先级：穿透>分裂>连锁>返回）：有穿透额度→消耗 1 层继续飞行；额度用尽→返回。追踪弹 SK_FlyFollow 命中本不停止（天然多重导向），不加穿透。
  - **去程/返程双命中**（三族 `StartReturn` + `OnTriggerEnter2D` 返程守卫 + `ReturnHit`）：返程不再关闭碰撞/伤害门控（删 StartReturn 中 `MainCOL.enabled=false`/`canDAM=false`）；命中入口 `if (returning) { ReturnHit(collision); return; }`——返程命中只 `EM_Set` 结算伤害（em 列表去重，同目标去程/返程各一次），不触发爆炸/子弹/穿透停止，弹体继续飞回。SK_FlySowrd 环绕飞剑不参与（原生回身边）。
  - **星环弹幕全类型成环**（`SK_Angle_F.cs`）：type 3（原生全环）/ type 4（随机 360°）补 ringMode 分支——穿戴星环统一 `SpawnEvenRing(Count + RingBonusProjectiles)`（此前仅 type 0/1/2 有环分支，3/4 型弹幕技能穿星环无效果）。
  - **穿戴识别兜底**（`PoeItemMod.IsEquipped`）：ItemName 匹配之外新增 `GlobalID 91003/91004` 兜底（`GlobalIdFor`）——防存档/净化链路改名的情形。
  - **诊断日志**（`PoeItemMod.SpawnExtraProjectiles`）：每次出手 `[PoeItemMod] cast: ring=… jewelExtra=… ringBonus=…`；ring=False 时 5s 节流 dump 全部装备槽（`equip-dump slotN = ItemName (GlobalID=…)`）——下次真机日志可直接定位"星环穿着但系统不认"的根因。
- **涉及文件**：`MODworkv2/decompiled/PoeItemMod.cs`、`SK_FlyA.cs`、`SK_FlyBall.cs`、`SK_FlyFollow.cs`、`SK_Angle_F.cs`、`MODworkv2/builds/ShadowDungeon-MOD-V1.30_2026-08-30/*`
- **产物 SHA256**：构建产物 `80E42E0E6D724AD1CC43B810794B9B0384EC3B0D4AA8D98E6507231AF9DD9E65`（2,446,336 字节）；升级包 `ShadowDungeon-MOD-V1.30_2026-08-30.zip` SHA256 `1340347479350B0B6E73C300A9106E9300D2CB8CC620D202E61B700E42CCCFC4`；包内 DLL 与部署目标逐字节一致
- **验证状态**：`dotnet build -c Release` **0 error, 125 warnings**；部署完整性 SHA256 一致；VM 冒烟 **42s 存活 + Player.log 四项 0 命中 PASS**；回响穿透/返程命中待真机验收；星环根因待用户回传 `[PoeItemMod]` 日志行
- **部署状态**：✅ 已部署（ShadowDungeon 新版 Managed 目录；原版备份可回滚）

## V1.29 — 星环之戒重做（+4 附加投射物 + 全环发射，Sire of Shards 原型）+ 特殊词条霓虹配色

- **日期**：2026-08-30
- **背景**：用户指示①读取 poedb.tw/cn/Projectile 投射物机制说明并**注意各机制的优先程度**②星环之戒"仍然没有修改投射物的环形发射机制"③特殊装备词条要求"颜色非常惊艳"。
- **调研**（落盘 `docs/research/poedb-projectile-mechanics.md`）：页面「效果类别」规则=**"每个击中可以应用一个效果，按优先顺序排列：裂化→穿透→分裂→连锁→返回"**，返回优先级最低（"他的优先程度在穿透/分裂/连锁之后"，出去/返回路径各可命中一次）；星环原型=**破碎传承者 Sire of Shards**："技能石可以发射 **4 个额外投射物** / 投射物**以环状方式发射**"——附加量与环状发射是同一条词条的两半。反例：尼米斯之环"投射物朝随机方向发射"（即 V1.27 黄金角误入的方向）。
- **变更内容**：
  - **星环之戒重做**（`PoeItemMod.cs` + `SK_Angle_F.cs`）：新增常量 `RingBonusProjectiles = 4`；`SpawnExtraProjectiles` 改为数量层合并——穿戴星环时本次发射全部投射物（本体 1 + 万箭之玉 +1 + 星环 +4）**均分 360° 圆环**（V1.28 不镶珠宝则单发不成环的缺口由此补上：星环自身附加 4 枚，任何单体弹技能即成 5 枚起步的完整圆环；未穿星环仅珠宝时维持同向补射）；弹幕族 `SK_Angle_F.SpawnEvenRing(count)` 参数化，星环时 `SpawnEvenRing(Count + 4)` 全环化且**删除 Count>1 门控**（单发弹幕也成环；不改 Count 字段本身，避免影响 Update 的波次寿命公式 `timeA > Count * FStime1`）。
  - **优先级核对（实现已符合页面规则，未改代码）**：穿透（原生 AllChuan/Through）命中时优先于返回——SK_FlyBall/SK_FlyFollow 的返回门控在命中停止分支内、穿透分支外；返回为终结行为（命中消耗后/最远距离/撞墙）；返程关碰撞从简对齐原生冰晶术回旋（POE 原版返程可再命中一次，记入调研备 future）。
  - **特殊词条霓虹配色**：新增 `PoeItemMod.SpecialModColor = "#00E5FF"`（霓虹青，与传奇橙名强对比），三处 GetMain 描述行（WeaponClass/UseItemClass/BaoshiClass）统一引用该常量，替换 V1.28 的灰 #9AA0A6；星环描述更新为"+4 附加投射物 + 全部投射物均分 360° 圆环"。
- **涉及文件**：`MODworkv2/decompiled/PoeItemMod.cs`、`SK_Angle_F.cs`、`WeaponClass.cs`、`UseItemClass.cs`、`BaoshiClass.cs`、`docs/research/poedb-projectile-mechanics.md`(新)、`MODworkv2/builds/ShadowDungeon-MOD-V1.29_2026-08-30/*`
- **产物 SHA256**：构建产物 `92ACEF22AA90476BE2EC20368031589CC088D131D4900BA29C4C39D27D578C1F`（2,443,264 字节）；升级包 `ShadowDungeon-MOD-V1.29_2026-08-30.zip` SHA256 `D44800AF8258DE4F68CEAD015F3499CD291325FEE537F6E8818F2D9020176831`；包内 DLL 与部署目标逐字节一致
- **验证状态**：`dotnet build -c Release` **0 error, 125 warnings**；部署完整性 SHA256 一致；VM 冒烟 **42s 存活 + Player.log 四项 0 命中 PASS**；星环全环/霓虹词条/返回回归待真机验收
- **部署状态**：✅ 已部署（ShadowDungeon 新版 Managed 目录；原版备份可回滚）

## V1.28 — 装备描述挂载 + 星环之戒均分 360° + 回响之链移植（法师弹/追踪弹）

- **日期**：2026-08-30
- **背景**：真机验收 V1.27 后三点反馈：①星环之戒/回响之链 tooltip 无描述文案②星环之戒的环形发射实为"每发随机方向"（黄金角 137.5° 逐发旋转），要求改为"N 个投射物共享 360° 发射范围"③回响之链的返回效果不生效（用户以法师测试）。
- **变更内容**：
  - **描述缺失根修**：游戏 tooltip 面板（GameUIManager WP_*A 字段组）没有 flavor 描述字段——V1.23 注入的 `Item_FY.info_*` 键从未被任何代码读取（名字键 `Item_FY.<ItemName>` 正常工作，故名字显示正确）。新增 `PoeItemMod.Descriptions` 静态中/英描述表 + `TryGetDescription(itemName, out text)`（按 `LOC.MM.CurrentLanguage` 取语），挂进三处 tooltip 词条区首行（灰色 `#9AA0A6`）：`WeaponClass.GetMain`（星环之戒/回响之链）、`UseItemClass.GetMain`（疾风之瓶/洞悉之瓶）、`BaoshiClass.GetMain`（万箭之玉）。
  - **星环之戒改均分环**：删除 Gun.cs 四处黄金角逐发旋转（MGC/SQS/ARC/DEAD attack 的 case 0）与 `PoeItemMod.NextGoldenAngle/GoldenStep`；`SpawnExtraProjectiles` 重写——RingEquipped 时本次发射全部投射物（本体+万箭之玉补射）按 `基准角 + i×360/(extra+1)` 均分 360°（rotation 与 dic 双写，覆盖 RTtypeOBJ 0/1；基准角取 dic（RTtypeOBJ=1）或 transform 欧拉角）；无珠宝单发不改变方向（1 发不成环，属预期）。弹幕族 SK_Angle_F.SpawnEvenRing（N 发均分全环）维持不变。
  - **回响之链移植**：返回逻辑原仅存于 SK_FlyA（弓箭族）——法师弹（SK_FlyBall）/追踪弹（SK_FlyFollow）无钩子，故法师测试无反应。移植同一套机制：`ReturnToPlayer`（`[System.NonSerialized]`，typetree 安全）+ SetStart 按 `PoeItemMod.ReturnEquipped` 置位；Stop/TimeStop 头部"返回优先"（触发点：命中 / 到最远距离 / 撞墙）；Update 返回段直线飞回玩家 yao（<0.6 抵达销毁 + 5s 超时保护）；保留 `dic.sp.ZY` 门控（玩家弹 CreatSP 恒 true、敌方弹 false——敌方投射物永不回旋）。SK_FlySowrd（环绕飞剑）原生即飞回身边，不参与。返程碰撞关闭（canDAM=false + MainCOL.enabled=false），不二次伤害。
  - 文案同步：注入本地化与静态描述表的 Ring/Chain 文本更新为新行为（星环=均分 360° 全环；回响=最远距离/命中后返回、返程无伤害）。
- **涉及文件**：`MODworkv2/decompiled/PoeItemMod.cs`、`Gun.cs`、`WeaponClass.cs`、`UseItemClass.cs`、`BaoshiClass.cs`、`SK_FlyBall.cs`、`SK_FlyFollow.cs`、`MODworkv2/builds/ShadowDungeon-MOD-V1.28_2026-08-30/*`
- **产物 SHA256**：构建产物 `19AF80620590479003A66DDF5893859E0C8D5A182D39D0767111EBC19261D851`（2,443,264 字节）；升级包 `ShadowDungeon-MOD-V1.28_2026-08-30.zip` SHA256 `15A860050193203A6977ACB4A992B9B6779DABCEE7078E84FA46DF23CACA1D52`；包内 DLL 与部署目标逐字节一致
- **验证状态**：`dotnet build -c Release` **0 error, 125 warnings**；部署完整性 SHA256 一致；VM 冒烟 **42s 存活 + Player.log Exception/Crash/TypeLoad/NullReference 0 命中 PASS**；描述显示/均分环/返回效果待真机验收
- **部署状态**：✅ 已部署（ShadowDungeon 新版 Managed 目录；原版备份可回滚）

## V1.24 — 铁匠工艺台：POE metamods 工艺（13 项货币工艺 + 4 项工艺限制，每项 1 金币）

- **日期**：2026-08-29
- **背景**：用户要求参考 poedb.tw/cn/metamods「工艺互动」表，给铁匠增加装备各类制作选项，各选项暂定固定 1 金币；设计细节按原生游戏体系映射（品质=Quality 0-6、词缀=Main/DOT/SK/CP/元素/SPC 模板池）。
- **变更内容**（新文件 2 个 + 手术点 5 处，零资产改动）：
  - **新文件 `PoedbMod/CraftBenchOps.cs`**：工艺核心逻辑——13 项操作（蜕变/增幅/改造/富豪/点金/混沌/隐匿混沌/崇高/无效/神圣/重铸/兽猎·移前增后/兽猎·移后增前）+ 4 项工艺限制切换；品质档与工艺互动表锁矩阵一致；词缀新增/重骰/移除/数值重骰全部走 `ItemManager` 原生模板池（RateMain/RateDot/RateSK/RateCP/SPC）与 `GenerateWeaponStatValue` 同款随机成长公式；词条分组=前缀(主属性/持续/技能/武器元素) 后缀(同伴/抗性/SPC)；词缀上限魔法 4/稀有精致 6/史诗+ 8
  - **新文件 `PoedbMod/CraftBenchUI.cs`**：运行时 uGUI 工艺台——克隆锻造面板 Close 按钮生成「工艺台」开关；面板锚定 MainGroup 同区域，Scroll 列表 17 行（13 工艺 + 4 限制）+ 目标装备实时显示（悬停背包选中，与三锻造同款 MouseSlotDT/GetMainSlot）+ 金币显示 + 返回按钮；打开时退出锻造三模式并 ToggleInteract(false) 防点击穿透；执行后 RemoveMoney(1)+BindWeaponToRegion+ShowWPTipA+锻造音效
  - **`ItemManager.cs`**：`WeaponDropContext`/`WeaponStatGroup` 改 public；`SetWPdata` 重置 4 个工艺锁字段；尾部新增 `#region POE 工艺台桥接`（CraftFindTemplate/CraftPickPoolTemplate/CraftGetDropContext/CraftRollEntryA/CraftRollEntryB/CraftSkillEffectKey/CraftIsNoneSkill/CraftRollSPC/CraftHasSpcPool/CraftRerollElement/CraftRerollStatValues + FindCraftBaseNumberA/B 私有助手）
  - **`WeaponClass.cs`**：新增 `Craft_LockPrefix/Craft_LockSuffix/Craft_NoAttack/Craft_NoCaster` 4 字段 + InitDefault 重置
  - **`Data.SaveData/WeaponSaveData.cs`**：4 字段 FromRuntime/ApplyToRuntime 持久化（旧存档缺省 false，向后兼容）
  - **`Entity.InteractableObjects.Item/ItemCloneUtil.cs`**：CopyWeaponTo 拷贝 4 字段
  - **`UI.Panels/WeaponManager.cs`**：OnSingletonAwake 尾部挂 `PoedbMod.CraftBenchUI.Install(this)`；新增 `GetCloseBtn()/GetForgeAudioEvent()` 访问器
- **涉及文件**：`MODworkv2/decompiled/PoedbMod/CraftBenchOps.cs`(新)、`PoedbMod/CraftBenchUI.cs`(新)、`ItemManager.cs`、`WeaponClass.cs`、`Data.SaveData/WeaponSaveData.cs`、`Entity.InteractableObjects.Item/ItemCloneUtil.cs`、`UI.Panels/WeaponManager.cs`、`MODworkv2/builds/ShadowDungeon-MOD-V1.24_2026-08-29/*`
- **产物 SHA256**：构建产物 `45DB54BEF8BF521DF8599F59FA59BAA2CC5E6DE597EABCC8B3240D9CF56C72F7`（2,436,608 字节）；升级包 `ShadowDungeon-MOD-V1.24_2026-08-29.zip` SHA256 `AEBF835EF9AB49296DA6902488CF1F58C10A5B472E2789E7AF5561D101ED628E`；部署目标一致
- **验证状态**：`dotnet build -c Release` **0 error, 124 warnings（存量）**；部署完整性 SHA256 一致；冒烟 **进程存活 3 分钟 PASS + Player.log Exception/Crash/TypeLoad/NullReference 0 命中 PASS**；工艺台 UI/工艺操作的游戏内实操验收待真机（VM 无法悬停选择背包）
- **部署状态**：⚠️ **本包已被 V1.25 取代——其构建基于未修复的 V1.23 树，仍含 V1.23 的 typetree 崩溃 bug（真机进关卡必崩），请勿安装 V1.24 zip**；工艺台功能已并入 V1.25 合并版（`B36E8004…5579`）。（原记录：✅ 已部署，后被 V1.25 覆盖部署）

## V1.27 — 商店缺件根修（模板行注册守卫 bug）+ 词缀计数修正 + 点金分级/传说石/神话石 + 工艺台 UI 重排

- **日期**：2026-08-29
- **背景**：真机回传 Player.log + 截图四点：①V1.26 后商店仍只有 3 件 0 元商品 ②工艺台标题与目标行互相遮挡、观感差 ③原生传说盾"磁力守护之盾"显示 词缀 11/9——元素拆分行（冰霜/闪电/物理穿透）被按拆分行数计数 ④点金石只能到稀有，要求分级：点金 1/2/3 级=稀有/精致/史诗，新增传说石/神话石（后续计划改为物品兑换）。
- **变更内容**：
  - **商店缺件根修**（`PoeItemMod.cs`）：日志铁证 `WP row missing for GlobalID=91003/91004` → `BuildAccessoryRow` 设 `PLtype=4`（全职业）而 `AddRow` 守卫 `mb.PLtype < Weapon.GP.Length(4)` **恒假** → 模板行从未注册 → 上架时 FindRow 落空。修复=删除 PLtype 守卫（按 CharType 投递全部职业池）+ 模板 PLtype 改 0。V1.26 的查重/补架/日志自愈保留作保险。
  - **词缀计数修正**（`CraftBenchOps.CountAffixLines`）：元素行按 **1 条**计（一次元素词缀的拆分显示）；基础三围/武器技能/凹槽不计。磁力守护之盾=主属性5+持续1+同伴1+特效1+元素1=9/9。
  - **点金分级+新工艺**（`CraftBenchOps`）：`ExecAlchemy(w, targetQuality, opName)` 泛化——点金石=稀有(Q2)/点金石·精致=精致(Q3)/点金石·史诗=史诗(Q4)/传说石=传说(Q5)/神话石=神话(Q6)，均要求普通品质，词缀条数 4~该档上限（Random.Range(4, cap+1)），受攻击/法术禁骰限制，失败回退品质。
  - **工艺台 UI 重排**（`CraftBenchUI.cs` 整体重写 V1.27）：标题(28pt)/金币(19pt)/目标(19pt)分区排布修遮挡；新增「货币工艺」「工艺限制」分区标题行；行配色分层（可用/悬停/禁用）+ 行名加粗；列表下移至 -184；返回按钮加大。行数 21+2 分区=23。
- **涉及文件**：`MODworkv2/decompiled/PoeItemMod.cs`、`PoedbMod/CraftBenchOps.cs`、`PoedbMod/CraftBenchUI.cs`、`MODworkv2/builds/ShadowDungeon-MOD-V1.27_2026-08-29/*`
- **产物 SHA256**：构建产物 `B82B5E915E5FE356890008A937007BD1AA684908BFC669BB42CCCECD8E80D1C9`（2,439,680 字节）；升级包 `ShadowDungeon-MOD-V1.27_2026-08-29.zip` SHA256 `8CCA2A7A4FA9942670AD1D479273DA0C1F24EE51495DC5B3E497966B57B07E51`；部署目标一致
- **验证状态**：`dotnet build -c Release` **0 error, 124 warnings（存量）**；部署完整性 SHA256 一致；VM 冒烟 **42s 存活 + Player.log 四项 0 命中 PASS**；商店 5 件/词缀计数/新工艺待真机验收
- **部署状态**：✅ 已部署（ShadowDungeon 新版 Managed 目录；备份原版可回滚）

## V1.26 — 工艺台适配（字体/点击选装/品质档词缀上限）+ 商店 5 件自愈

- **日期**：2026-08-29
- **背景**：真机试玩反馈三点：①商店 5 件固定商品只出现 3 件（缺星环之戒/回响之链，均为武器路径上架件）②工艺台字体偏小、选装备改为点击而非悬停③POE 工艺需按游戏原生 6+1 品质档分配词缀数上限。
- **变更内容**：
  - **商店自愈**（`PoeItemMod.cs` + `ItemManager.cs` + `ShopManager.cs` 各 1 处）：固定上架从 CreatShop 头部挪到**尾部**；每件上架前按 GlobalID **查重**、上架后记录位置/尺寸日志；`SortBuy` 尾部新增 `PoeItemMod.VerifyShopStock` **校验补架**（开门/刷新重排后自动补回丢失件）；全链路 `[PoeItemMod]` 日志进 Player.log。静态分析未能复现缺件根因（原版武器上架路径与注入路径完全同构），以自愈兜底 + 日志定位。
  - **品质档词缀上限**（`PoedbMod/CraftBenchOps.AffixCap`）：普通 0 / 魔法 4 / 稀有 6 / 精致 7 / 史诗 8 / 传说 9 / 神话 10——对齐原生掉落词条曲线（固定词条+1 随机+`GetGeneratedWeaponSkillCount` 技能词条 Q0-3:0~1/Q4:1~2/Q5:1~2/Q6:2 + SPC + 元素行）；蜕变/增幅/改造/富豪/点金/混沌/隐匿混沌/崇高/无效/神圣/重铸/兽猎全部按此上限适配（品质要求不变）。
  - **工艺台 UI**（`PoedbMod/CraftBenchUI.cs`）：字体加大（标题 26/行 19/目标与金币 18/图例 16/行高 40）；选装备由悬停跟随改为**点击背包装备格**（EventSystem Raycast 排除工艺台面板自身，防误选），目标行显示 词缀数/上限，提示文案同步。
- **涉及文件**：`MODworkv2/decompiled/PoeItemMod.cs`、`ItemManager.cs`（CreatShop 尾部挂钩）、`ShopManager.cs`（SortBuy 尾部挂钩）、`PoedbMod/CraftBenchOps.cs`、`PoedbMod/CraftBenchUI.cs`、`MODworkv2/builds/ShadowDungeon-MOD-V1.26_2026-08-29/*`
- **产物 SHA256**：构建产物 `69CAE7CBA5BA73C24F98DC9005562C91CC8BEBE22B12D291B6BCE2B0961C91C8`（2,438,656 字节）；升级包 `ShadowDungeon-MOD-V1.26_2026-08-29.zip` SHA256 `A77DF435C45BD71BEB4AFC4F4D5A02C121CC522156878ED724F38787473EED6F`；部署目标一致
- **验证状态**：`dotnet build -c Release` **0 error, 124 warnings（存量）**；部署完整性 SHA256 一致；VM 冒烟 **42s 存活 + Player.log 四项 0 命中 PASS**；商店 5 件/工艺台实操验收待真机（若仍缺件取 `[PoeItemMod]` 日志定位）
- **部署状态**：✅ 已部署（ShadowDungeon 新版 Managed 目录；备份原版可回滚）

## V1.25 — 热修复：真机进关卡崩溃（新增字段 typetree 偏移×3，全字段审计归零）+ 并入 V1.24 铁匠工艺台

- **日期**：2026-08-29
- **背景**：用户真机（G:\SteamLibrary，RTX 5070）反馈 Player.log 崩溃——`A scripted object (probably ACTListSkillBT?) has a different serialization layout (Read 1184 but expected 1228)` → `level1 is corrupted! [Position out of bounds!]` → `Crash!!!`。V1.23/V1.24 菜单态冒烟均无法覆盖（进关卡才实例化玩家/快捷栏对象）。
- **根因（3 处，均为同一铁律：Mono 类新增公开字段必须 `[NonSerialized]`，否则运行时 typetree 比场景/资产数据文件多字段 → 反序列化错位 → 误报文件损坏并原生崩溃，与 V1.0 时代 SK_FlyA P0 同类）**：
  1. V1.23 `PlayerManager.BS_ExtraProjectiles` 用了 `[HideInInspector] public`——**HideInInspector 不阻止序列化**，玩家对象存在于关卡场景；
  2. 技能标签系统（V1.12）给 `ACT_skillSample` 新增 `public string SkillName`——该类经 `ACT_skillData` 内联嵌在快捷栏 `ACTListSkillBT`（崩溃报文点名的对象）中；
  3. V1.24 `WeaponClass.Craft_LockPrefix/Craft_LockSuffix/Craft_NoAttack/Craft_NoCaster` 4 个 public bool——`WeaponClass` 被装备栏 `CharButton`、`Hand`、`ItemScript`、`DropItemController` 等场景/资产对象**内联序列化**。
- **变更内容（4 文件各 1 行属性）**：`PlayerManager.cs`、`ACT_skillSample.cs`、`WeaponClass.cs`（4 字段）全部加 `[System.NonSerialized]`（工艺限制/珠宝加成等运行时逻辑不受影响——工艺限制改由 `WeaponSaveData` 存档链路持久化）。**level1 与用户存档均未损坏**（"corrupted"为布局错位误报）。
- **全字段审计**：原版 DLL ilspycmd 反编译树 × 当前树逐字段（含 `= new …()` 初始化器字段）比对——除上述 3 处外，其余差异均为 ILSpy 渲染噪音（异步状态机字段、Spine 命名空间别名、private struct SortEntry、事件、static/override）。修复后 Unity 序列化面与原版**完全一致**。
- **合并构建说明**：本版构建时工作树已含并行会话的 V1.24 铁匠工艺台源码，故本 DLL = **V1.24 全部功能 + 热修复**。
- **版本协调记录**：V1.25 首包（`B36E8004…`/zip `CAB54BDF…`）仅修根因 1，真机未复测即被本终版取代——**`B36E8004` 包作废勿装**；早期另版（`ED498A84…`）仅修根因 3，同样被本终版取代。
- **涉及文件**：`MODworkv2/decompiled/PlayerManager.cs`（1 行）、`ACT_skillSample.cs`（1 行）、`WeaponClass.cs`（4 行）、`MODworkv2/builds/ShadowDungeon-MOD-V1.25_2026-08-29/*`
- **产物 SHA256**：构建产物 `F35A0E1CD7F15B4C7FB2C9B129A14DBA1D9F6A7A3F4CD27B831ED499326161F4`（2,436,608 字节）；升级包 `ShadowDungeon-MOD-V1.25_2026-08-29.zip` SHA256 `07BE7E61471E72D92D1FC67B99234D801EC8991D868D4891E8B9ECD230354069`；部署目标一致（注：首次重制的 zip 内 install.ps1 误留作废首包的 B36E8004 校验值，安装时误报 SHA 不匹配——实际安装的 DLL 即正确终版；已修正重打，自检规则：打包脚本哈希替换必须断言）
- **验证状态**：`dotnet build -c Release` **0 error, 124 warnings（存量）**；部署完整性 SHA256 一致；VM 冒烟 **42s 存活 + Player.log 四项 0 命中 PASS**；**真机需用本包重装后重测进关卡**（V1.23/V1.24 包及 B36E8004 首包勿再安装）
- **部署状态**：✅ 已部署（ShadowDungeon 新版 Managed 目录；备份原版可回滚）

- **日期**：2026-08-29
- **背景**：用户要求新增 5 件测试装备——POEDB 式功能药剂（可重复饮用，有持续/冷却）、所有投射物环形发射的传奇戒指、投射物在最远距离/无法击中后返回的项链、所有投射物 +1 的镶嵌珠宝；全部固定出现在商人处且 0 元购买。图标复用原版、命名/描述走运行时本地化注入。
- **变更内容**（新文件 1 个 + 手术点 16 处，零资产改动，原版装备/技能行为零变化）：
  - **新文件 `PoeItemMod.cs`**：5 件商品定义（GlobalID 91001-91005）、穿戴门控（IsEquipped 扫 CharBT）、黄金角逐次旋转器、单体弹补射器、Item_MB 行运行时合成（Quality 5 传奇/PLtype 4 全职业/DropLevelStart=999 不进随机掉落池/图标音效借同部位原版饰品）、商店固定上架（CreatShop 头部调用→占 1 号买页前 5 格→Price=0）、Item_FY 本地化 fallback 注入（复用 PoedbSkillInjector 的 LOC._table 反射模式，语言切换清表后自动重注）
  - **商品 1/2 功能药剂（疾风之瓶 +40% 移速 5s/CD4s；洞悉之瓶 +30% 暴击 6s/CD6s）**：`UseItemClass.Use()` case1 新增 poe_flask_gale/poe_flask_insight → BuffManager.AddPotionBuff（时长增益，受药水时长词缀加成）+ SimplePotionManager.AddSimpleDrink（CDTime 冷却门控 HasSameDrink）；`BuffPotionItem` Init/DelBuff（MVSpeed_Tmp/BJrate_Tmp 进出）+ 文案/弹窗两处；`BuffManager.GetPotionIcon` 复用图标 13/4；`InventoryManager.UseItem/UseItemACT_Use` + `WarehouseManager.UseItem` 三处消耗段加 IsRepeatableFlask 门控（不扣堆叠不丢瓶）；UseItemACT 补两 case 支持快捷栏绑定
  - **商品 3 星环之戒（环形发射）**：`Gun.cs` 四个出手点（MGC/SQS/ARC/DEAD 的 FStype 0 case + MGC case 2 补射点）黄金角 137.5° 旋转（含 RTtypeOBJ==1 的 dic 向量随角重算）；`SK_Angle_F.FaShe` 扇形 Type 0/1/2 在 Count>1 时改走 SpawnEvenRing（360°/Count 均匀全环，FX 家族保持原 case）
  - **商品 4 回响之链（箭矢返回）**：`SK_FlyA.cs` SetStart 回旋判定追加 `PoeItemMod.ReturnEquipped`（try/catch 双路径），复用 V1.6 冰晶术返回机制——TimeStop（最远距离）与命中无穿透余量双路径 StartReturn，关碰撞飞回玩家
  - **商品 5 万箭之玉（镶嵌珠宝，所有投射物 +1）**：`WeaponBaoshiApplyUtil.GetSocketType` 新增 BStype "projectile"→Type 26（任意部位）；`WeaponClass.ApplySocketedGemStats` case 26 → 新字段 `PlayerManager.BS_ExtraProjectiles`（穿/脱 ±）；`SaveDataEquipmentSanitizer.GemFloatFields` 登记 26（存档读写不剥）；消耗双挂钩互斥防双倍——`ACTbar.SetSkill_Sample` 两处 `Count_F = Count_F_Last + ExtraProjectiles()`（弹幕族），`Gun.CreatSP` 补射走 `PoeItemMod.SpawnExtraProjectiles`（单体弹族 SK_FlyA/Ball/Follow/Sowrd，BuffTime>0 跳过防 SK_BuffA 重复注册）；`BaoshiClass.GetMain` 补 tooltip；`ShopManager` 新增公开 `CreatBS(SlotData)`（镜像 CreatUSE 的 CloneBaoshi 落位）
- **涉及文件**：`MODworkv2/decompiled/PoeItemMod.cs`(新)、`Gun.cs`(5处)、`SK_Angle_F.cs`、`SK_FlyA.cs`、`ACTbar.cs`(2处)、`ItemManager.cs`(2处)、`ShopManager.cs`、`UseItemClass.cs`、`UI.UIItems/BuffPotionItem.cs`(4处)、`UI.Managers/BuffManager.cs`、`InventoryManager.cs`(4处)、`WarehouseManager.cs`、`WeaponBaoshiApplyUtil.cs`、`WeaponClass.cs`、`PlayerManager.cs`(1字段)、`BaoshiClass.cs`、`Data.SaveData/SaveDataEquipmentSanitizer.cs`、`MODworkv2/builds/ShadowDungeon-MOD-V1.23_2026-08-29/*`
- **产物 SHA256**：构建产物 `8661C351F10456F4219519BFC3E936B1581CF8ED1357930F0F02B5080DAEA505`（2,411,008 字节）；升级包 `ShadowDungeon-MOD-V1.23_2026-08-29.zip` SHA256 `E15A895B6AF40D0F9C63BE3433FB9D77B18D0E7774BE7B4F4247DD46A31DE3CD`（800,479 字节）；部署目标一致
- **验证状态**：`dotnet build -c Release` **0 error, 124 warnings（存量）**；部署完整性 SHA256 一致；冒烟 **42s 存活 + Player.log Exception/Crash/TypeLoad/NullReference 0 命中 PASS**；游戏内购买/穿戴/饮用/镶嵌实操验收待真机
- **部署状态**：✅ 已部署（ShadowDungeon 新版 Managed 目录；备份原版可回滚）

## V1.22 — 主题迭代：火球术紫色（方案一 Flipbook）+ 冰晶术亮红（方案三 Shuriken）

- **日期**：2026-08-29
- **背景**：用户试玩 V1.21 后要求换主题——火球术改紫色、冰晶术改亮红。特效结构不变，纯常量迭代（验证了"颜色收敛为常量"设计的迭代效率）。
- **变更内容**：
  - `FxSpriteFactory.cs`：火焰图集配色常量化（FireTip/Body/Core/DarkColor 四常量），当前值换紫色系（端梢 #D9A8FF / 主体 #A855F7 / 暗核 #5B1FA8 / 余烬 #241040）
  - `SK_FlyA.cs`：三色常量改中性命名 CoreTint/MainTint/DeepTint（消除 Ice 命名与主题解耦），方法改名 ApplyShurikenStyle，当前值换亮红系（#FF5450 / #FF2E29 / #B80A1A）
  - `SK_FlyBall.cs`：仅注释更新（配色说明指向 FxSpriteFactory 常量）
- **涉及文件**：`MODworkv2/decompiled/FxSpriteFactory.cs`、`SK_FlyA.cs`、`SK_FlyBall.cs`、`MODworkv2/builds/ShadowDungeon-MOD-V1.22_2026-08-29/*`
- **产物 SHA256**：构建产物 `9A08B9C35DBB80132E5BE65DBB20CFDB9EECDC2C15EDF71F047B61BEE6BB96FB`（2,397,696 字节）；升级包 `ShadowDungeon-MOD-V1.22_2026-08-29.zip` SHA256 `634254D14221FDECC7D918729AE93EB80E26E57C544EE0B6AA13BE673BD0B93B`（795,435 字节）；部署目标一致
- **验证状态**：构建 **0 error, 124 warnings（存量）**；部署完整性 SHA256 一致；冒烟 **42s 存活 + Player.log 四项 0 命中 PASS**
- **部署状态**：✅ 已部署（观感迭代继续改常量即可）

---

## V1.21 — 火球术方案一 Flipbook + 冰晶术方案三 Shuriken 程序化特效（真机试玩版）

- **日期**：2026-08-28
- **背景**：用户看过三方案动态演示页后指定——火球术改方案一（自制序列帧）、冰晶术改方案三（Shuriken 深度程序化），真机试玩。基于 V1.20 源码构建（含注入修复全量）。
- **变更内容**：
  - **新文件 `FxSpriteFactory.cs`**：纯静态运行时贴图工厂——软圆/冰晶碎片/8 帧 512×64 火焰爆炸图集全部 `SetPixels` 代码生成（浮点缓冲 over 合成，固定种子确定性输出）；材质克隆游戏自带粒子 shader（`par`/`parLoop` 的 sharedMaterial 作模板，`_MainTex`/`_TintColor` 换装），回退 Sprites/Default；提供 SpawnFlipbookBurst（图集一次性播放）与 SpawnIceBurst（火花圈+旋转碎片+中心闪光）两个一次性发射器。零外部资产、零 POE 素材、无 public 序列化字段（typetree 安全）。
  - **火球术 → 方案一**（`SK_FlyBall.cs`）：SetStart 识别 `skillName=="FireBall"` 启动 Flipbook，Update 顶部 14fps 循环换 `Arrow.sprite`（不受 CanMV 影响）；TimeStop/Stop 双路径一次性爆裂（`_fxBurstDone` 护栏，SetStart 重置）。非 FireBall 零副作用。
  - **冰晶术 → 方案三**（`SK_FlyA.cs`）：撤销 V1.18 黑炎版（常量+方法删除），替换为 ApplyShurikenIceStyle——弹体白蓝亮核（#EAF6FF）+ 拖尾三段渐变（白蓝→淡冰→深冰淡出，time+0.06 max0.3）+ AttachShardTrail 飞行碎片子发射器（自绘碎片贴图，8/s，colorOverLifetime 曲线）；StartReturn 头部触发 SpawnIceBurst 末段爆裂（覆盖命中/超时双路径，一次性护栏）并清理子发射器；Stop 兜底清理。回旧行为不变。
- **涉及文件**：`MODworkv2/decompiled/FxSpriteFactory.cs`(新)、`SK_FlyBall.cs`、`SK_FlyA.cs`、`MODworkv2/builds/ShadowDungeon-MOD-V1.21_2026-08-28/*`
- **产物 SHA256**：构建产物 `410407D5693C0AA24F1F7147FE712AE495178CEB58717FCDDEC67FFA6191F370`（2,397,696 字节）；升级包 `ShadowDungeon-MOD-V1.21_2026-08-28.zip` SHA256 `96A5B71AD4386CA0D2D8C2C2732553C69113C7C4EF9E415D4343D0DC5C9982B4`（796,125 字节）；部署目标一致
- **验证状态**：`dotnet build -c Release` **0 error, 124 warnings（存量）**；部署完整性 SHA256 一致；冒烟 **42s 存活 + Player.log Exception/Crash/TypeLoad/NullReference 0 命中 PASS**；游戏内观感验收待真实机器
- **部署状态**：✅ 已部署（ShadowDungeon 新版 Managed 目录；备份原版可回滚；观感迭代只改 `SK_FlyBall.cs`/`SK_FlyA.cs` 类头常量）

---

## V1.20 — 技能参数"串台"修复 + 节点自动摆放

- **日期**：2026-08-28
- **背景**：用户真机截图反馈 V1.19 两问题——①悬停新技能显示的是"上一个悬停的原生技能"的参数（截图：标题旋风斩+狂风箭的蓝30/CD12/350%/需14点）②新节点位置不对（落在格线中间/压原生节点）。
- **根因**：
  - **串台**：注入器把克隆技能 SonA/SonB/SonC 清成 "0"；游戏 `GetManaSample/GetCD_Sample/GetBuffTime` **无空判解引用** `Sample_S[SonA]/[SonB]`（仅 SonC 有 "0" 特判）→ `ManaCost_Last` 抛 NRE → ShowSkilltip 恰在"标题已赋值、正文未赋值"之间中断 → 正文残留上一个悬停的原生技能（用户描述的"显示之前悬停的技能"逐字吻合）
  - **位置**：固定偏移 (95,-95) 是旧版树间距；新版网格实测约 155×170，偏移正好落格线中间
- **变更内容**：
  - SonA/SonB/SonC **保留模板原值**（龙卷射击←剃刀箭 Longbow/MultiShot；旋风斩←风之力 Whirlwind/Stormblade/Tailwind），符合游戏数据约定，tooltip/加点/施放的采样链全部恢复正常
  - 节点摆放改为**锚定格周边空闲槽位自动搜索**：网格步长 155×170，候选顺序同行右→左→下→上→斜角→外扩，须与全部既有节点间距 ≥45px 且在面板边界内；两个新节点依次搜索互不重叠；常量集中在文件头（GridStepX/GridStepY/SlotClearance/PanelMargin）
  - 证据链：新版 SampleF（path_id 1272）导出比对——Gale Arrow 蓝30/CD12/350%/解锁14 与截图逐项吻合；新版 Skill_FY（path_id 472）`info_Gale Arrow`="传送到目标位置并召唤6个龙卷风" 与截图描述逐字吻合
- **涉及文件**：`PoedbMod/PoedbSkillInjector.cs`（V2.2）、`MODworkv2/builds/ShadowDungeon-MOD-V1.20_2026-08-28/*`、`MODworkv2/tmp-csv/`（新版 CSV/Skill_FY 导出，取证用）
- **产物 SHA256**：构建产物 `38327E601DDE3DB61621DC70F8B6AA66895AE9B3DC24C799787491DF5637F670`（2,387,968 字节）；升级包 `ShadowDungeon-MOD-V1.20_2026-08-28.zip` SHA256 `B2B76F793FDF9D92D74B0F318D905E0B01FEDDA7C1C60E2D92B204AF859F9DDC`（792,510 字节）；部署目标一致
- **验证状态**：`dotnet build -c Release` **0 error, 124 warnings（存量）**；部署完整性 SHA256 一致；冒烟 **42s 存活 + Player.log 四项 0 命中 PASS**；游戏内可视验收待真实机器
- **部署状态**：✅ 已部署（ShadowDungeon 新版 Managed 目录；备份原版可回滚）

## V1.19 — 技能注入挂钩修正 + 自愈（修复 V1.18 节点不显示）

- **日期**：2026-08-28
- **背景**：用户真机反馈游侠天赋面板看不到 Tornado Shot/Cyclone 新节点（只有原生 3 系技能）。
- **根因**：V1.18 按钮克隆挂在 `TalentManager.OpenClose()`——全工程检索证实其为**零调用方死方法**；面板真正入口是 `GameUIManager.OpenClose_Talent()`（直接设 `BottomCAV[3]` 可见性，不经 TalentManager），克隆从未执行。
- **变更内容**：
  - 挂钩挪至 `GameUIManager.OpenClose_Talent()` 打开分支（一行，HasInstance 护栏 + try/catch）
  - `TryEnsureButtons` 自愈化：先 `TryInjectData` 再补按钮，打开面板一步到位
  - 去跨局静态短路（`_dataInjected`/`_buttonsEnsured`）：改为 ContainsKey/FindButton 天然幂等，退出重进对局不漏注入；LOC fallback 进程内一次（`_locInjected`）
  - 按钮搜索兜底：tm 层级 → FindObjectsOfType → Resources 全量（含未激活，scene.IsValid 过滤）；克隆引用重接兜底（模板 text 未初始化时直接找克隆体 "Text" 子节点）
  - TalentManager.OpenClose 上的 V1.18 挂钩移除（死方法，零残留）
- **涉及文件**：`PoedbMod/PoedbSkillInjector.cs`（V2.1）、`GameUIManager.cs`（1 行挂钩）、`TalentManager.cs`（移除死方法挂钩）、`MODworkv2/builds/ShadowDungeon-MOD-V1.19_2026-08-28/*`
- **产物 SHA256**：构建产物 `9AD86F9AB14E07341BDD4C53DAC353CB1155763A32B8F303928A77EBEA5BD4C8`（2,386,944 字节）；升级包 `ShadowDungeon-MOD-V1.19_2026-08-28.zip` SHA256 `EB863B26875BEC33CCFF975974A9CC84C7923313390860C1042880CF272EE644`（791,774 字节）；部署目标一致
- **验证状态**：`dotnet build -c Release` **0 error, 124 warnings（存量）**；部署完整性 SHA256 一致；冒烟 **2 分钟+ 存活 + Player.log 四项 0 命中 PASS**；游戏内可视验收待真实机器（VM 点击/键盘派发受前台焦点限制无法驱动游戏 UI）
- **部署状态**：✅ 已部署（ShadowDungeon 新版 Managed 目录；备份原版可回滚）

---

## V1.18 — 技能注入追加式干净重写 + 冰晶术黑炎测试特效（推倒 V1.9~V1.17 两条错误路线）

- **日期**：2026-08-28
- **背景**：用户反馈技能注入全链路异常（节点布局乱/原技能被顶/名称描述图标串台/无法施放）且冰晶术冰蓝特效观感差，要求推倒重做、不得在错误版本上打补丁。
- **变更内容**：
  - **注入重写（追加式）**：删除 `PoedbMod/PoedbReplaceInjector.cs`（替换式，覆写游侠 Razor Arrow 的元凶）与 972 行旧克隆式 `PoedbSkillInjector.cs`；全新重写追加式 `PoedbSkillInjector.cs`——原技能一律不动，四职业落地页 Xi 0/3/6/9 各追加 Tornado Shot（克隆该页首个主动技能，保留直射弹道，AllChuan_F=0 穿透，伤害 100%+30%/级/蓝 8/CD 1.2s）与 Cyclone（克隆该页首个 FStype 7/8/9 环绕型技能（游侠=Power of the Wind），保留环绕行为，伤害 80%+20%/级/蓝 10/CD 5s）；TalentManager 挂钩从 5 处散布收敛为 2 处（Start 首行 TryInjectData + OpenClose 打开分支 TryEnsureButtons）；按钮克隆走 ArcBoomerang PoC 真机验证路径（整格克隆+相对路径重接 text/SkillTU+原生 OnEnable/Start 自动注册，不手动挂监听）；节点锚定该页首个技能格右下 (95,-95)/(190,-95)（常量在文件头）；运行时 LOC fallback 注入 Skill_FY 标题+info_ 正文（文案与列值一致）；图标继承模板，data/poedb/icons/ 本地文件可选覆盖
  - **冰晶术特效重做（黑炎测试版）**：撤销 V1.16/V1.17 全部特效代码（ApplyMysticIceStyle/IceDust 程序粒子/StopIceDust 及 6 处调用点/V1.17 新增 OnDisable/_iceDustGO 字段）；新增 `ApplyBlackFlameStyle()`——实体/尺寸/弹道不动，仅换色：`Arrow.color` 近黑炎核 0.16,0.05,0.07 + `trail` 黑红三段渐变（核心 0.10,0.02,0.03 → 0.55,0.06,0.09 → 尾端透明，time+0.05 max0.25）+ `par` 游戏原生粒子全层重着色暗红 0.55,0.07,0.10；**不再程序新建无贴图粒子**（V1.16/17 方块光斑根源），主题仅 3 个 `BlackFlame*` 颜色常量可调
- **涉及文件**：`MODworkv2/decompiled/PoedbMod/PoedbSkillInjector.cs`（重写）、`PoedbMod/PoedbReplaceInjector.cs`（删除）、`TalentManager.cs`（挂钩收敛）、`SK_FlyA.cs`（特效重做）、`MODworkv2/builds/ShadowDungeon-MOD-V1.18_2026-08-28/*`
- **产物 SHA256**：构建产物 `66CCC0A02191FC31C917F15CB7AE14CFE1F85C6E7FD3C162D8D2B19277B2CBEB`（2,384,384 字节）；升级包 `ShadowDungeon-MOD-V1.18_2026-08-28.zip` SHA256 `7D5578D94829E77A0149E5C68BB680E6CA4306A0D27505E05744EEE1F58EB00A`（791,577 字节）；部署目标一致
- **验证状态**：`dotnet build -c Release` **0 error, 124 warnings（存量）**；部署完整性 SHA256 一致；冒烟 **40s 存活 + Player.log Exception/Crash/TypeLoad/NullReference 0 命中 PASS**；游戏内可视验收（天赋树新节点/黑炎观感）待真实机器
- **部署状态**：✅ 已部署（ShadowDungeon 新版 Managed 目录；备份原版可回滚）

## V1.16 — 冰晶术秘法特效：冰蓝换色 + 星尘尾迹 + 命中冰晶碎裂（SK_FlyA A+B2 车道）

- **日期**：2026-08-28
- **变更内容**：
  - **视觉增强（A+B2 零风险）**：`SK_FlyA.SetStart()` 尾部 `try` 内追加 `if (skillName=="Ice Crystal") ApplyMysticIceStyle()`（保留原 `ReturnToPlayer` 回旋 `Registry.IsBoomerangSkill`）+ 新增 `ApplyMysticIceStyle()` 私有方法：`Arrow.color 0.6,0.92,1,1` + `material.SetColor` 冰蓝；`trail.startColor 0.5,0.85,1,0.9 → endColor 0.7,0.92,1,0 + Gradient + time+0.12s(max0.35s)`；`par` 粒子 `startColor` 冰蓝 tint；`IceDust` 代码动态 `GameObject+ParticleSystem` 15 maxParticles/0.4s/0.12 size/8 rate/Circle 0.15r/`Particles/Additive` 冰尘
  - **保障**：全路径 `try/catch` + 空判 `dic/sp/Arrow/trail/par/Shader/material/renderer`，仅单文件 `SK_FlyA.cs`，不改资产
- **涉及文件**：`MODworkv2/decompiled/SK_FlyA.cs`、`MODworkv2/builds/ShadowDungeon-MOD-V1.16_2026-08-28/*`
- **产物 SHA256**：构建产物 `1CF2F53AF999B69B66C4F74FADEA1CD913C66517D2FE2905840BF45F9540FDDB`（2,406,912 字节）；升级包 `ShadowDungeon-MOD-V1.16_2026-08-28.zip` SHA256 `2B93C74F0BB4B753C723179396136DB894149481B3AEFCF8C367BF8FDC5CD509`（797,375 字节）；部署目标一致
- **验证状态**：`dotnet build -c Release` **0 error, 124 warnings**；部署完整性 SHA256 一致；冒烟 **38s 存活 + Player.log Exception/Crash/TypeLoad/NullReference 0 命中 PASS**
- **部署状态**：✅ 已部署（ShadowDungeon 新版 Managed 目录；备份原版可回滚）

---

## V1.17 — 冰晶术收敛修复：Trail缩短 + IceDust收敛 + 返程停发 + LogUtil修复（fix-2/fix-3 车道）

- **日期**：2026-08-28
- **变更内容**：
  - **Trail 收敛**：`startColor alpha 0.9→0.55`，`Gradient 0.9→0.55`，`time +0.12→+0.05 (max0.35→0.25)` 缩短30-50%，尾端透明度渐变
  - **IceDust 收敛**：`maxParticles 15→6`，`lifetime 0.4→0.18s`，`size 0.12→0.07`，`rate 8→3`，`radius 0.15→0.08`；`HasProperty` 保护 Additive 材质，Shader null 时保留默认材质避免粉色块
  - **返程停发**：新增 `_iceDustGO` + `StopIceDust()`（`emission.enabled=false + Stop(StopEmitting)`），在 `StartReturn()`/`StraightReturnMV()`/`TimeStop()`/`Stop()` 首行及 `OnDisable()` 兜底调用，返程立即停发
  - **编译修复**：`TalentManager.cs` 9 处 `LogUtil.Warning` → `LogUtil.Warn`（Warn(string,bool) 单参合法）
- **涉及文件**：`MODworkv2/decompiled/SK_FlyA.cs`（fix-2 14处 + 新增字段/方法）、`MODworkv2/decompiled/TalentManager.cs`（fix-3 9行 Warn）
- **产物 SHA256**：构建产物 `C8CE00B62F5A0A374D651163F89112354ADB990A69813239FAB722826B028F66`（2,412,544 字节）；升级包 `ShadowDungeon-MOD-V1.17_2026-08-28.zip` SHA256 `1A7B35F3ACDC73E3BE1F7AFAC54A4ADA0E497BB6D8D913357ECE214357AFFFA8`（799,004 字节）；部署目标一致
- **验证状态**：`dotnet build -c Release` **0 error, 0 warning**；部署完整性 SHA256 一致；冒烟 **38s 存活 + Player.log Exception/Crash/TypeLoad/NullReference 0 命中 PASS**
- **部署状态**：✅ 已部署（ShadowDungeon 新版 Managed 目录；截图过度拖尾与返程残留已收敛）

---

## V1.10 — 热修复：全职业可见（V1.9 双技能按钮克隆修复）

- **日期**：2026-08-27
- **变更内容**：
  - **根因**：V1.9 仅注入 Xi=6 风之游侠且未克隆天赋树按钮，若角色非游侠则看不到节点（用户截图：4系底部图标、30节点、0/4 不可见）。
  - **修复**：`PoedbSkillInjector.cs` 重写为全量 12 Xi 注入（落地页 0/3/6/9 全覆盖）+ `EnsureSkillButtons`/ `TryCloneButtonForSkill` 自动克隆按钮（Instantiate 同 parent 或 XiCAV[xi]，偏移 Tornado 95,-95 / Cyclone 190,-95，相对路径重接 Text/SkillTU，幂等 guard）；`TalentManager.cs` 在 Start/SetStart/OpenClose 4 时机点挂钩 InjectIfNeeded + EnsureSkillButtons + 全 Xi Refresh；日志增强 per Xi。
  - **结果**：任意职业首页（0/3/6/9）均可见两技能节点（0/4 灰度可点），旧档无需清档（关闭重开面板即刷新）。
- **涉及文件**：`MODworkv2/decompiled/PoedbMod/PoedbSkillInjector.cs`、`TalentManager.cs`、`PoedbMod/Registry.cs`（复用）、`MODworkv2/builds/ShadowDungeon-MOD-V1.10_2026-08-27/*`
- **产物 SHA256**：构建产物 `B540096E00D9C9F6F879C6337C01AB672FE221C5F31E27244456452D5FE891EF`（2,384,896 字节）；升级包 `ShadowDungeon-MOD-V1.10_2026-08-27.zip` SHA256 `2AE987E8C38FCCDF2FDDB97A3D019DC3DB3F767AC6D4B480B8E41E380AD481B5`（790,505 字节）；部署目标一致
- **验证状态**：`dotnet build -c Release` **0 error, 0 warning**；部署完整性 SHA256 一致；按钮克隆逻辑全代码审查通过（需用户按必现步骤人工复核：任意职业新建/读档 → P → 切 4图标均可见）
- **部署状态**：✅ 已部署（ShadowDungeon 新版 Managed 目录；install.ps1 已补 BOM 修复 ParserError）

---

## V1.9 — 游侠双技能：龙卷射击(Tornado Shot) + 旋风斩(Cyclone) 一级可学实装（poedb-双技能车道）

- **日期**：2026-08-27
- **变更内容**：
  - **数据**：`data/poedb/skills.json` 追加 `cyclone` 条目（Attack/Area/Melee，持续旋转近战范围），`shadow_dungeon_mapping` 模板 `Cleave`（回退 Razor Arrow），InfoKey `info_Cyclone`，列覆盖 `Xi=6 Price=0 UnLock_Point=0 Level_Max=4 FStype=7(环绕/附着) CountMulti=1 Damage 80+5/Lv Mana6 CD0.5 Size1.2 Range1 2.5`；`tornado-shot` 保留 FStype7 CountMulti6 穿透散射；`tools/poedb-pipeline/seed_data.py` 新增 `CYCLONE` 常量，`build_all()` 现 2 条，manifest 2 items；`nl-pack.py` 新增旋风/cyclone 兜底匹配
  - **运行时注入（代码注入优先）**：新增 `MODworkv2/decompiled/PoedbMod/PoedbSkillInjector.cs`（Xi=6 Windwalker/风之游侠 一级可学判定，模板查找三级回退，反射克隆 `SkillData_Sample_Father` 70+字段，override 指定列，写入 `XiData[6].Sample_F` + `SKI` + `FW`，幂等 guard，`LoadTalentTables` 尾部挂钩）；`TalentManager.cs` 加 `using PoedbMod` 并在 `LoadTalentTables` 内外各注入一次且 `Refresh(6)`；`SK_FlyA.cs` 加 `using PoedbMod` 且 `ReturnToPlayer` 扩展 `Registry.IsBoomerangSkill`（保留 Ice Crystal）；`SkillTagSystem.cs` 注释更新双技能形态说明；`Registry.cs` 重写 `CollectPoedbTags` 双技能分流（Tornado 多弹环绕 vs Cyclone 单体环绕）
  - **本地化**：`PoedbSkillInjector.InjectLocalizationFallback` 反射 `LOC._table` 注入 `Skill_FY.Tornado Shot/Cyclone` 与 `info_Tornado Shot/info_Cyclone` 中英（保证 tooltip 不空白）；正式合并到 `resources.assets` path_id=433 的 `localization.json` 流程已在 `docs/research/poedb-mod-framework-attempt3.md` 说明且 `builds/packs/cyclone & tornado-shot` 均生成 localization.json
  - **飞行行为**：Tornado 复用 SK_FlyA FStype7 环绕散射（Tier2 可二次 360°），Cyclone 复用 `Gun ARCattack case7` 附着玩家环绕（`SetParent(pl.transform)`），注释说明 Tier2 可复用 `SK_Round/SK_Sword` 持续光环
  - **工具链**：`seed_data` 重新物化 BOM，`schema.py validate` PASS，`nl-pack` 为双技能均生成 `builds/packs/cyclone|tornado-shot`（各 7 文件，含 samplef_row.csv/ localization.json）
- **涉及文件**：`data/poedb/skills.json`、`data/poedb/manifest.json`、`tools/poedb-pipeline/seed_data.py`、`tools/poedb-pipeline/nl-pack.py`、`MODworkv2/decompiled/PoedbMod/PoedbSkillInjector.cs`(新增)、`TalentManager.cs`、`SK_FlyA.cs`、`SkillTagSystem.cs`、`PoedbMod/Registry.cs`、`builds/packs/tornado-shot/*`、`builds/packs/cyclone/*`、`docs/research/poedb-mod-framework-attempt3.md`(新增)
- **产物 SHA256**：构建产物 `F14927CEBFA13BCC2F6705F6C5F775D62392ABAE731FC63B9EA19CCB738EDC6E`（2,379,776 字节）；部署目标一致（覆盖后校验一致）
- **验证状态**：`dotnet build -c Release` **0 error, 123 warnings**；`python schema.py validate` PASS；`nl-pack --list` Tornado+Cyclone 双可见 + 自然语言制包双 PASS；部署后 **35秒存活 PASS** + `Player.log` Exception/Crash/TypeLoad/NullReference **0 命中 PASS**
- **部署状态**：✅ 已部署（ShadowDungeon 新版 Managed 目录；resources.assets 未动走 fallback，备份原版可回滚）

---

## V1.8 — ShadowDungeon 新版：背包排序栏左侧垂直重排 + 分页修复（des-1/fix-2 车道）

- **日期**：2026-08-26
- **变更内容**：
  - **排序栏重排（des-1）**：`InventorySortBar.cs` 按钮组从顶部水平排列改为**贴着物品栏左侧边、垂直排列**；从上到下 **等级 → 稀有度**；锚定格子区左缘外侧（anchor=(0,0.5)、pivot=(1,0.5)、x=-8px 贴左外缘、垂直居中），组尺寸 120×68；按钮顺序反转为等级在上，`SortModeBase` 同步（等级→Level 基值 2、稀有度→Quality 基值 0）；点击重排、同键翻转升降序、激活态高亮逻辑不变
  - **分页修复（fix-2）**：`InventoryManager.ApplySort` 从"仅当前页 + single:true 重建"改为"跨全部 MainPages 收集 + single:false 跨页重建"（复用已验证的 SortAll 流程），并补 `RebindVisibleItemObjRegions()`/`EnsureCurrentPageItemObjs()`；修复排序后背包 2 页变 1 页（旧逻辑当前页放不下的物品被 Throw 丢弃）
- **涉及文件**：`MODworkv2/decompiled/InventorySortBar.cs`、`InventoryManager.cs`(ApplySort 重写)
- **产物 SHA256**：部署版 `71D874BF51239CA6D697F46A7C7F5E6B309FD3D4F9FDFBB3E29FD897528B66A8`（2,335,744 字节）；升级包 `ShadowDungeon-MOD-V1.8_2026-08-26.zip` SHA256 `307081F8C10C3EBF5141EEC9FD4039B21CE6B60427E4B43E67A51BD30B48A9D8`（770,148 字节）
- **验证状态**：构建 0 error（116 warning 存量）；部署完整性 SHA256 一致。**冒烟测试由用户人工完成**（本版本未自动冒烟）
- **部署状态**：✅ 已部署（ShadowDungeon 新版 Managed 目录；assets 未动；原版备份 `MODworkv2/backup/Assembly-CSharp-original.dll`）

---

## V1.7 — ShadowDungeon 新版：背包稀有度/等级排序（HUD 修改）

- **日期**：2026-08-26
- **变更内容**：
  - **背包顶部排序按钮栏**：新增 `InventorySortBar.cs`（全静态实现，P0 typetree 教训——不在现有 MonoBehaviour 上新增实例字段）；稀有度 / 等级 两按钮右对齐于格子区顶部；点击执行一次重排、同键再点翻转升降序（默认降序，高稀有度/高等级在前）；激活态高亮 + ↑↓ 箭头
  - **排序管线**：`InventoryManager.ApplySort(mode)` 新增公共入口（复用 SortAutoWeapon/Baoshi/UseItem 的 Cache→Sort→Rebuild 管线）；复合比较器=所选字段（Quality/Level）→ Price 降序回退 → 收集序号兜底；作用于**全部物品**（装备/药水/宝石），统一按 ItemClass 基类字段 Quality/Level 排序
  - **生命周期**：`InventoryManager.Update` 首行注入 `InventorySortBar.Tick(this)`（按 cav 可见性驱动按钮栏显隐）
- **涉及文件**：`MODworkv2/decompiled/InventorySortBar.cs`(新增)、`InventoryManager.cs`(ApplySort + Update 挂钩)
- **产物 SHA256**：部署版 `88C6AC1A8FBCA928D5B511C8FCE2842F9B3919349E0E62005882AEE63F689795`（2,335,744 字节）；升级包 `ShadowDungeon-MOD-V1.7_2026-08-26.zip` SHA256 `B5FAD692905DE2DD3A82BD998C50D6680C83E8598BC1BCD315E4077FC258D692`（769,951 字节，含 UTF-8 BOM 修复版 install.ps1）
- **验证状态**：构建 0 error（116 warning 存量）；部署完整性 SHA256 一致；冒烟 40s 存活 + Player.log Exception/Crash/TypeLoad/NullReference 零命中 PASS。游戏内可视验收（排序按钮栏渲染、稀有度/等级排序行为）待真实机器
- **部署状态**：✅ 已部署（ShadowDungeon 新版 Managed 目录；assets 未动；原版备份 `MODworkv2/backup/Assembly-CSharp-original.dll`）

---

## V1.6 — ShadowDungeon 新版：技能标签 + 冰晶术返回（decompiled-v2 车道）

- **日期**：2026-08-26
- **变更内容**：
  - **新版反编译工程建立**：`MODworkv2/decompiled`（895 个 .cs，ilspycmd 8.2.0 反编译 ShadowDungeon 新版 DLL）+ `MODworkv2/refs`（126 个引用 DLL ASCII 副本）；csproj 引用全部指向 refs；修复 2 处 `array[^1]`（System.Index 在 netstandard2.0 缺失，改为 `array[array.Length-1]` 语义等价）；构建 0 error（116 warning 存量）
  - **技能标签系统**：新增 `SkillTagSystem.cs`（移植自旧版，适配新版 `LOC.MM.GetSkill`）；双维标签（◆元素系蓝 + ◇行为形态橙）实时推导；注入 `GameUIManager.ShowSkilltip/RefreshSkilltip` 尾部；「回旋」白名单登记 `Ice Crystal`
  - **冰晶术返回效果**：`SK_FlyA.cs` 新增 `ReturnToPlayer/returning/StartReturn/StraightReturnMV`；`ReturnToPlayer` 门控 `skillName == "Ice Crystal"`；命中（非穿透）与超时（TimeStop）双路径触发返回，返回途中关碰撞防二次伤害，飞回玩家 0.6f 内回收
- **涉及文件**：`MODworkv2/decompiled/SkillTagSystem.cs`(新增)、`SK_FlyA.cs`、`GameUIManager.cs`、`ItemManager.cs`(2 处 ^1 修复)、`Assembly-CSharp.csproj`(重写引用)
- **产物 SHA256**：部署版 `2AA9834193F09FE0F9524E87366CD3E78766A349E5576B499CE33F0E4B72E8E1`（2,329,600 字节）
- **验证状态**：构建 0 error（116 warning 存量）；部署完整性 SHA256 一致；冒烟 40s 存活 + Player.log Exception/Crash/TypeLoad/NullReference 零命中 PASS。游戏内可视验收（tooltip 标签渲染、冰晶术返回行为）待真实机器
- **部署状态**：✅ 已部署（ShadowDungeon 新版 Managed 目录；assets 未动；原版备份 `MODworkv2/backup/Assembly-CSharp-original.dll`）

---

## V1.5 — 背包筛选行重做：格子下移 + 整行筛选器 + 排序（exp-4/fix-5/fix-6 车道）

- **日期**：2026-08-25
- **变更内容**：
  - **格子下移一格**：`InventoryCategoryTabs.EnsureGridShifted()` 首次面板可见时 `Gird.anchoredPosition.y -= slotSize`（幂等 guard，逻辑层零改动——物品摆放/悬停/tooltip 全部动态取槽位 transform 自动跟随）
  - **筛选栏迁入顶行**：页签栏 parent 改挂 IVgird 自身，local pos=(0, +slotSize×0.5) 垂直居中于腾出的顶行（无时序依赖）；底板拉通全行（宽=格子区宽−8px，深色半透明 raycastTarget=false 不挡格子）
  - **右侧排序按钮组**（新文件 `InventorySortBar.cs`）：稀有度/获取时间/等级 三按钮右对齐同带；点击执行一次重排、同键再点翻转升降序（首次默认 稀有度/等级降序、时间升序）；激活态高亮+↑↓箭头
  - **排序管线**：`InventoryManager.ApplySort(mode)` 公共入口复用 SortCur 的 Cache→Sort→Rebuild 管线，复合比较器=所选字段→Price 降序回退→收集序号兜底；完成后调 `NotifyPageChanged` 让视图过滤自愈
  - **获取时间追踪**：三个 SaveData 加 `public long AcquiredAt`（plain class 不触 P0 红线，旧档缺省 0 恒排末尾）+ FromRuntime/ApplyToRuntime 映射；运行时以 ConditionalWeakTable 注册表承载（防 GC 泄漏+身份快照防槽位对象复用串档）；打点覆盖拾取直入/手持入包/仓库商店入包/换位保时全链路
- **涉及文件**：`InventoryCategoryTabs.cs`、`InventorySortBar.cs`(新)、`InventoryManager.cs`、`Data.SaveData/{Weapon,Baoshi,UseItem}SaveData.cs`
- **产物 SHA256**：部署版 `52427E63BD893640CF3673B2E54515535C0845FF05471E85275B7ECC7F1D0516`（1,718,784 字节）；补丁包 `ShadowDungeon-HUD-Patch-V1.5_2026-08-25.zip` SHA256 `34C34E4A04C02392FD1B68D362DED407A9F023BE2973DCC43A81777A897F627C`（1,123,285 字节）
- **验证状态**：构建 0 error（114 warning 存量）；部署完整性 SHA256 一致；冒烟 42s 存活 + Player.log 四项零命中 PASS。游戏内可视验收（顶行对齐观感、排序按钮 y 微调常量集中在 InventorySortBar.cs 文件头）待真实机器
- **部署状态**：✅ 已部署（游戏 Managed 目录；assets 未动）

## V1.0 — POE 三技能运行时注入（fix-l1 / loop-mt6011d6-xq9b51）

- **日期**：2026-08-24
- **变更内容**：新增运行时注入器，向技能系统注册 3 个移植自 POEDB.TW（v3.29.0 数据规格，见 `docs/research/poe-projectile-data-lib1.md`）的投射物技能并自动装填快捷栏：
  - `POE_Fireball`（Xi0 火）：直飞弹体 + colEXP 命中即爆 AoE + 合成 fire DOT 行实现点燃（25%/4s）
  - `POE_IceSpear`（Xi1 冰）：AllChuan 全穿 + 双弹 + MoveSpeedCut 30/2s 减速（BuffMG_EM 链）
  - `POE_LightningArrow`（Xi2 电）：直飞全穿多段逐敌命中 ≈ POE 溅射群伤 + LastEXP 末段爆裂
- **涉及文件**：`modwork/decompiled/PoeSkillInjector.cs`（新增）、`modwork/decompiled/BootstrapEntry.cs`（+1 行挂钩）
- **产物 SHA256**：部署版 `185BC7CB52374AD47B2F5B4E5A5A2D19626D643536B3BE4F5BC06AFCDD6F3B05`（1,701,888 字节；已随回滚移除）
- **验证状态**：构建 0 error；无头 harness 全管线断言 27/27 通过（字典注册/数据包/DOT/快捷栏绑定）；菜单态 42s 存活零异常；游戏内注册日志在本 VM 因 HomeScene 崩溃不可达（判定记录 `.opencode/loop-history/loop-mt6011d6-xq9b51/history-001|002.md`，真实机器终验待执行：`-poemod-autostart`）
- **⚠ 重出包须知**：该版本的部署二进制内含当时尚未修复的 SK_FlyA public 序列化字段缺陷（HomeScene 崩溃元凶，V1.1 已修复）。**重新出包必须基于含 `[NonSerialized]` 修复的当前源码**，否则必复现场景崩溃。
- **部署状态**：❌ 未部署（已回滚）

## V1.1 — 投射物机制包（另一会话）

- **日期**：2026-08-23 ～ 2026-08-24
- **变更内容**：
  - **SK_FlyA 回旋镖返回**：`ReturnToPlayer/StartReturn` 追踪返程 → **P0 序列化陷阱修复**（public 字段插类中部致 typetree 偏移、原生读取 sharedassets1 越界崩溃 → `[System.NonSerialized]` 还原布局，真机验收通过）→ 直线返回改造 `StraightReturnMV()`
  - **ArcBoomerang 新技能节点**：SampleF CSV 克隆 Razor Arrow 行加行（72 行×152 列自验）+ `TalentManager.TryCloneArcBoomerangButton()` UI 按钮克隆注册（幂等 guard）
  - **剃刀箭环形 8 箭**：`Gun.ARCattack case 0` 按 `dt.skillName == "Razor Arrow"` 门控追加 7 支 45° 环形副箭（RTtypeOBJ/FX 定向出特效）
  - **描述同步**：Skill_FY 本地化 772→774 键（`info_Razor Arrow` 更新环形8箭+回旋文案；新增 `ArcBoomerang` 名键与描述键）
  - **真机验收 PASS**（RTX 5070）：ArcBoomerang 节点可见/加点/施放、箭矢回旋确认；返回点 45° 偏差记 KI-001
- **涉及文件**：`SK_FlyA.cs`、`Gun.cs`、`TalentManager.cs`、`resources.assets`（Skill_FY path_id=433）、`sharedassets1.assets`（SampleF path_id=1276）
- **产物 SHA256（历史阶段）**：PoC 双文件部署 DLL `FFD42295…C225` + assets `4B4E7C10…50B2`；ring-test DLL `B738EB58…BCBF`
- **验证状态**：真机隔离矩阵（原版/PoC assets/新 DLL 二分 + PRISTINE 对照）定位 P0 根因；V3 修复后真机 Play 场景验收通过；本机冒烟多轮零异常
- **部署状态**：❌ 未部署（已全量回滚至原版）

---

<!-- 下一版本号：V1.6。新条目追加在本注释之下，按版本号降序（新在上）。 -->

## V1.4 — 合并终版：HUD V1.3 + 技能协同装备化（同日合并，零冲突）

- **日期**：2026-08-25
- **变更内容**：
  - **HUD 侧（V1.3 保留）**：背包三页签夹缝带（`InventoryCategoryTabs.cs`）+ 技能标签（`SkillTagSystem.cs` + 全表 `skill-tags-catalog.md`）；天赋树已退回原版
  - **协同侧（装备化二期联动）**：新增 `CustomEquipGate.cs`（`Custom1=Ring` / `Custom2=Return` 装备持有判定 `CharButton.hasWeapon`）+ `Gun.cs` 4 处环形门控 `Has(RingItem)?z+=RingCursor.Next()`（137.5°步进）+ `SK_FlyA.cs:535` 回旋门控 `ReturnToPlayer=Has(ReturnItem)` + `ItemManager.cs` 商店固定位注入 `CreatCustomEquips()`（GlobalID 90001-90003，try/catch 包裹，品级 Epic）
  - 合并判定：两会话文件级零重叠（HUD 动 Inventory* / SkillTag，协同动 Gun/SK_FlyA/ItemManager/CustomEquipGate），源码盘已自然共存；排序与筛选语义互补（自定义装备归“通用”页签）
- **涉及文件**：`CustomEquipGate.cs`(新增)、`Gun.cs`、`SK_FlyA.cs`、`ItemManager.cs`、`InventoryCategoryTabs.cs`、`SkillTagSystem.cs`、`docs/research/skill-tags-catalog.md`
- **产物 SHA256**：`B5D16760B97167E132067C41A6DE0EE604884D1F4B07F74370EB8B5CEA5B02FA`（1,709,568 字节，与 V1.3 同哈希——协同改动在 V1.3 构建时已在盘，clean 构建复核一致）；补丁包沿用 `ShadowDungeon-HUD-Patch-V1.3_2026-08-25.zip`（`3CF4D7DB…6800`）即为合并终版物化包
- **验证状态**：clean 构建 0 error（114 警告存量）；部署 SHA256 一致；冒烟 42s 存活 + Player.log 四项零命中 PASS（EXPLORE 阶段确认无冲突）
- **部署状态**：✅ 已部署（游戏 Managed 目录；assets 未动）—— 两会话改动同捆生效

## V1.3 — HUD 迭代：退回技能树双页 + 背包页签重定位（obs-1/fix-3/fix-4 车道）

- **日期**：2026-08-25（V1.2 同日迭代）
- **变更内容**：
  - **移除技能树「主技能/增幅」双页切换**（用户要求退回）：删除 `TalentPageFilter.cs` 及 TalentManager（RegisterSkillBT/Start/TryBuildTalentPageTabs/BuildOnePageTab/CreateFallbackTabText/SetStart 五处）与 GameUIManager.OpenClose_Talent 的全部挂钩，天赋树恢复原版布局；全工程零残留引用
  - **保留技能 Tooltip 双维标签**：SkillTagSystem 零改动继续生效；标签全表与机制说明落盘 `docs/research/skill-tags-catalog.md`
  - **背包三页签重定位重做视觉**：从面板顶部中央迁至「装备人偶区底部/物品格子顶部」夹缝带（面板高度 29% 处，截图实测全宽空置带），左对齐格子区左缘（12px 边距）；页签 100×28、间距 12、字号 17；新增半透明深色底板（复用 closeBtn sprite sliced，alpha 140，raycastTarget=false 不挡格子）；交互逻辑零改动。定位公式 `y=-panelH×0.29 / x=+12px`（锚 (0,1) pivot(0,1)），常量 GapYFraction/LeftMarginPx 可调
  - 过程记录：designer 车道因 provider 网络错误失败（无半成品写入，核查后由 fixer 按锁定规格重实现）
- **涉及文件**：`TalentPageFilter.cs`(删除)、`TalentManager.cs`、`GameUIManager.cs`(仅移除一行钩子)、`InventoryCategoryTabs.cs`、新增文档 `docs/research/skill-tags-catalog.md`
- **产物 SHA256**：部署版 `B5D16760B97167E132067C41A6DE0EE604884D1F4B07F74370EB8B5CEA5B02FA`（1,709,568 字节）；补丁包 `ShadowDungeon-HUD-Patch-V1.3_2026-08-25.zip` SHA256 `3CF4D7DBA05A02D85F929410B45778F9257992539CAA845C2505677CA3026800`（1,115,112 字节）
- **验证状态**：构建 0 error（114 warning 存量）；部署完整性 SHA256 一致；冒烟 42s 存活 + Player.log 四项零命中 PASS。游戏内可视验收（页签新位置观感、tooltip 标签）待真实机器
- **部署状态**：✅ 已部署（游戏 Managed 目录；assets 未动）

## V1.2 — HUD 改造：技能树双页 + 背包三标签 + 技能标签实时同步（fix-1/fix-2 双车道）

- **日期**：2026-08-25
- **变更内容**：
  - **技能树「主技能/增幅」顶级双页切换**：新增 `TalentPageFilter.cs`（静态过滤器+页签状态）；主技能页=Father 类节点（type 0/2/4），增幅页=子节点与倍率类（type 1/3/5/6）；页签克隆 SkillXiBT 样式置于 Xi 行上方（克隆体剥离 SkillXiBT/Button/UIButtonState 防持久化 onClick 误触，挂自建 `TalentPageTabBT`）；`OpenClose_Talent`/`RegisterSkillBT`/`SetStart` 四处联动刷新；隐藏节点经 OnEnable→RegisterSkillBT 自动重绑；加点逻辑零改动
  - **背包「药水/镶嵌物/通用」三分类页签**：新增 `InventoryCategoryTabs.cs`（视图过滤式，不动物理槽位与存档语义）；药水=UseItem InfoType 0/1、镶嵌物=Baoshi(ItemType 1)、通用=其余（武器+传送门+永久+特殊+扩容）；非匹配物品 alpha×0.25+禁射线（精确保存/还原原色），同签再点=全部；翻页 override 重施、面板开关 alpha 沿检测、0.5s 节流计数刷新；WarehouseManager 与 ContainerManager 基类零变化
  - **技能 tooltip 双维标签实时同步**：新增 `SkillTagSystem.cs`（纯静态推导）；元素系◆蓝（12 系，GetSkill(系IndexName) 优先/常量回退）+ 行为形态◇橙（直射/位移/环绕/落点/穿透/命中爆炸/末段爆裂/多弹/追踪/减速/DOT 词/回旋白名单{Razor Arrow, ArcBoomerang}）；注入 `ShowSkilltip`/`RefreshSkilltip` 尾部每次实时重算（数值走 _Last 访问器含天赋加成→加点即刷）；`RegisterTagContributor` 扩展钩子为装备化二期预留；异常全降级不阻断原 tooltip
  - **构建修复**：SkillTagSystem 的 richText 防御块与本工程 UI 模块 API 不符（CS1061×2）→ 删除冗余块（原生 GetInfoA 已依赖富文本开启，零行为影响）
- **涉及文件**：`TalentPageFilter.cs`(新)、`InventoryCategoryTabs.cs`(新)、`SkillTagSystem.cs`(新)、`TalentManager.cs`、`GameUIManager.cs`、`InventoryManager.cs`
- **产物 SHA256**：部署版 `DABDD364CA7797A6691E42952C92577DB79C7614300B6370B09D6B2B7EA0C779`（1,714,176 字节）
- **验证状态**：构建 0 error（114 warning 均存量）；部署完整性 SHA256 比对一致；启动冒烟 42s 存活 + Player.log Exception/Crash/TypeLoad/NullReference 零命中 PASS。本 VM 仅菜单态验证（KI-003）：技能树双页切换、背包页签过滤、tooltip 标签渲染的**游戏内可视验收待真实机器**
- **部署状态**：✅ 已部署（游戏 Managed 目录；assets 两文件未动）
