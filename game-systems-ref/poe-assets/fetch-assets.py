# -*- coding: utf-8 -*-
"""
POEDB MTX 特效素材抓取工具 V2（fetch-icons.ps1 的替代重写版）
=============================================================
旧脚本失败根因：直接猜测 CDN 路径 `Art/2DItems/Effects/<名>Effect.webp`，
不存在的路径 CDN 返回 403（非 404），18 连败后中止。

本工具的正确路线（已实测验证）：
  1. 抓 https://poedb.tw/cn/Microtransactions 全页 → 提取全部 `*_Effect` 相对链接 = 真实 MTX 特效目录（约 1389 条）
  2. manifest.json 的 poeCounterpart 与目录做词级匹配（排除 Portal/Pet/外观类误配）
  3. 逐个抓命中技能的 MTX 页 → 提取真实图标 URL（2DItems/Effects 路径）+ YouTube 视频 ID + 中文名
  4. 下载图标与视频缩略图到 skills/（UA + Referer 伪装浏览器，CDN 对真实路径返回 200）
  5. 回写 manifest.json（status/mtxEffect/mtxPage/iconUrl/youtubeId）+ 生成 fetch-report.json

用法：python fetch-assets.py          （可重复运行，已下载文件自动跳过，断点续传）
依赖：仅 Python 3 标准库。礼貌抓取：每次请求间隔 200ms，重试 2 次。
"""
import json, os, re, sys, time, urllib.request, urllib.parse

try:
    sys.stdout.reconfigure(encoding="utf-8")
except Exception:
    pass

HERE = os.path.dirname(os.path.abspath(__file__))
CACHE = os.path.join(HERE, "cache")
SKILLS = os.path.join(HERE, "skills")
UA = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/126.0 Safari/537.36"
# 这些词出现在 MTX 名里说明是传送门/宠物/外观类，不是技能特效
EXCLUDE = ("Portal", "Pet", "Footprints", "Wings", "Cloak", "Helmet", "Gloves",
           "Boots", "Armour", "Armor", "Frame", "Attachment", "Hideout", "Stash",
           "Portrait", "Character", "Bundle")
for d in (CACHE, SKILLS):
    os.makedirs(d, exist_ok=True)


def http_get(url, referer=None, binary=False, tries=3):
    last = None
    for i in range(tries):
        try:
            req = urllib.request.Request(url, headers={
                "User-Agent": UA,
                "Accept": "*/*",
                **({"Referer": referer} if referer else {}),
            })
            with urllib.request.urlopen(req, timeout=25) as r:
                data = r.read()
                return data if binary else data.decode("utf-8", "ignore")
        except Exception as e:
            last = e
            time.sleep(0.5 * (i + 1))
    raise last


def get_catalog():
    """MTX 全目录：缓存优先，否则抓 Microtransactions 总页提取 *_Effect 相对链接。"""
    cache_file = os.path.join(CACHE, "mtx_catalog.txt")
    if os.path.exists(cache_file) and os.path.getsize(cache_file) > 10000:
        return [l.strip() for l in open(cache_file, encoding="utf-8") if l.strip()]
    html = http_get("https://poedb.tw/cn/Microtransactions")
    rel = sorted(set(u for u in re.findall(r'href="([^"]+)"', html)
                     if not u.startswith(("http", "/", "#")) and u.strip()))
    eff = sorted(set(u for u in rel if u.endswith("_Effect")))
    open(cache_file, "w", encoding="utf-8").write("\n".join(eff))
    print("[目录] MTX 技能特效页 %d 条" % len(eff))
    return eff


def match(effects, counterpart):
    """词级匹配：精确 <Name>_Effect > 全词包含；排除外观/传送门类。"""
    target = re.sub(r"[^A-Za-z0-9 ]", "", counterpart).strip().replace(" ", "_")
    if not target:
        return None, []
    lo = target.lower()

    def ok(c):
        return not any(w.lower() in c.lower() for w in EXCLUDE)

    exact = [c for c in effects if c.lower() == lo + "_effect" and ok(c)]
    if exact:
        return exact[0], []
    word_hits = [c for c in effects
                 if c.lower().endswith("_effect") and ok(c)
                 and all(("_%s_" % w) in (c.lower() + "_") for w in lo.split("_"))]
    if word_hits:
        word_hits.sort(key=len)
        return word_hits[0], word_hits[1:6]
    return None, []


def parse_effect_page(name):
    """抓 MTX 页 → (图标URL, youtube_id, 中文标题)。"""
    cache_html = os.path.join(CACHE, name + ".html")
    if os.path.exists(cache_html):
        html = open(cache_html, encoding="utf-8", errors="ignore").read()
    else:
        html = http_get("https://poedb.tw/cn/" + name, referer="https://poedb.tw/cn/Microtransactions")
        open(cache_html, "w", encoding="utf-8", errors="ignore").write(html)
        time.sleep(0.2)
    icon = None
    m = re.search(r'https://cdn\.poedb\.tw/image/[^"]*2DItems/Effects/[^"]+?\.(?:webp|png|jpg)', html)
    if not m:
        m = re.search(r'https://cdn\.poedb\.tw/image/[^"]*Effect[^"]+?\.(?:webp|png|jpg)', html)
    if m:
        icon = m.group(0)
    yt = None
    m2 = re.search(r"(?:youtube\.com/watch\?v=|youtu\.be/|img\.youtube\.com/vi/|youtube\.com/embed/)([A-Za-z0-9_-]{11})", html)
    if m2:
        yt = m2.group(1)
    t = re.search(r"<h1[^>]*>([^<]+)</h1>", html) or re.search(r'<meta property="og:title" content="([^"]+)"', html)
    title = t.group(1).strip() if t else name
    return icon, yt, title


def download(url, path, referer="https://poedb.tw/"):
    if os.path.exists(path) and os.path.getsize(path) > 200:
        return "skip"
    data = http_get(url, referer=referer, binary=True)
    open(path, "wb").write(data)
    return "ok"


def main():
    effects = get_catalog()
    manifest = json.load(open(os.path.join(HERE, "manifest.json"), encoding="utf-8"))
    report = {"startedAt": time.strftime("%Y-%m-%d %H:%M:%S"), "results": []}
    n_ok = n_nomtx = n_fail = 0

    for s in manifest["skills"]:
        en = str(s.get("localEN", "")).strip()
        cn = str(s.get("localCN", "")).strip()
        pc = str(s.get("poeCounterpart", "")).strip()
        best, alts = match(effects, pc)
        entry = {"localEN": en, "localCN": cn, "poeCounterpart": pc}

        if not best:
            s["status"] = "no-direct-mtx"
            s["mtxNote"] = "POE 目录无直接技能特效（buff/被动/同伴类或映射猜测无对应）"
            n_nomtx += 1
            entry["status"] = "no-direct-mtx"
            report["results"].append(entry)
            print("[无MTX] %s <- %s" % (cn, pc))
            continue
        try:
            icon_url, yt, title = parse_effect_page(best)
        except Exception as e:
            s["status"] = "fetch-failed"
            n_fail += 1
            entry.update({"status": "fetch-failed", "error": str(e)})
            report["results"].append(entry)
            print("[页失败] %s %s: %s" % (cn, best, e))
            continue

        base = re.sub(r'[\\/:*?"<>|]', "_", en or cn)
        got = []
        if icon_url:
            ext = os.path.splitext(icon_url)[1] or ".webp"
            p = os.path.join(SKILLS, "%s__%s%s" % (base, best, ext))
            try:
                got.append("icon:" + download(icon_url, p))
            except Exception as e:
                got.append("icon-fail:" + str(e)[:40])
        if yt:
            p = os.path.join(SKILLS, "%s__%s_video_hq.jpg" % (base, best))
            try:
                got.append("thumb:" + download("https://img.youtube.com/vi/%s/hqdefault.jpg" % yt, p,
                                               referer="https://www.youtube.com/"))
            except Exception as e:
                got.append("thumb-fail:" + str(e)[:40])

        s.update({
            "status": "ok",
            "mtxEffect": best,
            "mtxPage": "https://poedb.tw/cn/" + best,
            "mtxTitleCN": title,
            "iconUrl": icon_url or "",
            "youtubeId": yt or "",
            "alts": alts,
        })
        n_ok += 1
        entry.update({"status": "ok", "mtxEffect": best, "icon": bool(icon_url), "youtube": yt})
        report["results"].append(entry)
        print("[OK] %s <- %s (%s)" % (cn, best, ",".join(got) or "无素材"))
        time.sleep(0.2)

    manifest["meta"]["lastFetchAt"] = report["startedAt"]
    manifest["meta"]["fetchTool"] = "fetch-assets.py V2（页面抓取路线，替换猜测 URL 的 fetch-icons.ps1）"
    json.dump(manifest, open(os.path.join(HERE, "manifest.json"), "w", encoding="utf-8"),
              ensure_ascii=False, indent=2)
    report["summary"] = {"ok": n_ok, "noDirectMtx": n_nomtx, "failed": n_fail, "total": len(manifest["skills"])}
    json.dump(report, open(os.path.join(HERE, "fetch-report.json"), "w", encoding="utf-8"),
              ensure_ascii=False, indent=1)
    print("[完成] ok=%d noDirectMtx=%d failed=%d / %d" % (n_ok, n_nomtx, n_fail, len(manifest["skills"])))


if __name__ == "__main__":
    main()
