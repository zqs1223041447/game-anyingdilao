# -*- coding: utf-8 -*-
"""
fetch_poedb.py — POEDB 数据抓取器（雏形）
==========================================

基于 poedb.tw / poe2db.tw 的真实页面结构（Support_Gems、Modifiers、
Crafting_Bench、Passive_Skill_Tree 等）设计抓取逻辑。

当前实现：
  - 提供 `fetch_category(category)` 接口，按类别抓取并转换为统一 schema。
  - 提供 `--offline` 模式：无网络时回退到 seed_data.py 生成的本地种子数据，
    保证管线在任何环境都能产出可用的 data/poedb/*.json。

真实抓取说明（后续增量）：
  - poedb.tw 页面为服务端渲染 HTML，词缀/宝石数据以表格呈现。
  - 可参考开源项目抓取思路：
      * RePoE / PyPoE：解析 GGPK 数据文件（.dat），字段级精确。
      * haharazer/poe-trans-data：翻译数据。
      * Chuanhsing/poe-api：API 封装。
  - 本脚本的 `_parse_*` 函数预留了 HTML 表格解析的骨架，接入真实抓取时
    只需替换数据来源即可，Schema 不变。

用法：
    python tools/poedb-pipeline/fetch_poedb.py --category support_gems --offline
    python tools/poedb-pipeline/fetch_poedb.py --all --offline
"""

from __future__ import annotations

import argparse
import json
import os
import sys
from datetime import datetime, timezone

sys.path.insert(0, os.path.dirname(os.path.abspath(__file__)))
from schema import CATEGORIES, write_json  # noqa: E402
from seed_data import build_all  # noqa: E402


def _now() -> str:
    return datetime.now(timezone.utc).isoformat()


def fetch_category(category: str, offline: bool = True) -> dict:
    """抓取单个类别数据。

    真实网络抓取时，这里应调用 requests/httpx 请求 poedb 页面并解析表格。
    当前雏形在 offline 模式下直接复用 seed_data 的本地数据。
    """
    if offline:
        data = build_all()
        if category not in data:
            raise ValueError(f"未知类别: {category}")
        cat_data = data[category]
        cat_data["fetched_at"] = _now()
        return cat_data

    # ---- 真实抓取骨架（预留）----
    # 1. 构造 URL：https://poedb.tw/us/<CategoryPage>
    # 2. GET 页面 → BeautifulSoup 解析表格
    # 3. 按类别调用 _parse_* 转换 → 统一 schema
    # 4. 返回 make_category_file(category, items)
    raise NotImplementedError(
        "在线抓取尚未接入。请使用 --offline 模式（基于 seed_data 本地数据），"
        "或参考 RePoE/PyPoE 接入 .dat 解析。"
    )


def main() -> int:
    parser = argparse.ArgumentParser(description="POEDB 数据抓取器")
    parser.add_argument("--category", help="单个类别（equipment_effects/support_gems/...）")
    parser.add_argument("--all", action="store_true", help="抓取全部类别")
    parser.add_argument("--offline", action="store_true",
                        help="离线模式：使用 seed_data 本地数据（默认开启）")
    parser.add_argument("--out", default="data/poedb", help="输出目录")
    args = parser.parse_args()

    out_dir = os.path.abspath(args.out)
    os.makedirs(out_dir, exist_ok=True)

    if args.category:
        categories = [args.category]
    elif args.all:
        categories = list(CATEGORIES.keys()) + ["skills"]
    else:
        parser.error("必须指定 --category 或 --all")

    # 确保控制台 UTF-8
    try:
        if hasattr(sys.stdout, "reconfigure"):
            sys.stdout.reconfigure(encoding="utf-8", errors="replace")
    except Exception:
        pass

    for cat in categories:
        try:
            cat_data = fetch_category(cat, offline=args.offline)
            write_json(os.path.join(out_dir, f"{cat}.json"), cat_data, use_bom=True)
            print(f"[OK] {cat}.json  ({len(cat_data.get('items', []))} items)")
        except NotImplementedError as e:
            print(f"[SKIP] {cat}: {e}")
        except ValueError as e:
            print(f"[ERR] {cat}: {e}")

    # 重新生成 manifest
    from seed_data import write_manifest
    data = build_all()
    write_manifest(out_dir, data)
    print("[OK] manifest.json")
    print(f"Output dir: {out_dir}")
    return 0


if __name__ == "__main__":
    sys.exit(main())
