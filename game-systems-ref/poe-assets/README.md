# POE 商城特效素材库（参考复刻研究专用）

> 素材版权归 GGG（Grinding Gear Games）所有，仅作观感参考研究；**禁止拷入 `ShadowDungeon/`、禁止随升级包分发或商用**。法务论证见 `docs/research/poe-mtx-effect-fusion-example.md` §4。

## 当前状态（2026-08-28 晚，fetch-assets.py V2 抓取后）

- `skills/`：**69 个文件** —— 41 个真实 MTX 图标（webp）+ 28 个 YouTube 展示视频缩略图（jpg）
- `manifest.json`：131 条映射已回写真实数据（status：`ok`=41 / `no-direct-mtx`=90；新增 mtxEffect/mtxPage/mtxTitleCN/iconUrl/youtubeId/alts 字段）
- `fetch-report.json`：本次抓取逐条清单
- `cache/`：MTX 全目录（1389 条 `_Effect` 页）与已抓页面 HTML（断点续传用）

## 抓取工具：`fetch-assets.py`（替代已删除的 fetch-icons.ps1）

```bash
python fetch-assets.py    # 可重复运行，已下载文件自动跳过（断点续传）
```

旧脚本失败根因（已修正）：直接猜 CDN 路径 `Art/2DItems/Effects/<名>Effect.webp`——**不存在的路径 CDN 返回 403（非 404）**，18 连败后中止。V2 路线：抓 `/cn/Microtransactions` 总页提取全部真实 `*_Effect` 页 → 词级匹配（排除 Portal/Pet/外观类）→ 抓各 MTX 页提取**真实**图标 URL + YouTube 视频 ID + 中文名 → 下载（浏览器 UA + Referer，对真实路径返回 200）。

## `no-direct-mtx`（90 条）说明

POE 本来就不给这些做技能特效 MTX：buff/被动/DOT/光环类（霜冻、冥想、鹰眼、各种祝福…）、同伴类（骷髅战士、毒精灵…）、以及当初映射表的少量错误猜测（如 Razor Arrow / Poison Arrow 并非 POE 技能名）。这些技能的特效融合走"同系风格借位"（见调研报告 §4.2/§4.3）。

## 动画/视频素材的使用边界

- POE 特效本体是 GGG 私有引擎的实时 3D 粒子，**poedb 不提供任何效果资产下载**（只有图标 webp + YouTube 内嵌）。
- YouTube 视频**只有人工观感参考价值**：不可下载搬运（YouTube ToS + GGG 版权），不可抽帧做序列帧（衍生复制，同样违约）。缩略图仅作配色/构图参考。
- 游戏内"动画感"的合法实现路径（自制序列帧 / CC0 素材包 / Shuriken 程序化 / 游戏内粒子重着色）见 `docs/research/poe-fx-fusion-survey.md` §8。
