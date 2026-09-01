# 代码索引 (Code Index)

> 关键代码定位信息更新时维护。行号易漂移，以方法名+文件为准。

## 入口文档

| 文档 | 内容 |
|---|---|
| [codemap.md](../codemap.md) | 工作区 Atlas（顶层结构/工作流/域聚合/调用链） |
| [modwork/decompiled/codemap.md](../modwork/decompiled/codemap.md) | 反编译树总览（62 文件夹逐个职责） |
| [modwork/decompiled/root-scripts.md](../modwork/decompiled/root-scripts.md) | 根目录 ~440 个核心脚本 12 组分类 + 5 条调用链 |

## 技能系统速查（改技能效果先看这里）

| 关注点 | 位置 | 说明 |
|---|---|---|
| 技能释放入口 | `Entity.Character.Player/PlayerActionManager.cs` → `UseSkill(int)` | 扣蓝、播动画、设 CurUseSK |
| 武器动画帧回调 | 根目录 `MGC.cs` / `SQS.cs` / `ARC.cs` / `DEAD.cs` | 分别调 Gun 的 MGCattack/SQSattack/ARCattack/DEADattack |
| 投射物生成总口 | 根目录 `Gun.cs` → `CreatSP()` | 读 ACT.skillBT[CurUseSK].actL.DT.simple，LeanPool 生成，填 SkillOBJ_DT_SP |
| 出膛点/朝向 | `Gun.cs` → `MGCattack/SQSattack/ARCattack/DEADattack` | 按 dt.FStype switch |
| 箭矢行为（已加回旋镖） | 根目录 `SK_FlyA.cs` | **含 ReturnToPlayer/StartReturn 返回效果**；SimpleMV/FollowMV/TimeStop/Stop/OnTriggerEnter2D |
| 大弹/绕点/追踪/飞剑 | `SK_FlyBall.cs` / `SK_FlyRound.cs` / `SK_FlyFollow.cs` / `SK_FlySowrd.cs`（已有 Back() 半成品）/ `SK_FlyBack.cs`（开发者预留空壳） | |
| 技能参数包 | 根目录 `ACT_skillSample.cs` | ~130 平铺字段（FStype/OBJ/FlySpeed/Speed1-4/Count_F/S…） |
| 技能包装 | 根目录 `ACT_skillData.cs` | Opend/type/SampleSkill/IndexName/ManaCost/simple/comp |
| 天赋树核心 | 根目录 `TalentManager.cs`（3873 行） | CSV 加载 LoadData_Xi/LoadData_SampleF/LoadData_CompF/LoadData_DotF/LoadData_Bei；加点 AddPoint→AddPointXxx；60+ GetXXX 访问器 |
| 数值联动模式 | `SkillData_Sample_Father.cs` 等 `_Last` 属性 | `XXX_Last => XXX_Base + TalentManager.GetXXX(xi,name)` |
| 树→施法桥接 | `ACTbar.cs` → `SetSkill_Sample` | Father 数据拷进 ACT_skillSample |
| 技能 UI | `SkillBT.cs`（加点）/ `SkillXiBT.cs`（分支）/ `ACTListSkillBT.cs`（快捷栏） | |
| 同伴技能数据 | `Data.RuntimeData.Skills.CompSkill/CompanionRuntimeData.cs` | ~70 字段快照，Gun.SetCPData 填充 |

## 五条关键调用链

1. 施法主链：`TryUseSkillDown → UseSkill → PlayerSP/CP → Gun.CreatSP → SK_FlyA.SetStart`
2. 召唤链：`CreatCP → CompanionRuntimeData → SK_FSQ_comp.Init`
3. 天赋链：`AddPoint → AddPointXxx → SetXiBuff`
4. 武器 SPC 链：`AddWP_SPC → SK/HIT/DIE/HURT 字典 → ACTprefabFS/TakeBoomDie`
5. DOT 链：`SetDot → DOT_MG.AddDot → TakeBoomDie`

## 已落地修改记录

| 修改 | 文件 | 内容 | 状态 |
|---|---|---|---|
| 背包格子下移+整行筛选器+排序 | `InventoryCategoryTabs.cs` + `InventorySortBar.cs`(新) + `InventoryManager.cs` + `Data.SaveData/{Weapon,Baoshi,UseItem}SaveData.cs` | Gird 首次可见幂等下移一格（逻辑层零改动）；筛选栏挂 IVgird 锚定顶行(+slotSize×0.5)、底板拉通全行；右侧三排序按钮（稀有度/获取时间/等级，同键翻转升降序）走 ApplySort(mode) 复合比较器（主键→Price 回退→序号兜底）；AcquiredAt 存档字段+ConditionalWeakTable 运行时注册表+全入包路径打点；刷新经 NotifyPageChanged 解耦 | V1.5 已部署，菜单态冒烟 PASS |
| 技能协同装备化（Ring/Return 门控+商店注入） | `CustomEquipGate.cs`(新) + `Gun.cs`(4处) + `SK_FlyA.cs:535` + `ItemManager.cs` | Custom1=Ring 环形分散门控(RingCursor 137.5°步进)、Custom2=Return 箭矢回旋门控；商店固定位注入 GlobalID 90001-90003（Epic 品级）——来自“skill synergy”会话，V1.4 合并登记 | V1.4 已部署，菜单态冒烟 PASS |
| POE 测试装备包（5 件商人 0 元商品） | `PoeItemMod.cs`(新) + `Gun.cs`(5处) + `SK_Angle_F.cs` + `SK_FlyA.cs` + `ACTbar.cs`(2处) + `ItemManager.cs`(2处) + `ShopManager.cs`(CreatBS新) + `UseItemClass.cs` + `BuffPotionItem.cs` + `BuffManager.cs` + `InventoryManager.cs`(4处) + `WarehouseManager.cs` + `WeaponBaoshiApplyUtil.cs` + `WeaponClass.cs` + `PlayerManager.cs`(BS_ExtraProjectiles) + `BaoshiClass.cs` + `SaveDataEquipmentSanitizer.cs` | 91001-91005：疾风之瓶/洞悉之瓶（InfoType1 自定义 UseType=poe_flask_*：AddPotionBuff 增益+AddSimpleDrink 冷却+IsRepeatableFlask 三处不消耗门控）、星环之戒（四出手点黄金角 137.5° 旋转 + SK_Angle_F Type0/1/2→SpawnEvenRing 360°全环）、回响之链（SK_FlyA ReturnToPlayer 追加 ReturnEquipped，复用冰晶术返回）、万箭之玉（BStype "projectile"→SocketType 26→PlayerManager.BS_ExtraProjectiles；ACTbar Count_F+Extra 与 Gun 单体补射互斥防双倍；Sanitizer GemFloatFields+26 存档安全）；Item_MB 行运行时合成（DropLevelStart=999 不进掉落池），CreatShop 头部固定上架 1 号买页前 5 格 Price=0，Item_FY LOC 反射注入 | V1.23 已部署；⚠️ 其 PlayerManager 公开字段致 typetree 崩溃，V1.25 修复（[NonSerialized]）；功能并入 V1.25 合并版 |
| 技能树主技能/增幅双页切换 | ~~TalentPageFilter.cs~~ + TalentManager/GameUIManager 挂钩 | （V1.2 实现）静态过滤器按 SKI type 分类+克隆页签 | **V1.3 已按用户要求整体移除**，天赋树恢复原版布局，零残留 |
| 背包药水/镶嵌物/通用三页签 | `InventoryCategoryTabs.cs` + `InventoryManager.cs`（Update Tick + ChangePage override 两处挂钩） | 视图过滤式：非匹配物品 alpha×0.25+禁射线（原色精确保存还原），同签再点=全部；分类=InfoType0/1·Baoshi·其余；翻页/开关面板/0.5s 节流重施；WarehouseManager 零变化。位置演进：V1.3 夹缝带 → **V1.5 顶行整排**（挂 IVgird，格子下移腾行） | V1.5 已部署，菜单态冒烟 PASS |
| 技能 tooltip 双维标签实时同步 | `SkillTagSystem.cs` + `GameUIManager.cs`（ShowSkilltip/RefreshSkilltip/HideSkillTip 尾部各一行） | 元素系◆蓝（12 系 GetSkill 优先/常量回退）+ 形态◇橙规则表（直射/位移/环绕/落点/穿透/命中爆炸/末段爆裂/多弹/追踪/减速/DOT 词/回旋白名单{Razor Arrow, ArcBoomerang}）；每次 Show/Refresh 实时重算（_Last 含天赋加成）；RegisterTagContributor 为装备化二期预留；异常全降级。全表说明见 `docs/research/skill-tags-catalog.md` | V1.3 已部署，菜单态冒烟 PASS |
| 回旋镖返回效果 | `SK_FlyA.cs` | ReturnToPlayer=true 默认开启；StartReturn() 关碰撞清命中列表锁玩家目标；TimeStop/命中双路径触发返回；Update 距离<0.6f Stop 回收 | 部署验证 PASS |
| POE 投射物技能注入器 | `PoeSkillInjector.cs` + `BootstrapEntry.cs`(1 行挂钩) | 运行时克隆 Sample_F donor 行注册 3 个技能（POE_Fireball/IceSpear/LightningArrow）+ 合成 fire DOT 并装填快捷栏；每秒轮询幂等；`-poemod-autostart` 参数可自动开局验证 | 构建/部署/菜单态运行 PASS；游戏内验证待可用环境（本 VM HomeScene 原生崩溃为预存问题） |
| ArcBoomerang 天赋节点 PoC | `TalentManager.cs`（Start 首行注入 `TryCloneArcBoomerangButton()` + `RelativePathUnder()` 辅助） | 资产侧 SampleF 第 72 行（克隆 Razor Arrow/Xi6/OBJ38，IndexName=ArcBoomerang，Price=UnLock_Point=0）；代码侧 Start 时找 Xi=6/SkillType=0 模板按钮克隆整格、相对路径重接 text/SkillTU、偏移 (95,-95)、改 IndexName/Xi/SkillType 后靠原生 OnEnable/Start 完成 RegisterSkillBT+SetSkillBT；全程 try/catch 防御 | 构建/部署/启动验证 PASS，部署保持中；游戏内验收步骤见 `modwork/asset-inventory/POC-REPORT.md` |
| POEDB 追加式技能注入器（Tornado Shot/Cyclone） | `PoedbMod/PoedbSkillInjector.cs`(V2.2) + `TalentManager.cs`（Start 首行 `TryInjectData`）+ `GameUIManager.cs`（`OpenClose_Talent` 打开分支 `TryEnsureButtons`，自愈先补数据再补按钮） | 追加不替换：Xi 0/3/6/9 各克隆"该页首个主动技能"→Tornado Shot（保留直射弹道，AllChuan_F=0 穿透，100%+30%/Lv/蓝8/CD1.2s）+ 克隆"该页首个 FStype 7/8/9 环绕技能"（游侠=Power of the Wind）→Cyclone（保留环绕，80%+20%/Lv/蓝10/CD5s）；**SonA/SonB/SonC 必须保留模板原值**（GetManaSample/GetCD_Sample/GetBuffTime 无空判解引用 Sample_S[SonA]/[SonB]，仅 SonC 允许 "0"；清空→ManaCost_Last NRE→tooltip 标题更新后正文残留上个悬停技能，V1.19 实测）；数据入 Sample_F/SKI/FW，原技能零改动；按钮整格克隆+相对路径重接+原生自动注册；**节点摆放=锚定格周边空闲槽位搜索**（网格步长 155×170/间距≥45/面板边界内，常量文件头）；LOC fallback 标题+info_ 键与列值一致；图标继承模板，data/poedb/icons/ 可选覆盖；无跨局静态短路。**面板真入口=GameUIManager.OpenClose_Talent（TalentManager.OpenClose 为零调用方死方法）**。新版资产 path_id：SampleF=sharedassets1:1272、Skill_FY=resources:472（导出工具 PocCsvRow 已改）；新版 CSV 无原生 Tornado/Cyclone 行 | V1.20 已部署（SHA `38327E60…F670`），42s 冒烟 LOG CLEAN；游戏内可视验收待真机 |
| 铁匠工艺台（POE metamods 工艺） | `PoedbMod/CraftBenchOps.cs`(新·工艺逻辑) + `PoedbMod/CraftBenchUI.cs`(新·运行时 uGUI) + `UI.Panels/WeaponManager.cs`(OnSingletonAwake 尾 `CraftBenchUI.Install(this)` + GetCloseBtn/GetForgeAudioEvent) + `ItemManager.cs`（WeaponDropContext/WeaponStatGroup 转 public、SetWPdata 清工艺锁、尾部 Craft* 桥接 region 11 方法）+ `WeaponClass.cs`（Craft_LockPrefix/Suffix/NoAttack/NoCaster 4 字段）+ `Data.SaveData/WeaponSaveData.cs`（持久化）+ `ItemCloneUtil.cs`（拷贝） | 13 工艺（蜕变/增幅/改造/富豪/点金/混沌/隐匿混沌/崇高/无效/神圣/重铸/兽猎移前增后·移后增前）+ 4 工艺限制切换，全部 1 金币（CraftBenchOps.CraftPrice）；品质=原生 0-6 档、锁矩阵对齐 poedb 工艺互动表；词缀池=Item_MB RateMain/RateDot/RateSK/RateCP/SPC + GenerateWeaponStatValue 原生公式（CraftPickPoolTemplate 按品质档选同族模板，找不到回退本装备模板）；词条分组=前缀(主属性/持续/技能/武器元素) 后缀(同伴/抗性/SPC)、攻击(主/技/武元) 法术(持续/同伴)；词缀上限 V1.26 品质档阶梯=普通0/魔法4/稀有6/精致7/史诗8/传说9/神话10（对齐原生掉落曲线）；V1.27 元素拆分行计数改 1 条、点金分级 ExecAlchemy(目标品质)（点金石=稀有/精致/史诗+传说石/神话石，普通起 4~上限条）；UI=V1.26 起点击选装（EventSystem Raycast 排除面板）、V1.27 分区标题行+配色分层修标题遮挡、打开时退出锻造模态+ToggleInteract(false) 防点击穿透、执行链 RemoveMoney→BindWeaponToRegion→ShowWPTipA→锻造音效；重铸=保锁组清其余+品质回普通+清全部工艺限制 | V1.24→V1.25（修字段序列化）→V1.26 适配版已部署（SHA `69CAE7CB…1C8`）；设计记录 docs/research/craftbench-metamods-design.md |
| 商店 5 件固定商品自愈 | `PoeItemMod.cs`（StageShopItems 挪 CreatShop 尾部+IsStaged 查重+VerifyShopStock 补架+逐步日志）+ `ItemManager.cs`（CreatShop 尾部挂钩）+ `ShopManager.cs`（SortBuy 尾部挂钩） | 真机首测 5 件只出 3 件（缺星环之戒/回响之链两条武器路径件）；与原版 CreatWeapon 上架路径逐行同构、静态分析无法复现→自愈兜底：查重+重排后校验补架+`[PoeItemMod]` 全链路日志进 Player.log | V1.26 自愈未根治 → **V1.27 根修已部署**（SHA `B82B5E91…1C9`）：真机日志实锤 `WP row missing 91003/91004`——BuildAccessoryRow 的 PLtype=4 使 AddRow 守卫 `PLtype<GP.Length(4)` 恒假，模板行从未注册；已删守卫+PLtype=0，查重/补架/日志保留作保险 |
| V1.28-30 装备描述 + 星环环状发射 + 回响之链穿透/双程命中 | `PoeItemMod.cs`（Descriptions+TryGetDescription；SpecialModColor="#00E5FF"；RingBonusProjectiles=4；IsEquipped 加 GlobalID 91003/91004 兜底；SpawnExtraProjectiles 数量层合并均分环 + cast/equip-dump 诊断日志）+ `WeaponClass/UseItemClass/BaoshiClass.cs`（GetMain 首行描述）+ `Gun.cs`（删 4 处黄金角）+ `SK_Angle_F.cs`（SpawnEvenRing(count) 参数化 + type 0-4 全部 ringMode→Count+4 全环）+ `SK_FlyBall/SK_FlyFollow/SK_FlyA.cs`（返回 + V1.30 穿透/返程命中） | 星环=Sire of Shards（+4+环状发射）。回响 V1.30：**+1 穿透优先于返回**（SK_FlyA `ChainPierceOrReturn` 三处命中点、SK_FlyBall Stop 门控 pierceLeft 分支；pierceLeft 回响=1）+ **去程/返程双命中**（StartReturn 保留 MainCOL/canDAM，OnTriggerEnter 顶部 `if (returning) { ReturnHit(); return; }` 只 EM_Set 结算，em 去重同目标去返各一次）。优先级对齐 poedb：穿透>分裂>连锁>返回（docs/research/poedb-projectile-mechanics.md） | V1.30 已部署（SHA `80E42E0E…E65`）；42s 冒烟 LOG CLEAN；回响待真机；星环根因待 `[PoeItemMod]` cast/equip-dump 日志 |
| V1.31/32 词条档位显示（T几 | [可roll范围]，双回退全标注） | `PoedbMod/AffixTierDisplay.cs`(新·静态显示层) + `WeaponClass.cs`（四处循环行尾注入：`AppendMainArrayLines`/`GetDot`/`GetSK`/`GetCP`） | 池反查=`ItemManager.CraftFindTemplate`（GlobalID 匹配+静态缓存）→`Item_MB.RateMain/RateDot/RateSK/RateCP` 同 Index（B 组同 SkillName\|Index，同值去重）档位 NB 降序；可达区间=逐档复刻 `GenerateWeaponStatValue`/`GetWeaponStatRandomMultiplier`/`ApplyWeaponIntegerGrowth`/`ApplyMijingExtraIntegerGrowth`（分类表/成长上限/乘数区间逐字对照 ItemManager 私有谓词，**改原生公式需同步 AffixTierDisplay**）并按物品自身 Level/Quality/DropScene 重建；值落区间定名次（多命中取中心最近，全脱靶兜底最近档）；多档 T=排名、单档浮动按百分位 T1~T5、秒回单档 T1、超上限 `T1+`；后缀 ` T? | [lo-hi]`（用户定稿 `T1 | [9-26]`，多档 T 即名次；T1 金 `#FFD24A`/其余灰 `#8F8F8F`，常量文件头）；Fixed 单档/无池不标注；套装共鸣行（GetMainArrayLine/GetDotArrayLine 合成调用点）不受影响；秒回 L≥100 秘境 GivePRC_Base 曲线乘数按 1 近似（最近档兜底）。V1.32 双回退：池内无该 Index→**全局档位梯**（WP_Main/WP_DOT/WP_SK/WP_CP 全池行该 Index NB 去重，模板固定词条也标注）；值落不进任何档→**品质档家族池并集**（同名基底全职业×全品质模板池，治点金升品质后词缀属低档池错位）。星环技能路径=Gun 四攻击函数 switch 后总出口（V1.31 重建版 `2216612A…`，并行会话修，FStype 全覆盖+组件守卫）；ACTbar Count_F 通道已试验并撤销（SK_Fly 族不读 Count_F）。**BuffTime>0 死门已移除**（V1.23 遗留，技能样例自带存活计时全被拦，四族与 CreatSP 均不消费该字段=星环历次「无效果」终极根因，遥测实锤）；克隆弹经 gun.CreatSP 按当前技能样例全量构建 + TargetPos 环向修正（防寻的弹收拢）；星环/回响 tooltip 诊断行（TryGetEquipDiagnostics 挂 GetMain，IsEquipped 已去 hasWeapon）：`⚠ 未穿戴（在背包中）`/`✓ 已穿戴生效` + **「上次出手」遥测**（LastCastInfo，截图替代日志回传） | V1.32 四轮已部署（SHA `168433A6…954B`，=V1.30+V1.31+V1.32 合并体）；42s 冒烟 LOG CLEAN；设计记录 docs/research/affix-tier-display-design.md |

### PoeSkillInjector 关键锚点（2026-08-24）

| 关注点 | 位置 | 说明 |
|---|---|---|
| 注入器本体 | 根目录 `PoeSkillInjector.cs` | Bootstrap(公开,由 BootstrapEntry 调用)→InjectorRunner 每秒 Tick→TryAutoStart/InjectSampleSkill/InjectFireDot/TryBindHotbar |
| 注册范式 | TalentManager.LoadData_SampleF 尾部(:1116-1120 同构) | `XiData[xi].Sample_F.Add(name,data)` + `SKI.Add(name,new SKindex{Xi=xi,type=0})`；DOT 行 type=4 |
| 装填范式 | `ACTbar.AddSkillListSlotSP`→`CheckListSkill`→`SetSkill(xi,0,listBT,icon)`（OpendSkillBT=空闲槽下标） | Level_Base>0 为门槛 |
| donor 选择 | FireBall(Xi0/OBJ0)/Ice Crystal(Xi1/OBJ6)/Lightning Ball(Xi2/OBJ12) | SK_OBJ[0]="01 1 Fire Ball"[SkillOBJ_DT_SP+SK_FSQ_fatherA]、[6]="Angle (F)"[+SK_Angle_F]、[12]="03 1 TD Ball"[+SK_FSQ_fatherA]（AssetsTools 裸解析 SKprefab path_id=121575 实测） |
| 字段语义速记 | colEXP==0→接触爆炸(EXP AoE,无主伤害)；AllChuan_F==0→全穿；Follow_F==0→追踪；LastEXP==0→寿命末 EXP；MainEL 选 FX_shan/Angle 组内元素变体与音效下标 | 见 SK_FlyBall.SetStart / SK_Angle_F.FaShe |
| 减速链路 | People.EM_Set:256-283 → Buff_Enemy(type=0) → BuffMG_EM.AddBuff | MoveSpeedCut>0 且 DebuffTime>0 即生效，AttackType=true 必上 |
| 点燃链路 | ACTbar.SetDot(dt) 写 skillDOT[fire] → People.EM_Set:212 双随机(DotMulti×DOTrate) → DotEM.AddDot | 合成 Dot 行 SonA-D 必须指向 Xi.Dot_S 合法键（GetXXX 访问器无 null 保护） |
| 环境阻塞 | 本 VM HomeScene 加载 sharedassets1.assets 原生崩溃（UnityPlayer 0x80000003） | 最小无注入构建复现=预存问题；真实机器加 `-poemod-autostart` 可全自动验证 |
| 逻辑级验证工具 | `modwork/tools/InjectHarness/`（net8 控制台，引用构建产物） | 真实 CSV donor → 真实 CloneSample/Build* → 字典注册 → SetSkill_Sample 数据包复刻断言 → 合成 fire DOT + SetDot 复刻断言 → 装填绑定断言；27/27 通过。引擎边界（SingletonMonoScope.Instance/LeanPool UI 槽）已如实标注 |
| side-car 取证 | 注入器 `SideLog()` → `%USERPROFILE%\AppData\LocalLow\OO Cat\Shadow Dungeon\poemod-injection.log` | 每次 bootstrap/REGISTER/EQUIP 追加一行，try/catch 包裹不影响主流程 |

## 技能数据资产定位（exp-2 实证，2026-08-23）

| 事项 | 结论 |
|---|---|
| CSV 挂载方式 | `TalentManager`（ScopedSingletonMono）public 序列化字段 XiTA/skillTA[7]，Awake 时 LoadData_Xi→LoadData_Bei 依次解析 |
| CSV 所在容器 | **sharedassets1.assets**（8 个全部内嵌于此；本地化 _FY JSON 才在 resources.assets） |
| 解析器特征 | `LoadTextFile` 仅按 `\n`/`,` 切分，不支持引号转义；逐列 Parse，**新行必须列数齐全且数值可解析** |
| SampleF 关键列 | IndexName(唯一键)/icon(Sprite 下标)/Price/UnLock_Point/Xi/Level_Max/SonA-C/UseAni(0-4)/FStype/OBJ/RTtypeFX/ManaCost_Base…共 ~150 列 |
| 子节点解锁 | SampleS/DotS/CompS 的 FrontSkill/FrontSkillType/FatherSkill；SetSkillBT 校验父 Level>0 |
| SKPB | `SKprefab : ScriptableObject`（GameDataManager.cs:35），序列化数组 SK_OBJ/SK_FX/Skill[].OBJ 等，运行时只读，越界即异常；实例在 sharedassets1.assets |
| 图标 | `IconData : ScriptableObject { Sprite[] icon }`，TalentManager.iconDT[12 组]；SkillData.icon 为 Sprite 直接引用；复用旧索引零成本 |
| 名称显示 | LocalizationManager.GetSkill(IndexName) 读 resources.assets 的 Skill_FY JSON，缺键回退原文并 Warn |
| 音效引用 | 以 event:/ 路径字符串为主（AudioData SO / 预制体 SoundA[]），EventReference 强类型仅 SKPB.SoundRain；bank 在 StreamingAssets/Desktop（含 Skill.bank 71,569KB） |
| 动画 | 全线 Spine 无 Animator；玩家 UseAni 是 MGC/SQS/ARC/DEAD.ACT(int) 硬编码 switch 0-4（>4 落空）；敌人侧 UseAni 是 string[] attack 下标 |
| 存档兼容 | FlushDatas/RestoreDatas 按 IndexName 键控字典，新键自动纳入不破坏旧档 |
| UI 按钮 | SkillBT 是场景序列化对象，运行时从不实例化 → 新节点需 Clone 现有按钮改 IndexName 后 RegisterSkillBT |

详细论证见 [research/feasibility-skill-expansion.md](research/feasibility-skill-expansion.md)。

> 基线：游戏本体更新0830（92E0120F）新 Managed 基线 0 警告重建
