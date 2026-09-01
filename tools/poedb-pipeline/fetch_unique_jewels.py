# -*- coding: utf-8 -*-
"""
fetch_unique_jewels.py — POE 传奇珠宝抓取（任务 B：真实抓取版）
================================================================
数据源路线（已探明）：
  1. https://poedb.tw/cn/Jewels → 区块「珠宝 传奇 /112」内 class="UniqueItems" 链接 = 全部传奇珠宝
  2. 逐颗详情页（34KB 小页）：H3=中文名、title=「中文名 基底 - 流亡编年史」、
     h5 区块含「<名> Attr /N」（属性/限制）与效果文本（explicitMod div，数值内联）
输出：
  data/poedb/unique_jewels/cache/*.html
  data/poedb/unique_jewels/unique_jewels.json   全量（含解析好的效果/属性/来源）
礼貌抓取：200ms 间隔，重试 2 次。依赖：仅 Python3 标准库。
用法：python tools/poedb-pipeline/fetch_unique_jewels.py [--limit N]
"""
import json, os, re, sys, time, urllib.request

try:
    sys.stdout.reconfigure(encoding="utf-8")
except Exception:
    pass

HERE = os.path.dirname(os.path.abspath(__file__))
ROOT = os.path.abspath(os.path.join(HERE, "..", ".."))
OUT = os.path.join(ROOT, "data", "poedb", "unique_jewels")
CACHE = os.path.join(OUT, "cache")
for d in (OUT, CACHE):
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
    frag = re.sub(r'<span class="ndash">[^<]*</span>', '—', html_frag)
    text = re.sub(r"<[^>]+>", "", frag)
    return re.sub(r"\s+", " ", text).strip()


def strip_tags(s):
    return re.sub(r"\s+", " ", re.sub(r"<[^>]+>", "", s)).strip()


def clean_effect(text):
    """清洗效果文本：去内嵌数据尾巴/版本历史/接口噪音。"""
    t = text
    t = re.sub(r"local jewel effect base radius \[(\d+)\]", "", t).strip()
    t = re.sub(r"(?i)(Version history|VersionChanges|Version Changes).*", "", t).strip()
    t = re.sub(r"https?://\S+", "", t).strip()
    t = re.sub(r"^\d+\.\d+\.\d+.*$", "", t).strip()
    t = re.sub(r"\s{2,}", " ", t)
    return t


def parse_jewel(html, page):
    d = {"page": page, "url": "https://poedb.tw/cn/" + page}
    t = re.search(r"<title>([^<]+)</title>", html)
    if t:
        d["title"] = t.group(1).split(" - ")[0].strip()
    h3 = re.search(r"<h3[^>]*>(.*?)</h3>", html, re.S)
    if h3:
        d["name_cn"] = strip_tags(h3.group(1))
    heads = [(m.start(), strip_tags(m.group(1)))
             for m in re.finditer(r"<h5[^>]*>(.*?)</h5>", html, re.S)]
    sections = []
    for i, (pos, title) in enumerate(heads):
        end = heads[i + 1][0] if i + 1 < len(heads) else len(html)
        seg = html[pos:end]
        mods = [clean_mod(m.group(1))
                for m in re.finditer(r'class="(?:explicitMod|implicitMod|enchantMod)"[^>]*>(.*?)</div>', seg, re.S)
                if clean_mod(m.group(1))]
        mods = list(dict.fromkeys(mods))
        sections.append({"title": title, "mods": mods})
    d["sections"] = sections
    # 结构化字段：限制/属性区（<名> Attr）、效果区（title 全等于效果文本的 h5 与 explicitMod）
    attr = [s for s in sections if re.search(r"\bAttr\b", s["title"])]
    d["attributes"] = attr[0]["mods"] if attr else []
    unique_sec = [s for s in sections if "传奇物品" in s["title"]]
    d["acquisition"] = unique_sec[0]["mods"] if unique_sec else []
    # 效果 = 属性区之外的独立 h5（POEDB 把传奇效果作为 h5 标题列出）+ 全部 explicitMod
    effects = []
    for s in sections:
        if s in attr or s in unique_sec or s["title"] in ("Sites", "News", "About Site", "Community"):
            continue
        if re.search(r"固定|Recipe|Acquisition|Alternate|商店", s["title"]):
            continue
        if len(s["title"]) <= 120 and s["title"] not in (d.get("name_cn"), d["page"].replace("_", " ")):
            e = clean_effect(s["title"])
            if e:
                effects.append(e)
        for m in s["mods"]:
            e = clean_effect(m)
            if e and e not in effects and len(e) < 300:
                effects.append(e)
    d["effects"] = effects
    return d


def main():
    limit = None
    if "--limit" in sys.argv:
        limit = int(sys.argv[sys.argv.index("--limit") + 1])
    html = http_get("https://poedb.tw/cn/Jewels")
    # 「珠宝 传奇 /112」区块：class="UniqueItems UniqueItem" + data-hover + href（图/文两个链接同 href）
    links = []
    seen = set()
    for m in re.finditer(r'<a class="UniqueItems[^"]*"[^>]*?href="([A-Za-z0-9_%-]+)"', html):
        page_id = m.group(1)
        if page_id in seen:
            continue
        seen.add(page_id)
        name_m = re.search(r'href="%s"[^>]*>([^<>]+)</a>' % re.escape(page_id), html)
        name = strip_tags(name_m.group(1)) if name_m else page_id.replace("_", " ")
        links.append((page_id, name))
    print("[清单] 传奇珠宝 %d 颗" % len(links))
    todo = links[:limit] if limit else links
    out = []
    for i, (page, name) in enumerate(todo):
        cache_f = os.path.join(CACHE, page + ".html")
        if os.path.exists(cache_f) and os.path.getsize(cache_f) > 1000:
            h = open(cache_f, encoding="utf-8").read()
            src = "cache"
        else:
            h = http_get("https://poedb.tw/cn/" + page)
            open(cache_f, "w", encoding="utf-8").write(h)
            src = "net"
        d = parse_jewel(h, page)
        d["name_list"] = name
        out.append(d)
        if src == "net":
            time.sleep(0.2)
        if (i + 1) % 10 == 0 or i + 1 == len(todo):
            print("[%3d/%d] %s %s（效果 %d 条）" % (i + 1, len(todo), name, src, len(d["effects"])))
    json.dump({"schema_version": "1.0", "source": "poedb.tw/cn/Jewels",
               "fetched_at": time.strftime("%Y-%m-%dT%H:%M:%S"),
               "count": len(out), "jewels": out},
              open(os.path.join(OUT, "unique_jewels.json"), "w", encoding="utf-8"),
              ensure_ascii=False, indent=1)
    print("[完成] %d 颗 → unique_jewels.json" % len(out))


if __name__ == "__main__":
    main()
