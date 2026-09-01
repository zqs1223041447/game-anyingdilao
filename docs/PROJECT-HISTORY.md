# 项目总史（Project History）— 暗影地牢 Mod 工作区

> 目的：沉淀已完成工作、实现方法与重大踩坑，供后续装备词缀/商人系统等二期工作复用。按 AGENTS.md 文档维护职责，重大事项与踩坑点常驻于此。

## 1. 项目概览与目标演进

- **工作区性质**：Unity 2019.4.39f1 Mono 编译成品 + ILSpy 反编译研究区（`modwork/decompiled` 841 文件，可构建 netstandard2.0）。无源工程，允许资产与 DLL 级修改。
- **初始目标**：修改技能树体系，增加技能联动效果（如投射物返回）或新增技能（Loop maxAttempts=5，successCriteria=游戏可启动且不崩溃）。
- **一期演进**：单技能行为补丁 → 全树代码地图+工具链 → 新增技能节点 PoC（ArcBoomerang）→ 投射物特效装备化（当前阶段：固定商人新增自定义1/2/3装备承接特效，技能本体回滚）。

## 2. 最终架构与管线

```
用户自然语言 → 会话内解析 → spec.json (skill-spec.md v1)
       ↓
SkillForge run <spec> → 克隆模板行+列覆盖 → 自验（逐列逐行）→ staging assets
       ↓
DescSync（resources.assets Skill_FY）描述同步（强制校验）
       ↓
dotnet build → 部署（Managed/Assembly-CSharp.dll + sharedassets1/assets + resources.assets，均 SHA256 备份核验）
       ↓
本机 35s 菜单冒烟 → 真实机器 Play 场景验收（HomeScene 加载门禁）
```

- **数据层**：sharedassets1.assets 内 8 张技能 CSV（SampleF 等 path_id 1276...）+ Xi 表12系；resources.assets 内 Skill_FY JSON（path_id 433）本地化；CSV 解析器无引号转义、全列必可解析。
- **代码层**：Gun.ARCattack 等按 FStype 分支生成 SkillOBJ_DT_SP；SK_FlyA 等投射物行为组件；TalentManager.Awake 解析并建字典。
- **装备化二期**：效果将从技能硬编码迁移为“是否装备自定义物品”条件化（Gun/SK_FlyA 读已装备态）+ 固定商人新增可购买商品行承载。

## 3. 时间线大事记（详见 docs/worklog.md）

- 2026-08-23 环境基建（.NET SDK 8.0.424/ilspycmd 8.2.0.7535/.NET 6 Runtime）、反编译 840 cs、修复 csproj 中文乱码（→ modwork/refs ASCII 127 DLL）、基线 0 error 与 30s 原版冒烟
- 2026-08-23 Loop attempt 1 SK_FlyA 回旋镖返回 PASS（history-001）
- 2026-08-23 全树代码地图 62 文件夹 + root-scripts 42KB +两级总览落盘
- 2026-08-23 lib-1 资产工具链 + exp-2 代码侧事实 → 可行性报告定稿（Mono+传统布局为理想靶子，TextAsset 最安全）
- 2026-08-24 fix-12 AssetScan（UABEA v8 + AssetsTools.NET 3.0.5，7容器219 TextAsset，技能CSV path_id 全定位，源头中文GBK损坏发现）
- 2026-08-24 fix-13 试改演练6步全PASS（备份→AssetEdit 单值×10→部署→35s验证→还原，共享 typetree 陷阱前奏）
- 2026-08-24 poc-arcboomerang 端到端 PoC（Razor Arrow克隆→SampleF 72行+SkillBT克隆注册，部署双文件 40s 验证，待真机）
- 2026-08-24 fix-l1 POE三技能注入器 + InjectHarness 27/27→33/33 + SKprefab 70弹体映射核验 + 环境阻塞定界（HomeScene 预存崩溃为 VM 固有，真实机器可用 -poemod-autostart 绕过）
- 2026-08-24 二分 V1/V2/PRISTINE → 锁定 SK_FlyA 新增 public 序列化字段为元凶，V3 [NonSerialized] 修复并真机验收通过（45°偏差记 KI-001）
- 2026-08-24 框架三件套落盘 + SkillForge v1（run/verify+4负向用例全PASS）
- 2026-08-24 剃刀箭环形8箭（Gun.ARCattack case0 门控）→ 直线返回改造（StraightReturnMV）→ 描述同步（Skill_FY 772→774键）→ 本机双文件部署冒烟PASS
- 2026-08-24 全量回滚至原版三文件（SHA256 核验+35s冒烟），为装备化提供干净基线

## 4. 实现方法详解（怎么改的、怎么实现的）

### 4.1 反编译→重编译管线
- ilspycmd 8.2.0.7535 -p 导出 SDK 风格工程（netstandard2.0, LangVersion 11, AllowUnsafeBlocks）；中文路径改 ASCII 引用；自动补引 UnityEngine/AssetBundleModule/UnityWebRequestModule 后基线 0 error。
- 部署完整性：构建产物 vs Managed 目录 SHA256 比对；原版备份在 modwork/backup。

### 4.2 投射物返回（SK_FlyA）
- 初始：public bool ReturnToPlayer = true; + returning/returnSpeed 私有；StartReturn() 设 target=yao.transform, MainCOL.enabled=false, CanMove=true；TimeStop 与 OnTriggerEnter2D(!AllChuan 各分支) 调 StartReturn 而非 Stop；Update returning 分支判距离<0.6f Stop。
- 序列化陷阱：该字段插在类中部导致 typetree 偏移，原生读取 sharedassets1 越界（P0）。修复：[System.NonSerialized] public bool ReturnToPlayer = true;（运行时 true 不变，序列化布局还原）。
- 直线版：新增 StraightReturnMV() { dir=normalize(target-pos); right=dir; pos+= dir*speedTMP*(1+FlySpeed/100)*dt; }，Update returning 分支由 FollowMV/Slerp 改调此方法。

### 4.3 ArcBoomerang 新增节点
- 资产：PocCsvRow 工具裸读 m_Script（len-prefixed 裸读），克隆 Razor Arrow 行（Xi6/OBJ38 第40行），仅改 IndexName/name=ArcBoomerang, Info=info_ArcBoomerang, Price/UnLock_Point=0，追加至末非空行后，72行×152列自验。
- UI：TalentManager.Start() 注入 TryCloneArcBoomerangButton()：校验 Sample_F.ContainsKey → FindObjectsOfType 找 Xi=6/SkillType=0 模板按钮 → 克隆整格含 Text → 相对路径重接 SkillTU 引用 → 偏移(95,-95) → 改 IndexName/Xi/SkillType；靠原生 OnEnable/Start 自动 RegisterSkillBT+SetSkillBT。

### 4.4 剃刀箭环形8箭
- Gun.ARCattack case 0 主箭后：if (dt.skillName == "Razor Arrow") for i=1..7 { extra=CreatSP(); pos=ARCpointA; angle=z2+i*45°; RTtypeOBJ 0→rotation/1→dic=cos/sin; RTtypeFX 出特效; CreatACT_SK extra; } 每支带 ReturnToPlayer。

### 4.5 工具链
- AssetScan：7容器类型统计+219 TextAsset清单+表格预览导出（<1s）
- AssetEdit/DescSync/PocCsvRow/SkillForge：AssetsTools.NET 3.0.5 裸读重写（LoadClassPackage+ContentReplacerFromBuffer+file.Write），TextAsset/JSON 定点改写，全自验（往返字节、逐行逐列、SHA256、幂等）。

## 5. 重大踩坑大全

| 级别 | 现象 | 根因 | 解决 | 预防 |
|---|---|---|---|---|
| **P0** | V1/V2 均在 HomeScene 报 sharedassets1 Position out of bounds 原生崩溃，T1 原版DLL正常 | SK_FlyA 新增 public 字段插类中部，Unity 反射 typetree 布局后移，旧序列化数据读取越界 | [System.NonSerialized] 使布局还原；所有新增 public 字段必审计（Poe/Bootstrap/TalentManager 私有/常量则安全） | AGENTS.md 红线：新增字段一律 NonSerialized 或私有；PRISTINE 纯净对照验证 |
| P1 | ilspycmd 最新版报 DotnetToolSettings.xml not found | 新版要求 .NET 10，SDK 8 不兼容 | 锁 8.2.0.7535 + 装 .NET 6 Runtime | tools-index 锁版本记录 |
| P1 | csproj 中 HintPath 含中文乱码构建失败 | 非 ASCII 路径在序列化中损坏 | 统一改指 modwork/refs ASCII 副本（127 DLL）+ 自动补引漏掉模块 | 初始化即修复，不提交中文路径 |
| P2 | CSV 新行整列解析失败导致 Awake 异常 | 解析器 for(i=1;i<array.Length-1) 逐列 Parse，无引号转义，全列必可解析 | 模板整行克隆仅覆盖目标列，自验 72×152 逐列一致 | PocCsvRow/SkillForge 内建全量数值可解析断言 |
| P2 | 本VM HomeScene 必崩误导为“预存环境问题” | Hyper-V/WARP 软渲染下原生崩溃与真机测试隔离不足，T0/T1/T2 矩阵未先跑 | 真实机器 RTX 5070 隔离矩阵（原版/PoC assets/新DLL 分离）+ PRISTINE 对照 | 任何场景加载类改动后必须真机矩阵验收 |
| P2 | PowerShell -LiteralPath 不展开通配符导致 0 文件复制 | -LiteralPath 按字面量 | 批量复制改 -Path | 已知坑登记 |
| P3 | 字节搜索漏检 POE_Fireball 等（ASCII 搜 #US 堆） | 字符串字面量 UTF-16 存于 #US 堆，类型名 ASCII 在 #Strings | UTF-16LE 字节搜索 | 字符串取证用 Unicode.GetBytes |
| P3 | Xi/Baoshi 表中文显示 U+FFFD | 开发者导入期 GBK→UTF-8 有损转换，资产内已损坏 | 恢复走 Skill_FY 本地化 JSON（9 键 772→774） | 资产侧中文不可信，认 Skill_FY |
| P3 | 并行写 docs 导致 oldString not found | 多 fixer 同时改同文件，先读后改锚点失效 | 读取最新锚点后编辑；写范围声明互不重叠 | 已入 AGENTS.md 并行纪律 |
| P3 | provider network_error 车道失败 | 瞬时网络波动 | 重试同 prompt（失败会话不可复用，重开新会话） | 关键改动先本地 Read 再 Edit，降低重试成本 |
| P3 | 玩家读档触发 TalentManager 克隆二次执行 | Start 每次载档重入 | TryCloneArcBoomerangButton 首行校验 Sample_F.ContainsKey 已注册则跳过，幂等 | 克隆类逻辑必幂等 guard |

## 6. 当前部署与回滚

- 紧急回滚已执行（2026-08-24）：三文件自 modwork/backup 恢复并 SHA256 核验（Assembly-CSharp 7A78C3…, sharedassets1 CA85E2A4…, resources 589D172E…），35s 冒烟 0 异常。游戏现为纯净原版（古董存档兼容）。
- 装备化基底：modwork/decompiled-pristine-tmp（841文件）为纯净源码参考；后续装备模组将以此基底 + CustomEquip 载体实现环形/返回等效果的条件化（装备持有判定）。

## 7. 工具与资产速查

- 技能CSV：sharedassets1 path_id 1276（SampleF）/1226…/1210 Xi；本地化 Skill_FY path_id 433（resources.assets）
- SKprefab SO path_id 121575，SK_OBJ[70] 预制体映射（裸解析已验）
- 常用命令见 docs/tools-index.md；效果分级见 docs/effects-library.md（Tier1 CSV全自动/Tier2代码模式）
