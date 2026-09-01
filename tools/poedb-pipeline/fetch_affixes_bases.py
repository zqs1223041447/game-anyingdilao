# -*- coding: utf-8 -*-
"""
fetch_affixes_bases.py — 任务 A 第二阶段：补抓代表基底详情页（词缀池主体）
==========================================================================
POEDB 列表页（Amulets/Bows/Helmets_str…）静态只有隐式词缀；显式词缀池在
各基底详情页。本脚本从 affixes_all.json 的列表页 bases 里，为每个槽位类别
轮试候选基底详情页（直到解析出词缀），追加进 pages/ 与 affixes_all.json。
用法：python tools/poedb-pipeline/fetch_affixes_bases.py
"""
import json, os, re, sys, time
sys.path.insert(0, os.path.dirname(os.path.abspath(__file__)))
from fetch_affixes import http_get, parse_page, CACHE, PAGES, OUT  # noqa: E402

try:
    sys.stdout.reconfigure(encoding="utf-8")
except Exception:
    pass

FAMILIES = [("Helmets", "头盔"), ("Body_Armours", "胸甲"), ("Gloves", "手套"), ("Boots", "鞋子")]
WEAPONS = ["One_Hand_Axes", "One_Hand_Maces", "One_Hand_Swords", "Thrusting_One_Hand_Swords",
           "Two_Hand_Axes", "Two_Hand_Maces", "Two_Hand_Swords", "Bows", "Claws", "Daggers",
           "Rune_Daggers", "Staves", "Sceptres", "Wands", "Warstaves"]
# 类别 → 兜底基底候选（POE1 经典基底，词缀池同类别共享）
FALLBACK = {
    "主手武器": ["Eternal_Sword", "Splicer_Bow", "Imperial_Bow", "Harbinger_Bow", "Vaal_Blade", "Siege_Axe"],
    "头盔": ["Iron_Helmet", "Royal_Hunt_Guille", "Ezomyte_Burgonet", "Lion_Pelt"],
    "胸甲": ["Plate_Hauberk", "Astral_Plate", "Assassin_Garment", "Widowsilk_Robe"],
    "手套": ["Iron_Gauntlets", "Steelscale_Gauntlets", "Titan_Gauntlets", "Spiked_Gloves"],
    "鞋子": ["Iron_Greaves", "Steelscale_Greaves", "Titan_Greaves", "Sunscale_Sandals"],
    "护符": ["Onyx_Amulet", "Agate_Amulet", "Jade_Amulet", "Blue_Amulet"],
    "戒指": ["Two-Stone_Ring", "Gold_Ring", "Amethyst_Ring", "Coral_Ring"],
    "腰带": ["Leather_Belt", "Heavy_Belt", "Rustic_Sash", "Chain_Belt"],
    "副手": ["Tioco_Spirit_Shield", "Rounded_Spirit_Shield", "Branded_Kite_Shield", "Splinter_Quiver"],
    "药剂": ["Sapphire_Flask", "Quicksilver_Flask", "Granite_Flask", "Large_Hybrid_Flask"],
}


def pick_candidates(pages, cat):
    """从同类列表页 bases 取中部附近候选 + 兜底名单。"""
    cands = []
    for pname, pdata in pages.items():
        cat_of = None
        for fam, c in FAMILIES:
            if pname == fam or pname.startswith(fam + "_"):
                cat_of = c
                break
        if cat_of is None and pname in WEAPONS:
            cat_of = "主手武器"
        if cat_of is None:
            cat_of = {"Amulets": "护符", "Belts": "腰带", "Rings": "戒指",
                      "Utility_Flasks": "药剂", "Life_Flasks": "药剂", "Mana_Flasks": "药剂",
                      "Quivers": "副手"}.get(pname)
        if cat_of != cat:
            continue
        base_list = []
        seen = set()
        for s in pdata["sections"]:
            if "物品" in s["title"]:
                for b in s["bases"]:
                    if b["page"] not in seen:
                        seen.add(b["page"])
                        base_list.append(b["page"])
        if base_list:
            mid = len(base_list) // 2
            cands.extend(base_list[max(0, mid - 2): mid + 3])
    cands.extend(FALLBACK.get(cat, []))
    return list(dict.fromkeys(cands))


def main():
    all_data = json.load(open(os.path.join(OUT, "affixes_all.json"), encoding="utf-8"))
    pages = all_data["pages"]
    cats = ["主手武器", "头盔", "胸甲", "手套", "鞋子", "护符", "戒指", "腰带", "副手", "药剂"]
    ok = 0
    for cat in cats:
        if cat in ("头盔", "胸甲", "手套", "鞋子"):
            # 防具：列表页只有 implicit，必须抓基底详情
            need = True
        else:
            need = True
        if not need:
            continue
        got = False
        for base in pick_candidates(pages, cat):
            if base in pages and pages[base].get("slot_category") == cat:
                got = True
                break
            cache_f = os.path.join(CACHE, base + ".html")
            try:
                if os.path.exists(cache_f) and os.path.getsize(cache_f) > 1000:
                    h = open(cache_f, encoding="utf-8").read()
                else:
                    h = http_get("https://poedb.tw/cn/" + base)
                    open(cache_f, "w", encoding="utf-8").write(h)
                    time.sleep(0.2)
                secs = parse_page(h)
                n_mods = sum(len(s["mods"]) for s in secs)
                if n_mods < 10:
                    continue
                pages[base] = {"page": base, "url": "https://poedb.tw/cn/" + base,
                               "fetched_at": time.strftime("%Y-%m-%dT%H:%M:%S"),
                               "sections": secs, "slot_category": cat}
                json.dump(pages[base], open(os.path.join(PAGES, base + ".json"), "w", encoding="utf-8"),
                          ensure_ascii=False, indent=1)
                print("[OK] %-6s → %-26s mods=%d" % (cat, base, n_mods))
                got = True
                ok += 1
                break
            except Exception as e:
                print("   skip", base, str(e)[:50])
        if not got:
            print("[FAIL] %s：全部候选无效" % cat)
    json.dump(all_data, open(os.path.join(OUT, "affixes_all.json"), "w", encoding="utf-8"),
              ensure_ascii=False, indent=1)
    total = sum(len(s["mods"]) for p in pages.values() for s in p["sections"])
    print("[完成] 全量 %d 页 %d 词缀" % (len(pages), total))


if __name__ == "__main__":
    main()
