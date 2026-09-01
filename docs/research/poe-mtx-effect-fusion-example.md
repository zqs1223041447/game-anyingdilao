# POE 商城技能特效融合可行性 — 以「燃烧箭矢：秘法」为例

> 任务来源：用户指定特效页 https://poedb.tw/cn/Mystic_Burning_Arrow_Effect  
> 本地对照：`game-systems-ref/05-skills.md` 烈焰箭 **Flame Arrow (Xi7 OBJ44)**  `2 SP CD0 直射投射物·命中AoE·末段AoE`  
> 图标素材：`game-systems-ref/poe-assets/MysticBurningArrowEffect.webp` (6.2KB) + `MysticBurningArrowEffect_video_hq.jpg` (30KB) + 全量映射 `manifest.json` (131 技能)  
> 技术底座：Unity 2019.4.39f1 / Mono / `MODworkv2/decompiled` 可重编译（AGENTS.md 工作流）  
> 分析方法：本地代码只读审计（`ora-1`）+ POE 条款与素材链路联网核验（`lib-1` + 编排层补查）

---

## 1. 结论先行（给决策人）

| 问题 | 判定 | 一句话原因 |
|---|---|---|
| **直接搬运 POE 商城特效素材（贴图/模型/Shader/Prefab 直拷）** | **不可行** | 版权禁止 + 引擎管线不兼容 + 资产不在 DLL 工作流内 |
| **参考“秘法蓝紫能量箭”视觉，本地自制复刻** | **可行，推荐 A+B2** | 完全可在代码层落地，2 天可出可回滚，效果可达原版 80% |

> **推荐路径 A+B2**：A 换色（`SK_FlyA` 程序化调色+Trail）当天验证链路；B2 代码动态挂星尘粒子叠加还原“离散荧光尘”。资产不动，单文件改动，独立上线。

---

## 2. POE 侧事实（poedb 页面 + 展示视频）

- **页面**：`poedb.tw/cn/Mystic_Burning_Arrow_Effect` — 标题“燃烧箭矢：秘法”，Category `技能特效` / Type `燃烧箭矢外观` / Metadata `Metadata/Items/MicrotransactionSkillEffects/MicrotransactionMysticBurningArrowEffect`
- **附加目标**：`燃烧箭矢 / 瓦尔：燃烧箭矢`（Burning Arrow / Vaal Burning Arrow），技能效果同时只能用于一个宝石，可移除后换用
- **价格**：国服 7800 点券（历史最低 4680，见页面 8 条促销记录）；外服 US $130
- **视觉研判**（图标 + 视频 `a-mMw9ESjnM` 缩略图）：奥术青蓝系能量箭体 + 头部强发光点 + 沿弹道离散星点/荧光尘 + 细长 Additive 尾迹；非橙红写实火焰，半透明能量体
- **素材形态**：POE 自研引擎 3D PBR + 专用 Shader + 特效图集，视频展示为引擎内录制；**poedb 仅提供图标 webp 与 YouTube 预览，不提供源资产下载**（图标 CDN `https://cdn.poedb.tw/image/Art/2DItems/Effects/MysticBurningArrowEffect.webp` 已本地化）

---

## 3. 本地侧事实（Unity 2019.4 反编译工程审计，`ora-1`）

### 3.1 施法与特效挂载链路

```
Gun.CreatSP():142 → LeanPool.Spawn(GetSkillPrefab(sp)) → SkillOBJ_DT_SP
GetSkillPrefab = SKPB.SK_Group[sp.OBJ_Group].OBJ[sp.OBJ]   // 05-skills.md 的 OBJ 双下标
SKPB 来源：GameDataManager.cs:35 SKprefab (CreateAssetMenu ScriptableObject，sharedassets1.assets 序列化)
```

- **本地对照**：烈焰箭 = Xi7 `末日信徒` / OBJ44，`Gun.CreatSP` + `SK_FlyA.cs` 飞行体
- **参数包**：`SkillOBJ_DT_SP.cs` 70+ 字段快照（Damage/FlySpeed/FX/Trail 等），`Dicform.cs:13` 挂指针

### 3.2 投射物体形态

- **烈焰箭属 `SK_FlyA`**：`SpriteRenderer Arrow:9` + `TrailRenderer[] trail:11` + `float[] trTime:13` + `GameObject[] par:15` (子物体粒子开关) + `FX:39 / EXP:41` (命中/末段特效)。飞行 `SimpleMV()/FollowMV()`，`SetStart():315` 统一激活
- **家族一致**：`SK_FlyBall:18 parLoop:ParticleSystem[]`, `SK_DZ:8 parLoop` 等均证实 Shuriken 粒子直接挂 Prefab 子节点，代码仅 `Play/Stop`
- **Spine 边界**：Spine 仅用于角色/Boss `SQS/ARC/MGC/DEAD.cs`，投射物全线不用 `SkeletonAnimation`
- **资产固化**：`SKprefab` 数组只读序列化，运行时越界即崩；`AGENTS.md` 红线：仅允许覆盖 `Assembly-CSharp.dll`，`ShadowDungeon/*` 其他资产禁止改

> **一句话**：换肤 = 换 Prefab 内 Sprite/Trail 材质/Particle 预设，或代码层改 `trail[].time/color` / `Arrow.color/material`。

### 3.3 引擎能力

- `TrailRenderer 163 个`、`ParticleSystem 50+ 技能` 遍布，Built-in 2D 管线，Shuriken + SpriteRenderer 为主路径，2019.4 Mono 完全支持
- 无 URP/Shader硬限制，A+B2 完全在引擎能力内；性能注意：烈焰箭 `2 SP CD0` 高频，星尘需控 `maxParticles 20–30 / emission 15`

---

## 4. 法律与素材链路（联网核验）

### GGG《Terms of Use》关键条款

- **7(b)** 禁止 Modify or adapt the game client or its data（通过第三方工具亦禁止）
- **7(f)** 禁止 Use any data gathering and extraction tools to extract information from the Website
- **开发者文档**（https://www.pathofexile.com/developer/docs）“We cannot allow our Intellectual Property to be used to generate commercial revenue. This includes ... copying our assets to make other games.”；“Executable apps that interact with the game or game files ... strictly against our Terms of Use (7b,7c,7i)”
- **成熟判例**：社区 `VisualGGPK / PyPoE / BundlesExtractor` 可解包 `Content.ggpk`，但提取的 `.art / .ao / .bundle` 专有格式无 Unity 兼容，且分发即违约

> 研判：**可“看”不可“搬”**。poedb 的 CDN 图标与 YouTube 预览可作参考；`Content.ggpk` 内的模型/贴图/Shader 即使技术上可提，也不得复制进商业或自发布游戏。

### 素材可获取性

- **图标/预览图**：poedb CDN 公开，允许下载作研究（已本地化 `MysticBurningArrowEffect.webp` + 视频缩略图），格斯 2DItems/Effects 命名
- **源特效资产**：POE 私有 PBR 材质 + 粒子图集 + 定制 Shader，与本作 `Sprites/Default Additive + Shuriken` 不兼容；即使提取也需重制贴图与材质

---

## 5. 复刻方案（按推荐度排序，`ora-1` 原案整理）

### 方案 A：纯代码层换色 + Trail 调参（首选，成本小、风险低）

- **做法**：`SK_FlyA.SetStart():315` 分支 `if (sp.skillName=="Flame Arrow") ApplyMysticStyle()`：`Arrow.color = #7AF0FF`、`Arrow.material.SetColor("_Color", arcaneBlue)`、`trail[].startColor/endColor = 蓝紫渐变`、`trail[].time=0.25`、`par[]` 粒子 `startColor` tint
- **改动面**：`SK_FlyA.cs` 单文件（+ 可选白名单 Registry），约 20 行
- **效果上限**：蓝紫版烈焰箭，无离散星尘，但最稳、0 资产、0 风险

### 方案 B：新增星尘尾迹粒子（中成本，效果最接近原版）

- **B2 代码动态挂载（推荐）**：`SetStart` 时 `Instantiate(newParticlePrefab)` 到箭体下（`Resources/MysticDust.prefab` 或代码创建 Shuriken：`Emission burst + Velocity over Lifetime + Color over Lifetime 青蓝→透明 / Material Additive`），`Stop()` 时回收（复用 `SK_Strom.cs:99 / LeanPool` 范式）
- **B1 资产层克隆 Prefab**（不推荐）：改 `sharedassets1.assets` 内 `SK_Group[?].OBJ[44]`，需突破 DLL-only 红线并重打包
- **成本/风险**：B2 中 / 需压测齐射（Barrage 15 发）GC/DrawCall；B1 大 / 触资产管线
- **价值**：唯一能还原“离散荧光尘”

### 方案 C：Shader 变体 / Bloom

- 换 `Sprites/Default → Additive` 或加 Bloom 后处理强化蓝通道；本作 Built-in 2D 无 URP，需验证 `Shader.Find`，影响全局批次，成本中、风险中大

### 方案 D：Spine 换皮

- 抛弃 Sprite+Trail 全体系，成本大、批次差，不推荐

> **排序 A > B2 > B1 > C > D**。实战 A+B2 组合：先 A 验链路，再 B2 叠加，两步独立上线可回滚。

### 落地步骤（A+B2）

1. `SK_FlyA.cs:SetStart()` 加 `ApplyMysticStyle()` 分支
2. 新建 `MysticArrowDustFactory` 缓存粒子预制，动态挂载，随 `Despawn` 回收
3. 命中 `FX/EXP` 同步 tint（`OnTriggerEnter2D:719`）
4. `dotnet build -c Release` 0 error → 覆盖 `ShadowDungeon/.../Assembly-CSharp.dll` → 35s 存活 + `Player.log` 0 Exception → `CHANGELOG.md` 登记 + 打 `MOD-Vx.x.zip`

---

## 6. 能否“融合”到本地 — 分层回答

| 融合层级 | 可否 | 说明 |
|---|---|---|
| **素材直拷**（POE 贴图/模型/Shader 复制进本作） | 否 | 版权禁止 + 格式不兼容 + 资产不在工作流 |
| **视觉参考复刻**（自制青蓝能量箭 + Additive 尾迹 + 星尘） | 是 | A+B2 完全可行，效果 80% 还原，合法合规 |
| **批量化**（70 主技能 + 13 同伴 逐一映射 POE MTX） | 部分 | 有直接对应（如 Burning Arrow/Poison Arrow/Rain of Arrows/Tornado Shot 雏形）的可一对一复刻；无对应的可用 POE 同系 MTX 风格参考（见 `manifest.json` poeCounterpart 列） |

---

## 7. 批量扩展：所有主题图标拉取

- **已落实**：示例图标 `poe-assets/MysticBurningArrowEffect.webp` + 视频缩略图 已下载；`poe-assets/manifest.json` 已生成 131 技能全量映射（localCN/EN ↔ poeCounterpart ↔ poePage ↔ poeIconGuess）
- **拉取脚本**：`poe-assets/fetch-icons.ps1`（跳过已存在/重试 3 次/记录 SHA256/限速 150ms）。首轮 131 条 guess 中 18 条因 CDN 命名差异 404，属预期（POE 并非每技能都有独立 MTX 名）；脚本已记录 `fetch.log` 与 `fetch-report.json`，第二次运行自动校验 `poeIconGuess` 是否存在，不存在则标记 `failed-guess-404` 供人工补链
- **其余 4 主题**：`manifest.json: otherThemes` 已说明  
  - 装备词缀 230+ / 怪物词缀 20 均为文本词条，POE 侧 `Modifiers` 同为文本，无独立图标（设计如此）  
  - 宝石/镶嵌 137 项对应 `poedb.tw/cn/Gem`，图标位于 `cdn.poedb.tw/image/Art/2DArt/SkillIcons/`，按需可用同一脚本拉取  
  - 锻造对应 `Crafting_Bench`，为工作台 UI，无单体图标

> **可用性**：`pwsh -File fetch-icons.ps1` 即拉；已下载的示例图标与缩略图 SHA256 见 `manifest.json: exampleDownloaded`。

---

## 8. 风险与合规提示

- 复刻仅作“灵感参考、配色与尾迹形态自制”，素材 `MysticBurningArrowEffect.webp` **禁止随包分发**，文案写“灵感来自 POE Mystic 风格”即可
- 如文案提“奥术蓝焰”需同步 `resources.assets path_id=433 Skill_FY` 本地化键（AGENTS.md 红线：SampleF CSV 与 Skill_FY 必须同步）
- 资产改动如走 B1，需同步升级包资产增量并记录 SHA256，避免“行为改、资源旧”不一致

---

## 9. 参考资料

- POE 页面：https://poedb.tw/cn/Mystic_Burning_Arrow_Effect / https://poe.game.qq.com/shop/item/MysticBurningArrowEffect / 视频 https://www.youtube.com/watch?v=a-mMw9ESjnM
- 本地技能：`game-systems-ref/05-skills.md` Flame Arrow 行；`game-systems-ref/README.md` 入口
- 本地代码：`MODworkv2/decompiled/SK_FlyA.cs`, `Gun.cs:142`, `SkillOBJ_DT_SP.cs`, `GameDataManager.cs:35`, `SKprefab.cs`
- 法律：https://www.pathofexile.com/legal/terms-of-use-and-privacy-policy（ToU 7b/7f）/ https://www.pathofexile.com/developer/docs（IP 条款）/ https://store.steampowered.com/eula/238960_eula_1

> **一句话给决策**：不要拷 POE 资源，用 **A(换色)+B2(代码挂尘)** 2 天可出可回滚的版本，最小风险拿到“秘法蓝紫燃烧箭”的识别度；批量可按 `manifest.json` 逐一复刻。
