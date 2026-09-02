# 状态日志 (Status)

> 最后更新：2026-09-02（V1.35 DLL 版本与防旧产物构建链修复）

## 当前阶段

**V1.35 DLL 版本元数据与防旧产物构建链已完成源码修复，待完整 Windows 工作区编译/部署。** 根因是工程关闭 SDK AssemblyInfo 生成且反编译 `AssemblyInfo.cs` 固定 `AssemblyVersion=0.0.0.0`；现改由 `Directory.Build.props` 统一生成 `FileVersion=1.35.0.0` / `ProductVersion=1.35.0`，程序集身份版本为兼容 Unity 保持 `0.0.0.0`。新增 `build-mod.ps1` 唯一发布入口（refs 预检、clean、no-incremental、独立输出、时间/版本/SHA256 三重校验）、自校验安装器模板与 `verify-dll.ps1`。GitHub 纯仓库不含 gitignored 的 127 个引用 DLL 和游戏本体，本环境未伪造 DLL 构建；须在用户完整工作区执行说明中的构建和冒烟流程。

**ShadowDungeon 新版 V1.34（2026-09-01 Scheme A 回灌版，82AF138C）已封版**——Game-Later vanilla `92E0120F…2D52` + V1.32 六项回灌（`LastCastInfo`/`BuffTime死门`/`组件白名单`/`TargetPos环向`/`3+N单环均分`/`词条双回退`），全量 typetree `895 文件全部一致`，`dotnet build 0 error/122 warnings`，构建 `82AF138C…64E5C`；新升级包 `ShadowDungeon-MOD-V1.34_2026-09-01.zip`（`37492817…7D06`，`82AF138C…`，HASH_MATCH_PASS）。P0 已解除，旧版已归档至 `_archive/DELETE-2026-09-01/`，本次按指示**不做自动化部署**。

**ShadowDungeon 新版 V1.32（四轮修订）词条档位全标注 + 识别诊断行 + 出手遥测 + 星环 BuffTime 死门移除已部署（含 V1.31 重建版星环总出口修复，2026-08-30）。** 用户截图反馈：①词条档位"有的有标注有的没有"+饰品伤害+16% 显示 T5|[21-26] 落在范围外②星环/回响仍然无效果。修复：①AffixTierDisplay 双回退——固定词条（Index 不在本基底池）回退**全局档位梯**（整张池表该 Index 全部 NB，T=全游戏同类名次），值落不进本池回退**品质档家族池并集**（点金/蜕变升品质后词缀是低档池 roll 的）；白字/元素行仍不标注（非池词条）。②星环根因（挂钩在 case 0/2 内，FStype∉{0,2} 技能不走星环）已由并行会话在 V1.31 重建版以**总出口修复**解决；本会话曾试验 ACTbar Count_F 通道，取证 SK_Fly 族不消费 Count_F+双通道重复风险后**撤销**，保持总出口单一通道。③`IsEquipped` 去掉 hasWeapon 硬性要求 + 星环/回响 tooltip 加灰色 `[MOD] 识别:` 诊断行（名称/GlobalID 匹配+装备槽扫描结果）——**验收截图该行即可定位，无需日志**。二轮：诊断行改一眼可读状态（`⚠ 未穿戴（在背包中）——右键装备到戒指/项链槽后生效` / `✓ 已穿戴生效`）——用户截图实证两件当时均在背包。三轮：**出手遥测**（LastCastInfo：已生成 N 枚/BuffTime 拦截/组件不匹配，悬停即见）+ **TargetPos 环向修正**（克隆弹沿用本体同一目标点会收拢成一点）。四轮：遥测实锤终极根因 `[FireBall] 被 BuffTime=1 门控拦截`——**V1.23 遗留 BuffTime>0 死门移除**（SK_Fly 四族与 CreatSP 均不消费该字段；星环「万箭可见而星环无感」的完整解释=万箭走 Count_F 通道不经此门）；克隆弹经 CreatSP 按当前技能样例全量构建（伤害/暴击/穿透/弹速一致）。构建 0 error、部署 SHA `7A5ED0BC…89DDC8` 一致、42s 冒烟 LOG CLEAN。升级包 V1.32 zip 四轮（`39AB1801…B92F`，脚本断言 HASH_MATCH_PASS；二轮包曾因 install.ps1 哈希手抄错误校验失效，已修正并把校验改为脚本断言）。**待真机**：穿星环放火球→5 弹成环（对照天钻护符「元素引集新星」原生环形观感）；诊断行「上次出手」应显示已生成 4/4 枚。

**V1.31 同号合并修订：星环总出口修复并入（2026-08-30）。** 用户以火球术/冰晶术实测仍无环状发射→根因=星环出手点钩子原挂在 MGCattack 等 4 个攻击函数的 case 0/2 分支内，而 MGCattack 有 11 个 FStype 分支（火球/冰晶不在其中）→ 钩子不执行。修复：`SpawnExtraProjectiles` 挪到每个攻击函数 switch 之后（MGC/SQS/ARC/DEAD 总出口，任意 FStype 全覆盖，组件守卫防误生环）。最终合并构建 `2216612A60C8818F9A937BD33FBED4AC6F59FB85FB51CA3C671E193DF6971EC6`（词条档位显示+星环修复双功能）已部署并冒烟 PASS；包 zip `035E3EFA…0700`。EE20C084 为早一步同树构建（缺总出口修复）已取代。

**ShadowDungeon 新版 V1.31 词条档位显示（T几·第X名/共N档·可roll范围）已部署 = V1.30+V1.31 功能合并体（2026-08-30，并行撞号协调）。** 需求：词条行尾显示 T 几（T1 满档最好）+占同类词条第 X 名+可 roll 范围，先核对原生词条叫啥。调研（落盘 `docs/research/affix-tier-display-design.md`）：游戏原生无 Tier 体系（显示名走 Main_FY/MainDisplay_FY/SKStat_* 本地化键，`WeaponClass.GetMain/GetDot/GetSK/GetCP` 拼接）；随机池 `Item_MB.RateMain/RateDot/RateSK/RateCP` 内同 Index 天然存在 2~3 条 NB 档位=档位阶梯；roll=档位 NB×等级段乘数（非秘境 0.9~1.3/秘境按 DropScene 1.2~1.6，秒回/整数成长/Fixed 各有专门公式），物品持久化 Level/Quality/DropScene 可精确重建上下文。落地：新文件 `PoedbMod/AffixTierDisplay.cs`（静态纯显示层，CraftFindTemplate 反查池+分类表逐字复刻 ItemManager+可达区间重建+值→档位匹配，零实例字段 typetree 安全）+ `WeaponClass.cs` 四处循环行尾注入（套装共鸣行不受影响）。显示格式 `T1 | [9-26]`（同日用户定稿紧凑格式，名次折算进 T），T1 金色其余灰，机制类无 roll 词条自动不加。构建 0 error、部署 SHA `7A5ED0BC…89DDC8` 一致、42s 冒烟 LOG CLEAN。升级包 V1.31 zip（`668F079A…E76B2`）。**并行撞号**：V1.30 源码在 V1.31 构建前已入树→本 DLL=V1.29+V1.30+V1.31 合并体（V1.25 先例），V1.30 包目录误写入已从其 zip 还原（SHA 复验一致）；从 V1.30 包升级的用户直接升 V1.31（功能超集）。**待真机**：tooltip 档位标注；V1.30 的回响穿透/星环根因日志回传仍有效。

**ShadowDungeon 新版 V1.30 回响 +1 穿透/返程命中 + 星环成环补全与诊断已部署（2026-08-30；部署体已被 V1.31 合并体取代）。** 真机反馈：星环 +4 未生效未成环（万箭可见而星环无感——穿戴识别或技能路径存疑）；词条颜色霓虹青已确认。落地：①回响之链 +1 穿透（pierceLeft，命中拦截 ChainPierceOrReturn/Stop 门控穿透分支，**穿透优先于返回**对齐 poedb 优先级）②去程/返程双命中（StartReturn 保留碰撞，OnTriggerEnter 返程守卫+ReturnHit 只伤害不终止，同目标去返各一次）③星环弹幕 type 3/4 补成环分支（此前仅 0/1/2）④IsEquipped 加 GlobalID 兜底匹配 ⑤每次出手 cast 日志 + ring=False 时装备槽 dump（5s 节流）——下次真机日志直接定位星环根因。构建 0 error、部署 SHA `7A5ED0BC…89DDC8` 一致、42s 冒烟 LOG CLEAN。升级包 V1.30 zip（`668F079A…E76B2`）。**待真机**：回响穿透/返程命中；星环若仍无效回传 Player.log 的 `[PoeItemMod]` 行。

**ShadowDungeon 新版 V1.26 工艺台适配 + 商店 5 件自愈已部署（2026-08-29）。** 真机试玩反馈三点：①商店 5 件只出 3 件（缺戒指/项链，武器路径上架件）②工艺台字体偏小、改点击选装③工艺需按原生品质档配词缀上限。落地：上架挪 CreatShop 尾部+按 GlobalID 查重+`SortBuy` 尾部 `VerifyShopStock` 校验补架+全链路 `[PoeItemMod]` 日志（静态分析与原版路径同构无法复现缺件，自愈兜底+日志定位）；`AffixCap` 品质档阶梯 普通0/魔法4/稀有6/精致7/史诗8/传说9/神话10（对齐 `GetGeneratedWeaponSkillCount` 原生曲线）；工艺台字体加大+EventSystem 点击选装+目标行 n/cap。构建 0 error、部署 SHA `7A5ED0BC…89DDC8` 一致、42s 冒烟 LOG CLEAN。升级包 V1.26 zip 落盘（`A77DF435…ED6F`）。商店 5 件/工艺台实操验收待真机。

**ShadowDungeon 新版 V1.25 热修复已部署（2026-08-29，合并版=V1.24 铁匠工艺台全部功能+typetree 修复）。** 用户真机（G:\SteamLibrary）进关卡崩溃：`different serialization layout` → `level1 corrupted` → `Crash!!!`。根因=三处新增字段未阻断序列化（全字段审计确认）：① V1.23 PlayerManager.BS_ExtraProjectiles 用 [HideInInspector] public（不阻止序列化）；② 技能标签系统（V1.12）在 ACT_skillSample 新增 public string SkillName（经 ACT_skillData 内联嵌在崩溃报文点名的 ACTListSkillBT 中）；③ V1.24 WeaponClass.Craft_* 4 字段（被 CharButton/Hand/ItemScript 等场景对象内联序列化）——均与 V1.0 SK_FlyA P0 同类。修复=三处全部加 [System.NonSerialized]，序列化面与原版完全一致。level1/存档未损坏（误报）。构建时树内已含并行会话 V1.24 铁匠工艺台源码，本 DLL=双方合并（元数据实测含 CraftBench*/BS_ExtraProjectiles/PoeItemMod）。构建 0 error、部署 SHA `7A5ED0BC…89DDC8` 一致、42s 冒烟 LOG CLEAN。升级包 zip `5A2A3652…5B21`（早版 V1.25 B36E8004 仅修 1/3 根因已作废）。**V1.23/V1.24 升级包带崩溃 bug 已停用，真机请装 V1.25 zip。**

**ShadowDungeon 新版 V1.24 铁匠工艺台已部署（2026-08-29，被 V1.25 合并取代）。** 用户要求参考 poedb.tw/cn/metamods「工艺互动」表给铁匠增加装备各类制作选项，各选项暂定固定 1 金币。落地：铁匠（锻造面板）新增「工艺台」按钮 → 运行时 uGUI 工艺台（PoedbMod/CraftBenchUI.cs），13 项货币工艺（蜕变/增幅/改造/富豪/点金/混沌/隐匿混沌/崇高/无效/神圣/重铸/兽猎移前增后/移后增前）+ 4 项工艺限制（前缀/后缀无法被变更、无法骰出攻击/法术词缀，附加装备随存档），锁矩阵对齐 poedb 表；词缀随机全走原生模板池与成长公式（PoedbMod/CraftBenchOps.cs + ItemManager 桥接 region），品质=原生 0-6 档。其包 `45DB54BE…72F7` 基于 V1.23 未修复树构建，**含崩溃 bug 勿安装**；功能并入 V1.25。设计记录 docs/research/craftbench-metamods-design.md。

**ShadowDungeon 新版 V1.23 POE 测试装备包已部署（2026-08-29）。** 用户要求新增 5 件测试装备并固定上架商人 0 元：疾风之瓶/洞悉之瓶（POEDB 式功能药剂，可重复饮用+持续+冷却，不消耗瓶身）、星环之戒（投射物环形发射：黄金角逐次旋转+扇形弹幕转 360° 全环）、回响之链（箭矢超时/无穿透命中后返回，复用冰晶术返回机制）、万箭之玉（镶嵌珠宝所有投射物 +1，穿脱 ± 且入存档白名单）。新文件 PoeItemMod.cs（定义/门控/行合成/商店固定上架/LOC 注入）+ 16 处手术点，零资产改动。构建 0 error、部署 SHA `7A5ED0BC…89DDC8` 一致、42s 冒烟 LOG CLEAN。升级包 V1.23 zip 落盘。游戏内购买/饮用/穿戴/镶嵌实操验收待真机。

**ShadowDungeon 新版 V1.22 主题迭代已部署（2026-08-29，真机试玩版）。** 用户指定换色：火球术 Flipbook 图集换紫色系（FireTip/Body/Core/DarkColor 四常量烘焙）、冰晶术 Shuriken 换亮红系（CoreTint/MainTint/DeepTint 三常量，中性化命名）。结构零变化，构建 0 error、部署 SHA `7A5ED0BC…89DDC8` 一致、42s 冒烟 LOG CLEAN。升级包 V1.22 zip 落盘。观感继续迭代=改常量。

**ShadowDungeon 新版 V1.21 火球术 Flipbook + 冰晶术 Shuriken 特效已部署（2026-08-28，真机试玩版）。** 用户看过三方案演示页后指定落地：火球术=方案一（自绘 8 帧火焰图集 14fps 循环换帧 + 命中/到时一次性 Flipbook 爆裂）；冰晶术=方案三（白蓝亮核 + 拖尾三段渐变 + 飞行碎片子发射器 + 返程末段爆裂=火花圈/旋转碎片/闪光），撤销黑炎版。新文件 FxSpriteFactory.cs 全部运行时自绘贴图（零外部资产零 POE 素材，材质克隆游戏粒子 shader），无 public 序列化字段。构建 0 error、部署 SHA `7A5ED0BC…89DDC8` 一致、42s 冒烟 LOG CLEAN。升级包 V1.21 zip 落盘。基于 V1.20 源码（含注入修复全量），观感迭代=改类头常量。游戏内观感验收待真实机器。

**ShadowDungeon 新版 V1.20 技能参数串台修复 + 节点自动摆放已部署（2026-08-28）。** 用户截图反馈 V1.19 两问题并补充关键线索"新技能显示的是上一个悬停的原生技能"。取证：新版 SampleF(1272)/Skill_FY(472) 导出比对锁定 tooltip 正文=狂风箭（Gale Arrow）数据 → 根因=克隆时把 SonA/SonB/SonC 清成 "0"，游戏 GetManaSample 无空判解引用 Sample_S[SonA] 抛 NRE，ShowSkilltip 在标题与正文之间中断 → 正文残留上个悬停技能。修复：①Son 链保留模板原值 ②位置改锚定格周边空闲槽位自动搜索（网格 155×170、间距≥45、面板边界内）。构建 0 error、部署 SHA `7A5ED0BC…89DDC8` 一致、42s 冒烟 LOG CLEAN。升级包 V1.20 zip 落盘。游戏内可视验收待真实机器。

**工作区：** `Game-root/`（完整版新 vanilla）+ `MODworkv2/`（decompiled 923 .cs/refs 127/backup/builds）；旧版归档 `_archive/`。**POE 融合全景调研已落盘**（2026-08-28，`docs/research/poe-fx-fusion-survey.md`）：131 条映射分级 A20/B87/C24/D0，素材库实际仅 2 个示例文件（批量抓取 18 连败于 CDN 403，修正此前"404"记录），Tier1 换色复刻模式已被 V1.18 验证；实际修改测试需用独立文件夹 `MODworkv2/fx-testbed/`（未创建）。

> 注：`ShadowDungeon` 为**新版纯净游戏**（无旧 mod），与归档的 `_archive/暗影地牢 Demo_archived`（旧版 V1.5 已部署）为两个独立游戏目录。本里程碑针对 ShadowDungeon 新版。

## 已完成里程碑

| 日期 | 里程碑 | 证据 |
|---|---|---|
| 2026-08-29 | **ShadowDungeon 新版 V1.24 铁匠工艺台**：POE metamods 工艺落地（13 项货币工艺 + 4 项工艺限制随存档，每项 1 金币，锁矩阵对齐 poedb 工艺互动表）；CraftBenchOps/CraftBenchUI 新文件 + ItemManager/WeaponClass/WeaponSaveData/ItemCloneUtil/WeaponManager 手术点；0 error→SHA256 `45DB54BE…72F7` 部署→冒烟存活+LOG CLEAN PASS；升级包 V1.24 zip 落盘 | CHANGELOG V1.24 条目 + docs/research/craftbench-metamods-design.md |
| 2026-08-28 | **ShadowDungeon 新版 V1.20**：技能参数串台修复（Son 链清 "0"→GetManaSample NRE→tooltip 半更新残留上个技能，保留模板 Son 链）+ 节点自动摆放（锚定格周边空闲槽位搜索 155×170，替代旧版 95,-95 固定偏移）；0 error→SHA256 `38327E60…F670` 部署→42s 冒烟 LOG CLEAN；升级包 V1.20 zip 落盘 | CHANGELOG V1.20 条目 |
| 2026-08-28 | **ShadowDungeon 新版 V1.19**：技能注入挂钩修正（V1.18 按钮保障误挂零调用方死方法 TalentManager.OpenClose；真入口=GameUIManager.OpenClose_Talent）+ 注入器自愈化（先补数据再补按钮/去跨局静态短路/搜索三级兜底）；0 error→SHA256 `9AD86F9A…D4C8` 部署→2min 冒烟 LOG CLEAN；升级包 V1.19 zip 落盘 | CHANGELOG V1.19 条目 |
| 2026-08-28 | **ShadowDungeon 新版 V1.18**：技能注入追加式干净重写（删替换式 PoedbReplaceInjector + 旧克隆式 972 行，新追加式 Xi 0/3/6/9 各追加 Tornado Shot/Cyclone，挂钩 5→2，PoC 验证克隆路径）+ 冰晶术黑炎特效（撤销 V1.16/17，仅换色不复用程序粒子）；0 error→SHA256 `66CCC0A0…CBEB` 部署→40s 冒烟 LOG CLEAN；升级包 V1.18 zip 落盘 | CHANGELOG V1.18 条目 |
| 2026-08-26 | **ShadowDungeon 新版 V1.7**：背包顶部排序按钮栏（稀有度/等级，全部物品）+ InventoryManager.ApplySort 复合比较器；0 error→SHA256 `88C6AC1A…689795` 部署→40s 冒烟 LOG CLEAN；升级包 V1.7 zip 落盘 | CHANGELOG V1.7 条目 |
| 2026-08-26 | **ShadowDungeon 新版 V1.6**：反编译工程 decompiled-v2（895 .cs，0 error）+ refs-v2；SkillTagSystem 技能标签移植新版；冰晶术 Ice Crystal 返回效果（命中/超时双路径）；0 error→SHA256 `2AA98341…E8E1` 部署→40s 冒烟 LOG CLEAN | CHANGELOG V1.6 条目 |
| 2026-08-25 | **V1.5 背包筛选行重做**：exp-4 摸底（格子容器/排序字段/现成管线）→ fix-5（格子下移+筛选栏入顶带+全行底板）∥ fix-6（AcquiredAt 三 SaveData+ApplySort 复合比较器+InventorySortBar 右侧按钮组）零重叠并行；0 error→SHA256 `52427E63…D516` 部署→42s 冒烟 LOG CLEAN；补丁包 V1.5 zip 落盘 | CHANGELOG V1.5 条目 |
| 2026-08-25 | **V1.4 合并终版**：读取“skill synergy”会话（CustomEquipGate/Gun环形/SK_FlyA回旋门控/ItemManager商店注入）与 HUD V1.3 合并——零文件级冲突；clean 构建 0 error、部署 `B5D16760…02FA`（与 V1.3 同哈希，已含协同改动）→42s 冒烟 LOG CLEAN | CHANGELOG V1.4 条目 |
| 2026-08-25 | **V1.3 HUD 迭代**：退回技能树双页（TalentPageFilter 删除+五处挂钩清理，零残留）；保留 SkillTagSystem 标签；背包页签迁至夹缝带（y=-H×0.29、左对齐12px、100×28+深色底板）；0 error→SHA256 `B5D16760…02FA` 部署→42s 冒烟零异常（游戏本体更新0830 新基线 vanilla 先行冒烟 PASS，再在新 refs 上 0 警告重建）；补丁包 V1.3 zip 落盘 | CHANGELOG V1.3 条目 |
| 2026-08-25 | **技能标签全表文档**：`docs/research/skill-tags-catalog.md`——12 系◆+14 个◇标签的触发字段/数值来源/隐含机制逐行对照 SkillTagSystem.cs | 该文档 |
| 2026-08-25 | **V1.2 HUD 改造**：TalentPageFilter（技能树双页）/ InventoryCategoryTabs（背包三标签视图过滤）/ SkillTagSystem（tooltip 双维标签+装备扩展钩子）三新类 + TalentManager/GameUIManager/InventoryManager 最小挂钩；0 error 构建→SHA256 部署→42s 冒烟零异常（游戏本体更新0830 新基线 vanilla 先行冒烟 PASS，再在新 refs 上 0 警告重建） | CHANGELOG V1.2 条目 |
| 2026-08-24 | **desc-sync 描述同步**：规范立规（skill-spec 描述同步要求 + AGENTS.md 红线）+ 存量同步（Skill_FY 更新 info_Razor Arrow、新增 ArcBoomerang/info_ArcBoomerang 键），staging 全自验 PASS，未部署 | `modwork/asset-inventory/desc-sync/desc-sync-report.md` |
| 2026-08-24 | **ring-arrow 测试包**：Razor Arrow（Xi=6）ARCattack case 0 环形发射 N=8 支均回旋；0 error 构建，产物暂存未部署 | `modwork/builds/ring-test/Assembly-CSharp.dll` SHA256=B738EB58…BCBF |
| 2026-08-24 | **SkillForge v1 交付**：spec 驱动加技能流水线（run/verify），测试 A/B + 4 负向用例全 PASS，游戏目录零写入 | `modwork/builds/SKILLFORGE-REPORT.md` |
| 2026-08-24 | 框架文档三件套落盘：skill-spec（v1 架构）/ effects-library（Tier1/Tier2）/ known-issues（KI-001~004） | `docs/` |
| 2026-08-24 | **V3 序列化修复真实机器验收 PASS**：SK_FlyA.ReturnToPlayer 加 [NonSerialized] 恢复 typetree 布局；ArcBoomerang 节点天赋树可见/可加点/可施放，箭矢回旋行为确认（返回点 45° 偏差记 KI-001） | 本表 + `docs/known-issues.md` |
| 2026-08-24 | 二分定界完成：V1/V2 均崩排除注入器与克隆；PRISTINE 纯净重建对照锁定序列化字段为元凶 | `modwork/builds/PRISTINE-ANALYSIS.md` |
| 2026-08-24 | 真实机器隔离测试：原版 DLL 全正常、PoC assets 正常、新 DLL 必崩 → 元凶锁定重编译 DLL | 用户实测 + Player.log |
| 2026-08-23 | fix-13 试改演练六步全 PASS：资产修改管线打通 | `modwork/asset-inventory/DRILL-REPORT.md` |
| 2026-08-23 | **Loop attempt 1 PASS**：弓系箭矢回旋镖返回效果实现并部署，启动验证零异常（游戏本体更新0830 新基线 vanilla 先行冒烟 PASS，再在新 refs 上 0 警告重建） | `.opencode/loop-history/loop-mt5x07oy-5oozp9/history-001.md` |
| 2026-08-23 | 全树代码地图完成（62 文件夹 codemap + root-scripts.md 42KB + 两级总览） | `codemap.md`、`modwork/decompiled/codemap.md` |
| 2026-08-23 | lib-1 资产工具链调研 + exp-2 代码侧事实侦察 + 可行性报告定稿 | `docs/research/` |

## 进行中

（无运行中车道）

## 下一步

1. **desc-sync 部署**：`modwork/asset-inventory/desc-sync/resources.desc-sync.assets` 覆盖游戏 resources.assets（先备份，SHA256 `10DD3349596D…`），真实机器 tooltip 验收后关闭 KI-002
2. **框架实战首用**：用户以自然语言描述任意新技能需求 → spec 确认 → SkillForge 出包（含描述同步）→ 部署验收
3. KI-001（返回点改 ARCpointA）按需处理
4. 视听扩展按 B1-B4 路线逐项推进

> 基线：Game-root 完整版（92E0120F）新 Managed 基线（127 DLL）0 error 重建（2026-09-01）
