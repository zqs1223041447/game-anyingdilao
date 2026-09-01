# -*- coding: utf-8 -*-
"""
fetch_affixes.py — POEDB 装备词缀抓取（任务 A：真实抓取版）
=============================================================
数据源路线（已探明）：
  1. https://poedb.tw/cn/Modifiers  → 全部词缀页链接（<Base>#ModifiersCalc，101 页）
     页面类型两种：
       a) 列表页（Amulets/Bows/Helmets_str…）：物品卡区=基底+隐式词缀；传奇区=传奇 explicit
       b) 基底详情页（Onyx_Amulet/Crimson_Jewel/Bone_Ring…）：该基底完整词缀池（implicit+explicit，带数值范围）
  2. 列表页再提取每类代表基底详情链接补抓（词缀池主体在详情页）
输出：
  data/poedb/affixes/cache/*.html          原始页缓存（断点续传）
  data/poedb/affixes/pages/<页>.json       每页解析结果（区块→词缀文本+数值范围）
  data/poedb/affixes/affixes_all.json      全量合并（按 POE 页归并）
礼貌抓取：200ms 间隔，重试 2 次。依赖：仅 Python3 标准库。
用法：python tools/poedb-pipeline/fetch_affixes.py [--limit N]
"""
import json, os, re, sys, time, urllib.request

try:
    sys.stdout.reconfigure(encoding="utf-8")
except Exception:
    pass

HERE = os.path.dirname(os.path.abspath(__file__))
ROOT = os.path.abspath(os.path.join(HERE, "..", ".."))
OUT = os.path.join(ROOT, "data", "poedb", "affixes")
CACHE = os.path.join(OUT, "cache")
PAGES = os.path.join(OUT, "pages")
for d in (OUT, CACHE, PAGES):
    os.makedirs(d, exist_ok=True)
UA = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/126.0 Safari/537.36"


def http_get(url, tries=3):
    last = None
    for i in range(tries):
        try:
            req = urllib.request.Request(url, headers={"User-Agent": UA, "Accept": "*/*"})
            with urllib.request.urlopen(req, timeout=30) as r:
                return r.read().decode("utf-8", "ignore")
        except Exception as e:
            last = e
            time.sleep(0.5 * (i + 1))
    raise last


def clean_mod(html_frag):
    """explicitMod div 内 HTML → (文本[数值内联], 数值范围列表)。"""
    frag = re.sub(r'<span class="ndash">[^<]*</span>', '—', html_frag)
    vals = []
    def sub_val(m):
        inner = re.sub(r"<[^>]+>", "", m.group(1)).strip()
        nums = re.findall(r"-?[\d.]+", inner)
        if len(nums) >= 2:
            vals.append([float(nums[0]), float(nums[1])])
        elif len(nums) == 1:
            vals.append([float(nums[0]), float(nums[0])])
        return "<MV>%s</MV>" % inner
    frag = re.sub(r"<span class=['\"]mod-value['\"]>(.*?)</span>", sub_val, frag, flags=re.S)
    text = re.sub(r"<[^>]+>", "", frag)
    text = re.sub(r"\s+", " ", text).strip()
    return text, vals


def parse_page(html):
    """按 h5 区块切分，抽取各区块内词缀条目。返回 sections 列表。"""
    # 区块锚点
    heads = [(m.start(), re.sub(r"<[^>]+>", "", m.group(1)).strip())
             for m in re.finditer(r'<h5[^>]*>(.*?)</h5>', html, re.S)]
    sections = []
    for i, (pos, title) in enumerate(heads):
        end = heads[i + 1][0] if i + 1 < len(heads) else len(html)
        seg = html[pos:end]
        if not seg or len(seg) > 2_000_000:
            continue
        mods = []
        for mm in re.finditer(r'class="(implicitMod|explicitMod|enchantMod|pseudoMod)"[^>]*>(.*?)</div>', seg, re.S):
            kind = mm.group(1)
            if "x-tmpl-mustache" in mm.group(0):
                continue
            text, vals = clean_mod(mm.group(2))
            if text:
                mods.append({"kind": kind, "text": text, "values": vals})
        # 该区块的基底/物品链接（列表页物品卡区）
        bases = []
        seen = set()
        for bm in re.finditer(r'<a class="whiteitem[^"]*"[^>]*href="([A-Za-z0-9_%-]+)"[^>]*>([^<]{1,40})</a>', seg):
            if bm.group(1) not in seen:
                seen.add(bm.group(1))
                bases.append({"page": bm.group(1), "name": bm.group(2)})
        # 去重词缀（同区块内）
        uniq, seen_t = [], set()
        for m in mods:
            key = m["text"]
            if key not in seen_t:
                seen_t.add(key)
                uniq.append(m)
        sections.append({"title": title, "mods": uniq, "bases": bases})
    return sections


def main():
    limit = None
    if "--limit" in sys.argv:
        limit = int(sys.argv[sys.argv.index("--limit") + 1])
    # 1) 词缀页清单
    html = http_get("https://poedb.tw/cn/Modifiers")
    links = sorted(set(re.findall(r'href="(/cn/[^"#]+)#ModifiersCalc"', html)))
    pages = [l[4:].split("#")[0] for l in links]
    print("[清单] 词缀页 %d 个" % len(pages))
    # 2) 逐页抓取+解析
    all_pages = {}
    todo = pages[:limit] if limit else pages
    for i, base in enumerate(todo):
        cache_f = os.path.join(CACHE, base + ".html")
        if os.path.exists(cache_f) and os.path.getsize(cache_f) > 1000:
            html_b = open(cache_f, encoding="utf-8").read()
            src = "cache"
        else:
            html_b = http_get("https://poedb.tw/cn/" + base)
            open(cache_f, "w", encoding="utf-8").write(html_b)
            src = "net"
        secs = parse_page(html_b)
        n_mods = sum(len(s["mods"]) for s in secs)
        all_pages[base] = {"page": base, "url": "https://poedb.tw/cn/" + base,
                           "fetched_at": time.strftime("%Y-%m-%dT%H:%M:%S"),
                           "sections": secs}
        out_f = os.path.join(PAGES, base + ".json")
        json.dump(all_pages[base], open(out_f, "w", encoding="utf-8"), ensure_ascii=False, indent=1)
        print("[%3d/%d] %-28s %s sections=%d mods=%d" % (i + 1, len(todo), base, src, len(secs), n_mods))
        if src == "net":
            time.sleep(0.2)
    json.dump({"schema_version": "1.0", "source": "poedb.tw/cn",
               "fetched_at": time.strftime("%Y-%m-%dT%H:%M:%S"),
               "pages": all_pages},
              open(os.path.join(OUT, "affixes_all.json"), "w", encoding="utf-8"),
              ensure_ascii=False, indent=1)
    total = sum(len(s["mods"]) for p in all_pages.values() for s in p["sections"])
    print("[完成] 页 %d / 词缀 %d → affixes_all.json" % (len(all_pages), total))


if __name__ == "__main__":
    main()
