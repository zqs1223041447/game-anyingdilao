# 工作规范与仓库须知

## ✅ P0 已解除（2026-09-01 V1.34 完整版 82AF138C）

**原 P0：**`MODworkv2/decompiled` 与 `Game-Later` 已部署 V1.32 不同源——已按方案 A 以 `0C779D0E` 反编译树回灌 V1.32 六项（`LastCastInfo`/`BuffTime死门`/`组件白名单`/`TargetPos环向`/`3+N单环均分`/`词条双回退`）至新基线树，构建 `82AF138C…599` 与已部署 V1.32 功能同源，静默回退风险已除。本节保留为历史记录，下次出包后可删除。详见 `docs/workspace-audit-2026-09-01.md`。

## Repository Map

A full codemap is available at `codemap.md` in the project root.

Before working on any task, read `codemap.md` to understand:
- Project architecture and entry points
- Directory responsibilities and design patterns
- Data flow and integration points between modules

For deep work on a specific folder, also read that folder's `codemap.md`.

## Project Notes

- 本工作区是 Unity Mono 游戏《暗影地牢》(Shadow Dungeon) 的编译成品 + 反编译研究区。**当前活动工作区为新版（Game-Later 基线 2026-09-01 重建）**：
  - `Game-Later/` — **新版完整游戏本体**（Shadow Dungeon.exe，Unity 2019.4.39f1，Mono 后端；vanilla Assembly-CSharp.dll `92E0120F…2D52`，2,352,640 字节，127 个 Managed DLL）
  - `Game-Later/` 备份验证副本（同基线）
  - `MODworkv2/decompiled/` — 新版 Assembly-CSharp.dll 的 ILSpy 8.2 反编译工程（可构建，netstandard2.0，923 .cs，含 V1.31 全部 MOD 代码）
  - `MODworkv2/refs/` — 新版构建引用 DLL（127 个，ASCII 路径副本，与 Game-Later/Managed 1:1 同步）
  - `MODworkv2/backup/` — vanilla 原版备份 `Assembly-CSharp.dll`（`92E0120F…2D52`，与 Game-Later 一致）+ `Assembly-CSharp-vanilla-new.dll`
  - `MODworkv2/builds/` — 升级包输出区（`ShadowDungeon-MOD-Vx.x_YYYY-MM-DD.zip`）
- **旧版已归档**：`_archive/modwork_archived/`（旧 MOD 工作区 V1.0-V1.5），仅回溯用；`_archive/暗影地牢 Demo_archived/` 已于 2026-08-31 删除。
- **重建说明（2026-09-01）**：Game-Later 为新版完整游戏更新包；MOD 工作区自 Game-Later 重新反编译并回灌 V1.31（词条档位显示 + 星环总出口）生成新构建 `82AF138C…599`（全量补 Hand/ItemScript/ContainerItemData/SK_FlySowrd/SettingBT/PlayerManager Pick_* 及 Enchanted 双重，最新）。作废链 358ACF51→BC3336A3→CDEF29C2→DF7DB06E 均因 typetree 未补全已作废；G: 盘 level1 需同步 Game-Later 的 level1，否则 PlayerManager 6032→6040 仍崩。
- 修改流程：改 `MODworkv2/decompiled` 源码 → `dotnet build -c Release` → 将产物覆盖到 `Game-Later/Shadow Dungeon_Data/Managed/Assembly-CSharp.dll`（及备份，原版备份在 MODworkv2/backup）

## 工作规范（Work Norms）

### 修改与验证流程（已验证，强制）
1. 任何源码修改后必须 `dotnet build -c Release` 且 **0 error** 才允许部署。
2. 部署前确认 `MODworkv2/backup/Assembly-CSharp.dll` 为 vanilla 原版备份；**禁止无备份覆盖**。
3. 部署后验证标准：游戏进程存活 ≥35 秒 + Player.log 中 Exception/Crash/TypeLoad/NullReference 命中数为 0。
4. 部署完整性用 SHA256 对比构建产物与目标 DLL。
5. **版本登记（强制）**：任何 DLL / 资产文件 / 源码修改在出包或部署时，必须在根目录核心文件 `CHANGELOG.md` 登记版本更新说明——版本号自 V1.0 起顺序递增，条目含日期/变更内容/涉及文件/产物 SHA256/验证状态/部署状态。
6. **升级包（强制）**：任何更新无论大小必须制作成可分发的升级包（`MODworkv2/builds/ShadowDungeon-MOD-Vx.x_YYYY-MM-DD.zip`，内含 `Assembly-CSharp.dll` + `install.ps1` + `README.md`），附 SHA256 并与已部署本体一致；禁止“只部署不打包”或“只打包不部署”的中间态。

### 并行协作纪律
- 所有子任务一律后台（background）派发；取结果用 task_result，不用轮询。
- 并行写文件的车道必须声明互不重叠的写范围；只读车道（侦察/调研）不得写工作区文件。
- 多条车道之间不互相等待：依赖未就绪时先做不重叠的工作。
- **模型并发预算（强制）**：智谱套餐按账号限制并发模型请求数（超限报错码 3008，主会话与其他多开会话均占并发位），同一时刻最多 **1 个**子 Agent 在跑，其余车道排队串行派发；构建/脚本等本地后台 Bash 任务不占模型并发，可与子 Agent 并行。

### 文档维护职责
| 文档 | 维护时机 |
|---|---|
| `CHANGELOG.md`（根目录·核心文件） | 每次 DLL/资产/源码修改出包或部署时登记版本更新说明 |
| `docs/status.md` | 每次里程碑/车道状态变化 |
| `docs/worklog.md` | 每次重要事件追加时间线条目 |
| `docs/todo.md` | 任务增删与状态流转 |
| `docs/tools-index.md` | 新工具安装/移除时登记 |
| `docs/code-index.md` | 关键代码定位信息更新时 |
| `docs/resource-index.md` | 资产容器/外部资源认知更新时 |
| `docs/research/` | 每次专项调研落盘一份记录 |

### 技术红线
- 不修改 `Game-Later/` 下除 `Managed/Assembly-CSharp.dll` 以外的任何文件（除非任务明确要求且已备份）。
- 反编译工程仅用于研究与本机 mod，不分发。
- **typetree 铁律（真机原生崩溃级）**：给任何会被 Unity 序列化的类新增 `public` 实例字段（MonoBehaviour/ScriptableObject 本体，或被其**内联序列化**的可序列化普通类，如 `WeaponClass`（经 CharButton/Hand/ItemScript）、`ACT_skillSample`（经 ACTListSkillBT/ACT_skillData））必须加 `[System.NonSerialized]`——`[HideInInspector]` **不阻止序列化**；运行时状态需持久化的走 `Data.SaveData/*SaveData` 显式字段拷贝链路。违者场景/资产反序列化错位，进关卡原生崩溃（"different serialization layout/level1 corrupted"），与 SK_FlyA P0 同类（V1.25 三处实证）。出包前可用原版 DLL 反编译树做全字段 diff 自检。
- **技能修改描述同步**：任何 SampleF CSV 加行或技能效果变更（列覆盖/Tier 2 代码补丁改变弹幕形态、数量、飞行行为），必须同步更新 Info 列指向的 Skill_FY 本地化键（resources.assets TextAsset path_id=433），保证 tooltip 文案与实际行为一致；禁止"行为已改、描述仍旧文案"或"加行无键"的中间态出包部署（校验细则见 docs/skill-spec.md「描述同步要求」）。
