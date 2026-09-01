# POEDB 素材 × 暗影地牢 特效融合全景调研（多车道汇总）

> 日期：2026-08-28 ｜ 性质：**纯调研，未动任何代码/资产**
> 调研方式：3 条只读调研车道（素材盘点 / 游戏特效管线 / 融合矩阵分级）。后台子 agent 通道因 provider 并发限制不可用，按 2026-08-26 先例由编排层直接执行。
> 上游文档：`docs/research/poe-mtx-effect-fusion-example.md`（单示例研判，本报告为其全量扩展）｜ `game-systems-ref/poe-assets/`（素材区）

---

## 1. 结论先行

| 问题 | 判定 |
|---|---|
| POE 素材能否"完美融合"进本游戏 | **视觉素材直拷不可行**（GGG ToU 7b/7f + 格式不兼容）；**观感复刻可行**，已有 V1.18 `ApplyBlackFlameStyle` 验证过 Tier1 换色模式 |
| 131 条技能映射的融合分级 | **A 可完美融合 20 条 / B 需优化 87 条 / C 需特化重设计 24 条 / D 无对应 0 条** |
| 当前最大瓶颈 | ~~素材库为空~~ **已解决（2026-08-28 晚）**：403 真相=猜测路径不存在（CDN 对缺失文件返回 403 而非 404）；新工具 `fetch-assets.py` 走"页面抓真实 URL"路线，已落地 **41 技能图标 + 28 视频缩略图**（`skills/` 69 文件） |
| 装备效果（其余 4 主题） | 装备/怪物词缀 POE 侧为纯文本（无图标）；宝石图标可批量拉取；锻造为 UI 参照。详见 §5 |
| 实际修改测试 | 本次未做。若启动，必须用独立文件夹（建议 `MODworkv2/fx-testbed/`，与游戏目录/反编译工程隔离） |

---

## 2. 素材现状盘点（车道 1）

### 2.1 实际在库资产（全部）

| 文件 | 真实格式 | 价值 |
|---|---|---|
| `MysticBurningArrowEffect.webp`（6,236 B） | RIFF/WEBP **VP8X**（静态图标，非动画） | 配色/形态参考；Unity 不能直接用 webp，需转 PNG |
| `MysticBurningArrowEffect_video_hq.jpg`（30,481 B） | JPEG（YouTube hqdefault 缩略图） | 仅构图参考，分辨率不足以提取细节 |
| `manifest.json` | 131 条映射（localCN/localEN/xi/obj/poeCounterpart/poePage/poeIconGuess/status） | **全部 131 条 status=`guess-需校验`，URL 全部是猜测模式 `Art/2DItems/Effects/<名>Effect.webp`** |
| `skills/` | **空目录（0 文件）** | 批量拉取未落地任何文件 |

### 2.2 批量抓取失败真相（修正既有记录）

- `fetch.log`：2026-08-27 22:15 实际运行过，**18 连败全部 `403 Forbidden`**（非此前记录的"404 属预期"），脚本中止，`fetch-report.json` 从未生成。
- 403 说明 `cdn.poedb.tw` 对程序化直连（`Invoke-WebRequest` + 自定义 UA）做了拦截，或猜测路径模式不被接受——而示例文件（更早抓取）成功，说明 CDN 策略可能有时段/路径差异。
- **可行补全路线**（按推荐度）：
  1. **页面抓取路线**：抓 `poedb.tw/cn/<MTX页>` HTML，从页面 `<img>`/元数据中提取**真实** CDN 图标 URL 与 YouTube 视频 ID（不再猜 URL 模式）——同时天然解决"该技能是否真有 MTX"的校验问题；
  2. **YouTube 缩略图路线**：`img.youtube.com/vi/<id>/hqdefault.jpg` 对程序友好（示例已验证成功），批量拉取各技能展示视频缩略图作观感参考；
  3. 浏览器化抓取（browser-use）绕 UA/CDN 过滤，成本高，仅作兜底。
- 素材价值评估：webp 图标 = 配色板 + 潜在天赋节点图标参考；视频（缩略图/视频本体）= 唯一能看清"尾迹形态/粒子行为"的参考，**是复刻工作的刚需素材**；icon 本身对"特效复刻"贡献有限。

### 2.3 版权边界（README/manifest 原文要点）

素材版权归 GGG；仅作参考复刻研究，**禁止随包分发、禁止拷入 `ShadowDungeon/`**；`Content.ggpk` 可提但禁用于其他游戏。复刻产出文案写"灵感来自 POE ××× 风格"。

---

## 3. 游戏特效管线底座（车道 2，多为既有审计结论的确认与延伸）

### 3.1 一个技能特效的可动层

```
施法链：PlayerActionManager.UseSkill → Gun.CreatSP → SKPB.SK_Group[OBJ_Group].OBJ[OBJ] 预制体
弹体层：SpriteRenderer Arrow（贴图×tint）/ TrailRenderer[]（渐变+时长）/ par[] Shuriken 粒子
结算层：FX（命中特效）/ EXP（爆炸范围物）/ DOT 挂点（DOT_MG→DotEM）/ FMOD event:/ 字符串音频
形态分支：FStype 0-6 直射 / 3(MGC)=位移瞬移 / 7-8-9 环绕 SetParent 跟随 / 10 落点放置
```

- 粒子全线 Shuriken 直接挂预制体子节点（`SK_FlyBall.parLoop`、`SK_DZ.parLoop` 实证），代码只 Play/Stop——**代码层可改 startColor/gradient/size/rate/emission**。
- `SKprefab`（ScriptableObject）序列化只读数组在 sharedassets1.assets，运行时越界即崩；资产属技术红线（DLL-only）。
- **墙**：Spine 动画（角色/BOSS，投射物不用）与 FMOD bank 结构不可改；Built-in 2D 无 URP/Bloom 变体成本高。

### 3.2 特效改造分级（推广 V1.18 已验证模式）

| Tier | 手法 | 资产改动 | 风险 | 适用 |
|---|---|---|---|---|
| **Tier1** | 纯代码换色/调参：`ApplyBlackFlameStyle` 模式（Arrow.color + trail 渐变 + par 粒子 startColor 重着色，颜色收敛为文件头常量） | 无 | 低（typetree 安全：只加方法/静态常量，不加 public 序列化字段） | **A 级全部 + B 级大部分** |
| **Tier2** | 代码 + 运行时 `Texture2D.LoadImage`（**仅 PNG/JPG**，webp 需转码）/ 复用游戏材质克隆粒子 / FStype 参数特化（穿透/多弹/环绕半径） | 无（贴图放游戏目录外置或 Resources） | 中（性能：齐射控 maxParticles；LoadImage 主线程 IO） | B 级共性增强、图标替换 |
| **Tier3** | AssetsTools 改 sharedassets1.resources（预制体/图集/CSV）| 有（突破 DLL-only 红线，需备份+SHA256 登记） | 高 | C 级重设计、新增粒子图集 |
| 不可行 | Spine 换皮 / FMOD bank 改结构 / POE 资产直拷 | — | — | 墙 |

**教训约束**：V1.16 无贴图程序粒子（方块光斑）、V1.16 序列化字段 typetree P0、Mono `Warning` 不存在——新特效代码必须：复用游戏自带粒子重着色、新增字段加 `[NonSerialized]`、全路径 try/catch。

---

## 4. 融合矩阵分级（车道 3，131 条全量）

判级规则（规则化批量 + 关键词形态比对，边界条目建议人工复核）：

- **A 可完美融合（20 条）**：游戏形态与 POE 对应技能同构（投射物↔投射物、环绕↔刃类环绕、召唤↔召唤），Tier1 换色即可高保真。
- **B 需优化（87 条）**：主题相近但存在单侧形态偏差（如游戏环绕护盾 ↔ POE 光环 buff），Tier1+参数特化或 Tier2 小增强。
- **C 需特化/修改（24 条）**：POE 是本游戏管线没有的类别（区域持续/吟唱/位移），需映射到最接近的游戏形态（落点 FStype=10 / 环绕 / MGC 位移 FStype=3）或 Tier3。
- **D 无对应（0 条）**：当前映射表未出现（manifest 本身就是"择优配对"的产物，弱对应已归入 B/C）。

> 注：DOT/同伴技能与 POE 同名/同系技能配对；被动类未独立成桶。B 级是混合桶，实际动手前需逐条 triage。

### 4.1 A 级 Top-10 最佳候选

| # | 游戏技能 (Xi) | POE 对应 | 形态 | 融合手法 | 难度 |
|---|---|---|---|---|---|
| 1 | 火球术 FireBall (0) | Fireball | 直射 | Tier1 换色（橙红爆焰→按 MTX 主题） | ★ |
| 2 | 冰晶术 Ice Crystal (1) | Ice Spear | 直射 | Tier1（含回旋行为保留） | ★ |
| 3 | 冰霜球 Frost Ball (1) | Frostbolt | 直射·穿透 | Tier1 | ★ |
| 4 | 闪电球 Lightning Ball (2) | Ball Lightning | 直射·命中AoE·追踪 | Tier1 + 粒子重着色 | ★★ |
| 5 | 剑刃之舞 Blade Dance (3) | Blade Vortex | 环绕刃 | Tier1 换刃色/拖尾 | ★★ |
| 6 | 剃刀箭 Razor Arrow (6) | Razor Arrow | 直射（同名） | Tier1；已有环形 8 箭 mod 基础 | ★ |
| 7 | 烈焰箭 Flame Arrow (7) | Burning Arrow | 直射·命中/末段AoE | Tier1（即已研判的 Mystic Burning Arrow 示例本体） | ★ |
| 8 | 爆炸箭 Explosive Arrow (7) | Explosive Arrow | 直射·爆炸 | Tier1 + EXP 爆炸 tint | ★★ |
| 9 | 瘟疫之箭 Plague Arrow (8) | Plague Arrow | 直射·AoE·DOT | Tier1 + 绿毒系配色 | ★★ |
| 10 | 骷髅战士 Skeleton Warrior (9) | Skeleton Warrior | 召唤体 | Tier2（同伴外观贴图/发光重着色，Spine 不动） | ★★★ |

（同桶还有：剑灵 Blade Soul↔Bladefall、闪电之刃↔Lightning Strike、毒蛇之矢/叶绿箭↔Poison Arrow、复仇之矛↔Spear Throw、暗影球↔Shadow Orb、剧毒新星↔Venom Gyre、冰霜石头人↔Frost Golem，共 20 条。）

### 4.2 C 级最需重新设计 Top-5

| 游戏技能 (Xi) | POE 对应 | 为什么不同构 | 建议特化方向 |
|---|---|---|---|
| 暴风雪 Blizzard (1) | Blizzard | POE=屏幕级区域持续降雪；游戏无区域持续管线 | 映射落点型（FStype=10）+ 区域 DOT 粒子，Tier2/3 |
| 雷霆跃迁 Thunder Teleport (2) | Lightning Warp | POE=位移；游戏 Xi0-2 属 MGC 家族，**FStype=3 本身就是瞬移** | 换 FStype=3 + 闪电落点特效，Tier2（形态其实可行） |
| 圣盾庇护 Sanctuary Shield (4) | Shield Charge | POE=冲锋位移；游戏是环绕护盾 | 保环绕、借 POE 配色与光效（降级为 B 处理亦可） |
| 箭雨 Arrow Rain (8) | Rain of Arrows | POE=区域落下；游戏为单体/多弹 | 落点型多弹齐落（Tier2 数值+粒子特化） |
| 地狱火 Hellfire (0) | Incinerate | POE=吟唱射线；游戏无吟唱管线 | 重映射为多弹直射或环绕灼烧，纯观感借位 |

### 4.3 B 级（87 条）共性主题

护盾/结界类（游戏环绕 ↔ POE 光环，约 10 条）：Tier1 换色即可识别；区域/落点类（约 20 条）：需 Tier2 粒子面积特化；同名同类但子特征缺失（连锁/分裂/层数，约 30 条）：Tier2 代码增强，建议扩充 `docs/effects-library.md` Tier2 模式库；其余为映射宽松的（约 27 条），动手前逐条 triage。

---

## 5. 其余 4 主题（"装备效果"等）融合评估

| 主题 | 本游戏规模 | POE 侧对应 | 视觉素材 | 融合落点 | 分级 |
|---|---|---|---|---|---|
| 装备词缀 | 230+（01 文档） | POE Modifiers **纯文本** | 无（设计如此） | 数值/文案参照体系（词条措辞、 tier 划分思路） | 文本参照 A / 视觉 D |
| 宝石/镶嵌物 | 137（02 文档） | POE Gem | **有图标**（`cdn.poedb.tw/image/Art/2DArt/SkillIcons/`，可批量拉，受同样的 403 风险） | 镶嵌物图标风格参考 + 数值结构参照 | B |
| 怪物词缀 | 20（03 文档） | Monster Modifiers | 文本为主 | 精英词缀命名/数值参照；POE 精英光环可启发 Tier2 特效 | C |
| 锻造 | 33（04 文档） | Crafting Bench | 工作台 UI | 锻造选项文案/定价结构参照 | B（UI 文案参照） |

---

## 6. 缺口清单（启动实际融合前必须补齐）

1. **素材补全**：按 §2.2 路线 1（页面抓真实 URL + YouTube 视频 ID）重写抓取脚本；现有 131 条 guess URL 全部视为不可用。
2. **A 级 20 条逐条"观感规格卡"**：每条人工看一次 POE 页面/视频，记录配色三件套（箭体/尾迹/粒子）与形态要点 → 形成可执行的 Tier1 参数表（延续"颜色收敛为常量"约定）。
3. **弹体映射核实**：A 级候选的 OBJ → SK_Fly 系类别（SK_FlyA/SK_FlyBall/SK_Angle_F…）逐条确认，决定套用哪个换色钩子位置。
4. **Tier2 模式库扩充**：B 级共性（连锁/分裂/区域/位移）设计入 `docs/effects-library.md`。
5. **独立测试文件夹**：实际动手时建 `MODworkv2/fx-testbed/`（独立构建产物+测试记录，不触碰 `ShadowDungeon/` 与 `MODworkv2/decompiled/` 主工程），验证通过才按正规流程进主工程出包。**本次纯调研未创建。**
6. 版权文案红线：任何复刻描述写"灵感来自 POE"，POE 原素材不进游戏目录、不进升级包。

## 7. 建议下一步（供决策）

1. 修抓取脚本（路线 1）+ 批量拉 YouTube 缩略图 → 素材库从 2 个文件补到可用；
2. 从 A 级 Top-10 里挑 2-3 条做 Tier1 试产（推荐：烈焰箭↔Burning Arrow【示例研判已就绪】、剃刀箭↔Razor Arrow、火球术↔Fireball），走 fx-testbed → 主工程 → 出包全流程验证一次；
3. 人工 triage B 级 87 条，产出第二批规格卡。

---

> 关联：`poe-mtx-effect-fusion-example.md`（单示例深研）｜ `game-systems-ref/poe-assets/{manifest.json,fetch-report.json,README.md}` ｜ `docs/research/skill-tags-catalog.md` ｜ `docs/effects-library.md` ｜ `game-systems-ref/01~05-*.md`

---

## 8. 追补（2026-08-28 晚）：素材补全已执行 + 动画可用性结论 + 替代方案

### 8.1 抓取脚本已修复并执行（缺口 1 完成）

- **403 真相**：CDN 对**不存在的路径返回 403**（非 404）——旧脚本 18 连败全是"URL 猜错"，不是被墙；对真实路径，浏览器 UA + Referer 即 200。
- 新工具 `game-systems-ref/poe-assets/fetch-assets.py`（替代已删除的 fetch-icons.ps1）：抓 `/cn/Microtransactions` 总页 → 提取 **1389 条真实 `*_Effect` MTX 页目录** → 与 131 条映射词级匹配（排除 Portal/Pet/外观类误配）→ 抓各 MTX 页提取真实图标 URL + YouTube 视频 ID + 中文名 → 下载落盘。可重复运行、断点续传。
- **执行结果：ok=41 / no-direct-mtx=90 / failed=0**；`skills/` 69 文件（41 图标 webp + 28 视频缩略图 jpg）；manifest 已回写真实 URL/视频 ID/中文名。
- 90 条无对应属 POE 设计使然（buff/被动/DOT/同伴类无技能特效 MTX）+ 少量当初映射错误猜测（Razor Arrow / Poison Arrow 等并非 POE 技能名）——这些走 §4.2/§4.3 的"同系风格借位"。

### 8.2 动画素材可以直接用到本游戏吗？——**不能，抓取价值=人工参考**

| 形态 | 能否直接用 | 原因 |
|---|---|---|
| POE 引擎内特效本体 | 否 | GGG 私有引擎实时 3D 粒子/骨骼，格式与 Unity 不兼容且版权禁止；poedb 根本不提供下载 |
| poedb 图标 webp | 否（仅参考） | 静态图标；Unity 不支持 webp；作为游戏内贴图=复制资产（违约）。转 PNG 后也只能当配色板参考 |
| YouTube 展示视频 | 否（仅参考） | 不可下载搬运（YouTube ToS + GGG IP）；不可抽帧做序列帧（衍生复制）；技术上 VideoPlayer 放视频当特效也不成立（无 alpha、不响应游戏状态、固定镜头） |

→ **视频/缩略图的抓取价值 = 给人看的观感规格来源**（配色三件套、弹道形态、粒子节奏），28 个缩略图正是这个用途；规格转换成人手做，不进游戏。

### 8.3 替代方案（不照抄，"类似的"合法路径，按推荐度）

> **▶ 动态演示页：`game-systems-ref/poe-assets/fx-alternatives-demo.html`**（双击/浏览器打开，纯 Canvas 离线运行）——三方案同屏动态对比，支持 火球术/冰晶术/黑炎 三主题切换：方案一展示"生成的 8 帧 sprite sheet + 游戏内播放效果"；方案二展示"同一套底材贴图 × 三主题 tint 的一底多色发射器"；方案三展示"Trail 拖尾弹体 → Sub-Emitter 末段爆裂 + Size/Color over Lifetime 曲线可视化"。页面右上为 POEDB 抓取的真实图标对照（不进游戏）。

1. **游戏内粒子重着色/重组（Tier1，已验证）**：V1.18 `ApplyBlackFlameStyle` 模式推广——复用游戏自带 Shuriken 粒子（真实贴图）换 startColor/gradient/size/rate。零新素材即可覆盖 A 级 20 条的主体观感。
2. **自制序列帧 Flipbook（"动画感"主力）**：自绘/程序生成 sprite sheet（爆炸团、冰晶碎片、烟圈），Shuriken `Texture Sheet Animation` 或代码帧动画播放。Built-in 2D 原生支持，版权干净，节奏（起爆→维持→消散）可对照 POE 视频调。
3. **CC0/免费授权特效素材包（可直接进游戏）**：Kenney Particle Pack（CC0，可商用免署名，kenney.nl）、Unity 官方 Particle Pack（Asset Store 免费，EULA 允许游戏内使用）、OpenGameArt CC0 FX 序列帧。**与 POE 素材的本质区别：这些能合法随包分发**。用它们做粒子贴图底材，再按 POE 参考调 tint/参数。
4. **Shuriken 深度程序化（Tier2）**：size/color over lifetime 曲线、sub-emitter（末段爆裂）、粒子 Trails、重力+拉伸（Bladefall 落刃 = burst + gravity + stretch sprite）。
5. 不推荐：Custom Shader 变体（Built-in 管线全局批次影响、成本中风险中）；Bloom 后处理（无 URP）。

### 8.4 示范：观感规格卡（下一步批量产物格式，从已落地素材提炼）

| 卡 | 素材来源 | 配色三件套 | 形态要点 → 落地参数（Tier1） |
|---|---|---|---|
| 火球术 ↔ Dragon_Fireball_Effect | `skills/FireBall__Dragon_Fireball_Effect.webp` | 头部暗红 `#8B2500` / 主体橙 `#FFA02E` / 端梢亮黄 `#FFE08A` | 蓬松团簇火焰尾+大亮头 → Arrow 暗红核 + trail 橙黄渐变 + par 火星重着色、rate 调高 |
| 冰晶术 ↔ Swordfish_Ice_Spear_Effect | `skills/Ice Crystal__Swordfish_Ice_Spear_Effect.webp` | 头部白蓝 `#EAF6FF` / 碎片淡冰 `#BFE3F5` / 尾端透明 | 锐利亮头+离散碎片尾 → Arrow 亮白蓝 + trail 短而淡 + par 少量尖锐碎片（size 小、rate 低） |

> 批量产出方式：对 41 条 `ok` 技能逐张看图标（+有视频的看视频页）填卡；每卡即一份 Tier1 常量参数表（延续"颜色收敛为常量"约定）。实测建议在 `MODworkv2/fx-testbed/` 独立文件夹进行（仍未创建，待启动）。
