# 资源索引 (Resource Index)

> 资产容器/外部资源认知更新时维护本表。

## 技能 CSV 定位表（fix-12 扫描实证，与 exp-2 偏移交叉吻合）

全部位于 **sharedassets1.assets**：

| path_id | TextAsset 名 | 对应解析器 | 字节 |
|---|---|---|---|
| 1276 | 0 SampleF | LoadData_SampleF | 29,287 |
| 1226 | 1 SampleS | LoadData_SampleS | 20,774 |
| 1266 | 2 CompF | LoadData_CompF | 6,652 |
| 1188 | 3 CompS | LoadData_CompS | 4,512 |
| 1224 | 4 DotF | LoadData_DotF | 3,114 |
| 1348 | 5 DotS | LoadData_DotS | 5,181 |
| 1204 | 6 Bei | LoadData_Bei | 2,500 |
| 1210 | Xi | LoadData_Xi | 544 |

次要命中：Boss 敌人表(1215)、Baoshi(1222)/UseItem(1339) 物品表、Skill_FY 本地化 JSON(resources.assets path_id=433，IndexName→中文映射，可恢复源头损坏的中文名)。

⚠️ **源头数据损坏警告**：Xi/Baoshi/UseItem 表内中文在开发者导入期已被有损转换（资产内即 U+FFFD），资产侧不可恢复；恢复途径 = Skill_FY 本地化 JSON。

清单与预览：`modwork/asset-inventory/`（textassets-*.csv、preview/*.txt、REPORT.md）。

## 游戏资产容器（暗影地牢 Demo/Shadow Dungeon_Data/）

| 容器 | 内容判定（exp-2 实证） | 备注 |
|---|---|---|
| `sharedassets1.assets` (201.89MB + `.resS` 1,756MB) | **level1 主场景全部游戏性资产：8 个技能 CSV（Xi/SampleF/SampleS/CompF/CompS/DotF/DotS/Bei）+ Enemy/WP/USE/BS CSV + SKprefab SO + IconData 图标 Sprite + Spine skeleton/atlas + 全部技能/特效/弹体预制体** | 资产修改的主战场，操作前必须整文件备份 |
| `resources.assets` (319MB + `.resS` 3,117MB) | 9 个 `_FY` 本地化 JSON TextAsset（经 res:// 加载）、AudioData/MusicData SO（event:/ 明文路径）、res:// UI/Item prefab | |
| `sharedassets0/2.assets` | 近空（~4KB） | 忽略 |
| `level0/1/2` | 场景对象（TalentManager/GameDataManager/Gun/ACTbar 都在 level1） | |
| `StreamingAssets/Desktop/*.bank` ×14 | FMOD 2.2.24 音频库；**Skill.bank 71,569KB** 为技能音效主容器；另有 0Main/1Grass/2Forest/3Desert/4Gem/5Mountain/6Hell/Atmos/Enemy/Master/Scene/UI | 结构性修改不可行，仅等长替换或绕行 |
| `globalgamemanagers(.assets)` | 引擎全局配置 | 不动 |
| `boot.config` / `app.info` | 启动配置 / 公司名(OO Cat)+产品名(Shadow Dungeon) | 日志路径依据 |
| `Managed/` | 运行时程序集（修改目标 Assembly-CSharp.dll 在此） | 全套 DLL 在位是 Mono 资产编辑的最大利好 |
| `globalgamemanagers(.assets)` | 引擎全局配置 | 不动 |
| `boot.config` / `app.info` | 启动配置 / 公司名(OO Cat)+产品名(Shadow Dungeon) | 日志路径依据 |

## 工作区资源

| 路径 | 内容 |
|---|---|
| `modwork/refs/` | 127 个引用 DLL（ASCII 路径副本） |
| `modwork/backup/Assembly-CSharp.dll` | 原版备份（1,688,064 字节）——禁止无备份覆盖 |
| `modwork/decompiled/` | 反编译工程（840 .cs，基线 0 error） |
| `modwork/tools/` | 资产工具（UABEA、AssetScan 扫描器）——测试准备车道产出 |
| `modwork/asset-inventory/` | 资产扫描产出（TextAsset 清单/CSV 候选/REPORT.md） |

## 运行日志

`%USERPROFILE%\AppData\LocalLow\OO Cat\Shadow Dungeon\Player.log`
基线特征：shader Unsupported 警告、D3D11 视频解码回退、SteamApi_Init failed（无 Steam 客户端）、Odin Serializer 初始化——均为无害项；判定标准是 Exception/Crash/TypeLoad/NullReference 零命中。

## 外部资源链接（lib-1 调研沉淀）

| 资源 | 链接 | 用途 |
|---|---|---|
| UABEA / UABEANext | github.com/nesrak1/UABEA · /UABEANext | TextAsset/Texture2D 编辑主工具 |
| AssetsTools.NET Wiki | github.com/nesrak1/AssetsTools.NET/wiki | 脚本化资产读写（含新增条目+容器登记） |
| AssetRipper | github.com/AssetRipper/AssetRipper | 只读检查器/素材导出 |
| BepInEx 5 LTS | github.com/BepInEx/BepInEx/releases | 运行时注入（Unity Mono x64） |
| Spine JSON 格式 | esotericsoftware.com/spine-json-format | 4.0 骨架数据格式（复用为主） |
| spine-unity 安装表 | esotericsoftware.com/spine-unity-installation | 版本兼容（4.0 支持 2019.4 ✅） |
| FMOD 社区工具 | IZH318/FSB-BANK-Extractor-Rebuilder 等 | bank 试听/等长重建 |

## 技术画像结论（lib-1）

Mono 后端 + 传统资产布局（无 Addressables）+ 全套 Managed DLL 在手 → UABEA 的 MonoBehaviour type-tree 反推无障碍；CRC 风险为零；TextAsset 编辑是最安全资产操作。
