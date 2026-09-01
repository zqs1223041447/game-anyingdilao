#!/usr/bin/env python3
"""全树序列化面扫描：对比 vanilla 纯净树与 MOD 树的所有同名 .cs，
找出任何会导致 Unity typetree 布局变化的字段增删/顺序差异。

用法:
    python full_serialize_scan.py <vanilla_tree> <mod_tree>
"""
import os
import sys

sys.path.insert(0, os.path.dirname(os.path.abspath(__file__)))
from serialize_audit import parse, width  # noqa: E402


def walk(root):
    out = {}
    for dp, dns, fns in os.walk(root):
        dns[:] = [d for d in dns if d not in ('obj', 'bin', '.git')]
        for fn in fns:
            if fn.endswith('.cs'):
                p = os.path.join(dp, fn)
                out[os.path.relpath(p, root)] = p
    return out


def main():
    if len(sys.argv) < 3:
        print(__doc__)
        return 2
    vroot, mroot = sys.argv[1], sys.argv[2]
    vf, mf = walk(vroot), walk(mroot)
    common = sorted(set(vf) & set(mf))
    print(f"vanilla {len(vf)} 文件 | mod {len(mf)} 文件 | 共同 {len(common)} 文件")
    print("=" * 72)

    problems, clean = [], 0
    for rel in common:
        try:
            v, m = parse(vf[rel]), parse(mf[rel])
        except Exception as exc:
            problems.append((rel, f"解析失败: {exc}", []))
            continue
        vn = [f[0] for f in v]
        mn = [f[0] for f in m]
        if vn == mn:
            clean += 1
            continue
        only_v = [f for f in v if f[0] not in mn]
        only_m = [f for f in m if f[0] not in vn]
        detail = []
        for n, t, l in only_v:
            detail.append(f"缺(应为vanilla所有) L{l} {t} {n}  ~{width(t)}B")
        for n, t, l in only_m:
            detail.append(f"多(MOD新增public) L{l} {t} {n}  ~{width(t)}B")
        if not only_v and not only_m:
            vpos = {n: i for i, n in enumerate(vn)}
            moved = [n for n in mn if vpos.get(n) != mn.index(n)]
            for n in moved[:6]:
                detail.append(f"顺序变化 {n}: vanilla#{vpos.get(n)} -> mod#{mn.index(n)}")
        if not detail:
            clean += 1
            continue
        dv = sum(width(t) for _, t, _ in v)
        dm = sum(width(t) for _, t, _ in m)
        problems.append((rel, f"vanilla {len(v)}f~{dv}B / mod {len(m)}f~{dm}B / 差 {dm-dv:+d}B", detail))

    if problems:
        print(f"发现 {len(problems)} 个文件存在序列化面差异：\n")
        for rel, summary, detail in problems:
            print(f"[ {rel} ]")
            print(f"    {summary}")
            for d in detail[:10]:
                print(f"      - {d}")
            if len(detail) > 10:
                print(f"      ... 另有 {len(detail)-10} 条")
            print()
    else:
        print("全部一致。")
    print(f"无差异文件 {clean} 个。")
    return 1 if problems else 0


if __name__ == '__main__':
    sys.exit(main())
