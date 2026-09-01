# 工作区现状梳理与风险审计（2026-09-01）

> 审计方式：文档全量阅读（AGENTS.md / codemap.md / CHANGELOG.md / docs/*）+ 磁盘实测（SHA256、文件计数、源码检索）+ 实机构建验证。
> 结论优先级：P0 = 会导致实际 damage，必须处置后才能继续出包；P1 = 文档失真，会误导后续决策；P2 = 整理性债务。

---

## 一、磁盘实测事实（可直接复验）

| 对象 | 实测 SHA256 前 16 位 | 大小 | 判定 |
|---|---|---|---|
| `Game-root/Shadow Dungeon_Data/Managed/Assembly-CSharp.dll` | `92e0120fb939bfac` | 2,352,640 B | **原版 vanilla，未部署任何 MOD** |
| `ShadowDungeon/Shadow Dungeon_Data/Managed/Assembly-CSharp.dll` | `0c779d0ec89759a4` | 2,456,576 B | **V1.32 七轮修订版** |
| `MODworkv2/backup/Assembly-CSharp.dll` | `92e0120fb939bfac` | 2,352,640 B | 原版备份有效，可回滚 |
| `MODworkv2/backup/Assembly-CSharp-vanilla-new.dll` | `92e0120fb939bfac` | 2,352,640 B | 同上（冗余副本） |
| `MODworkv2/decompiled/bin/Release/netstandard2.0/Assembly-CSharp.dll` | `df7db06eec166148` | 2,452,480 B | **V1.31-rebuild** |
| `MODworkv2/builds/ShadowDungeon-MOD-V1.31_2026-09-01/Assembly-CSharp.dll` | `df7db06eec166148` | 2,452,480 B | V1.31-rebuild 包 |
| `MODworkv2/builds/ShadowDungeon-MOD-V1.32_2026-08-30/Assembly-CSharp.dll` | `0c779d0ec89759a4` | 2,456,576 B | 与已部署逐字节一致 |

**其他实测**

- `dotnet build -c Release`：**0 error / 0 warning**（增量构建，源码无改动）。
- `MODworkv2/decompiled` 共 **923 个 .cs**（vanilla 895 + MOD 28，`PoedbMod/` 22 文件）。
- `MODworkv2/refs` = 127 个 DLL，与 `Game-root/Managed` 数量 1:1。
- 两个游戏目录的资产容器**完全一致**：`level1` `67d86d7bb51f`、`resources.assets` `d9948ac3aced`、`sharedassets1.assets` `ca31e40a2f18` 三者 SHA 全等。差异仅存在于 `Assembly-CSharp.dll`。
- `Game-root` 与 `ShadowDungeon` 的 Managed 目录均为 127 个 DLL。
- V1.32 升级包自检**通过**：包内 `install.ps1` 的 `$expectedHash = 0C779D0E…23E27` 与包内 DLL 实测哈希一致，且等于 `ShadowDungeon` 已部署 DLL。

---

## 二、P0 — 源码树与部署版本不同源（最高优先级）

### 现象

源码树 `MODworkv2/decompiled` 的构建产物是 **DF7DB06E（V1.31-rebuild）**，而磁盘上真实部署的是 **0C779D0E（V1.32 七轮）**。两者不是同一个代码状态。

### 取证

在源码树中检索 V1.32 的特征代码，全部落空或仍是旧逻辑：

| V1.32 修订 | 检索特征 | 实测结果 |
|---|---|---|
| 三轮 · 出手遥测 | `LastCastInfo` | 全树 **0 命中** |
| 四轮 · 移除 BuffTime 死门 | `dt.BuffTime > 0f` | **仍在** `PoeItemMod.cs:172` |
| 五轮 · 移除组件白名单 | `SK_FlyA/Ball/Follow/Sowrd` 白名单 | **仍在** `PoeItemMod.cs:172` 同一行 |
| 三轮 · TargetPos 环向修正 | 克隆弹目标点重算 | 不存在 |
| 六/七轮 · 3+N 单环均分 | 总画幅 360° 均分 | 不存在 |
| 一轮 · 词条档位双回退 | 全局档位梯 / 品质档家族池 | 不存在 |

`PoeItemMod.cs:172` 现状（同时包含两个已判定为"死门"的拦截条件）：

```
if (gun == null || dt == null || dt.BuffTime > 0f
    || (dt.GetComponent<SK_FlyA>() == null && dt.GetComponent<SK_FlyBall>() == null
        && dt.GetComponent<SK_FlyFollow>() == null && dt.GetComponent<SK_FlySowrd>() == null))
```

这正是 CHANGELOG 记载的「星环之戒长期无效果」的两个终极根因。它们在新基线树里**完好无损地回来了**。

### 根因

2026-09-01 的重建流程是：

```
Game-root vanilla DLL (92E0120F)
  → ILSpy 重新反编译到 decompiled_fresh
  → 以「2026-08-30 版 V1.31 构建产物 2216612A」的反编译树回灌 MOD 代码
  → 构建 DF7DB06E
```

回灌源选的是 **V1.31**（`2216612A`），而不是 **V1.32 七轮**（`0C779D0E`）。V1.32 七轮的代码从未进入新基线树。

### 危害

任何人按 `AGENTS.md` 的标准流程「改源码 → `dotnet build` → 覆盖部署」操作，会把游戏从 V1.32 七轮**静默降级到 V1.31-rebuild**。表现为：星环之戒环形发射失效、回响之链识别异常、词条档位标注大面积缺失、诊断遥测行消失——且因为构建 0 error、冒烟 0 异常，**不会有任何报错提示**。这与 8 月 30 日用户反复反馈"星环没效果"的现象完全同构。

### 处置（二选一，需用户决策）

- **方案 A（推荐）**：以 `0C779D0E` 的反编译树为源，把 V1.32 七轮的 MOD 代码回灌到当前新基线树，重新构建并出 **V1.33**。功能不丢，且获得新基线的全部 typetree 修复。
- **方案 B**：认定 V1.31-rebuild 为新的功能主线，明确废弃 V1.32 七轮的六项修订（意味着放弃星环/回响修复），并同步清理 todo/status 中的相关验收项。

> 在决策落地前，**禁止对 `ShadowDungeon` 或 `Game-root` 执行任何部署操作**。

---

## 三、P1 — 文档登记与磁盘状态失真

| # | 文档位置 | 登记内容 | 实测 | 性质 |
|---|---|---|---|---|
| 1 | `CHANGELOG.md:16-17`「当前部署状态」表 | Assembly-CSharp.dll = `DF7DB06E…` | 磁盘无任何目录是它；`ShadowDungeon` = `0C779D0E`，`Game-root` = `92E0120F` | 登记错误 |
| 2 | 同上 | 未区分双目录 | `Game-root` **从未部署 MOD**，仍是原版 | 描述缺失 |
| 3 | `codemap.md:43` | 新构建 `358ACF51…2610E` | `SHA256-V1.31-2026-09-01.txt` 明确标注 358ACF51 **已作废**（缺 Enchanted 字段，typetree 不全） | 引用作废哈希 |
| 4 | `docs/status.md:7` | 新构建 `358ACF51…` / zip `FF4211B4…1776AE` | 实际产物 `DF7DB06E…`，zip 实测 `F8798159…` | 双错 |
| 5 | `docs/status.md` 第 19/21/23/25/27/29 行 | V1.25/V1.26/V1.30/V1.31 各条均写「部署 SHA `7A5ED0BC…89DDC8` 一致」 | `7A5ED0BC` 实为 **V1.32 六轮**产物；六个不同版本不可能共用同一哈希 | 复制粘贴污染 |
| 6 | `CHANGELOG.md:34,44` V1.32 条目 | zip 哈希 `2830EED2…` / `DA639D3F…` | 实测 `ShadowDungeon-MOD-V1.32_2026-08-30.zip` = `CD384BDF…` | zip 重打包未回登 |
| 7 | `AI-Handover-2026-09-01/README-接手必读.md:19` | decompiled 895 .cs | 实际 923 个（895 vanilla + 28 MOD） | 数字过时 |
| 8 | `AGENTS.md` / `codemap.md` | Game-root 与 ShadowDungeon「同基线」 | vanilla 基线确为同一，但 Game-root 未部署 MOD，措辞易被读成"两处都装了 MOD" | 措辞误导 |

---

## 四、P1 — CHANGELOG 结构破损

`CHANGELOG.md` 已不再满足其自定的「按版本号降序、新在上」规则：

1. **V1.23 条目丢失标题行**。`CHANGELOG.md:190` 直接以 `- **日期**：2026-08-29` 起段（内容为"新增 5 件测试装备"），被上方的 V1.25 段落吞并，读者会误以为 5 件装备是 V1.25 的内容。
2. **排序乱序**：V1.32 → V1.31 → V1.31-rebuild → V1.30 → V1.29 → V1.28 → **V1.24** → V1.27 → V1.26 → V1.25 → (V1.23) → V1.22 → …  V1.24 被夹在 V1.28 与 V1.27 之间。
3. **陈旧锚点残留**：`:421` 行 `<!-- 下一版本号：V1.6。新条目追加在本注释之下，按版本号降序（新在上）。 -->`，其下仍挂着 V1.4 / V1.3 / V1.2 三条。实际版本已到 V1.32。
4. **V1.11–V1.15 完全未登记**，但 `MODworkv2/builds/` 下存在 6 个对应 zip（V1.10~V1.15）。版本号出现空洞。

---

## 五、P2 — 待办与已知问题的整理性债务

1. **`docs/todo.md`「进行中」有 6 条同时自称"最优先"**（V1.32 / V1.31 / V1.30 / V1.29 / V1.28 / V1.27 / V1.26 各一条），彼此矛盾。实际只需验 V1.32 一条（其余均已被取代）。应收敛为单条并标注"已被 V1.32 取代"。
2. **KI-002（ArcBoomerang 缺本地化键）挂起 8 天未部署**，且其修复方案指向 `resources.assets` 的 `path_id=433`。但 `CHANGELOG.md:19` 已实测新版基线 Skill_FY 实际在 **`path_id=472`**——该 KI 的修复步骤已失效，需重新定位。
3. **`docs/todo.md:69` 提到「框架实战首用」与 SkillForge 流水线**，相关产物在 `_archive/modwork_archived/`，与新基线工作区已脱节，需确认是否仍为活跃目标。
4. **根目录存在 3 份交接快照**：`AI-Handover-2026-09-01/`（49 文件）、`AI-Handover-Full-2026-09-01/`（1543 文件）及各自 zip。其中 README 已出现数字过时（见第三节 #7），建议在 V1.33 出包后重新生成一份，并删除旧快照，避免多份交接包互相矛盾。
5. **根目录 `builds/` 与 `MODworkv2/builds/` 两个出包目录并存**（根目录那份是 V1.9 时代的 `packs/` 残留），易混淆。

---

## 六、健康的部分（确认可用）

- 构建链路完全健康：`dotnet build -c Release` **0 error / 0 warning**，refs 与游戏本体 127 个 DLL 同步。
- 原版备份有效：`92E0120F…` 两份备份齐全，随时可回滚。
- V1.32 升级包自洽且已部署：包内 `install.ps1` 哈希断言通过，与 `ShadowDungeon` 逐字节一致。
- 资产容器两目录完全一致，未出现资产分叉（技术红线守住）。
- `docs/research/` 下 13 份专项调研（投射物机制、词条档位设计、工艺台 metamods、POE 词缀映射等）质量高，是后续迭代的有效资产。
- V1.32 期间形成的三条工程纪律（typetree 铁律、打包哈希脚本断言、并行撞号处置）已被实证有效，应继续保留。

---

## 七、建议的下一步顺序

1. **先决策 P0 处置方案**（A 回灌 V1.32 七轮出 V1.33 / B 废弃 V1.32 修订）。在此之前不部署。
2. 决策后修正 `CHANGELOG.md`「当前部署状态」表，改为分目录登记（Game-root / ShadowDungeon 各一行），并以实测哈希为准。
3. 清理 `codemap.md` / `docs/status.md` 中的作废哈希与复制粘贴污染。
4. 重整 CHANGELOG 结构：补 V1.23 标题、删除陈旧注释、补登或清退 V1.11–V1.15。
5. 收敛 `docs/todo.md` 至单条 V1.33 验收项。
6. 重新定位 KI-002 的 `path_id`（433 → 472）。
7. 出包后重新生成一份交接快照，删除旧的两份。
