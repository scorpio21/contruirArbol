import itertools, sys, json; sys.path.insert(0,'/home/claude/gl_solver')
from gl_solver import *
exec(open('/home/claude/gl_solver/run2.py').read().split("user=from_rc")[0].split("from gl_solver import *")[1])
user=from_rc("s.z.10.16.17.18.19.1a.1b.1c.1d.1e.1f.1g.1h.1i.2s.2t.2u.2v.2w.48.49.6q.6r.6s.6u.6v.72.73.9b.9c.9d.9e.9g.9k.9l.fo.h0")
B=set(json.load(open('base_ids.json')))
PA=Params(a_cast=0,a_cdr=0,a_cost=0,time_limit=60)      # solo daño + maná
PB=Params()                                              # base (con cdr/coste)
evA,idsA,glA=optimize(PA,start_ids=B,iters=4,verbose=False)
json.dump(sorted(idsA),open('A_ids.json','w'))
print('A valido:',valid(PA,idsA),'B valido:',valid(PB,B),'tu build valido:',valid(PB,user))
rows=[('tu build',user),('A  (daño+maná)',idsA),('B  (+Observatorio)',B)]
for wname,P in [('pesos A (sin cdr/coste)',PA),('pesos B (con cdr/coste)',PB)]:
    print('\n--',wname)
    base_s=best_glyphs(P,user)[0]['score']
    for tag,ids in rows:
        e,gl=best_glyphs(P,ids)
        print(f"  {tag:<20} score={e['score']:.3f} ({100*(math.exp(e['score']-base_s)-1):+.0f}% vs tu build)  D={e['D']:.0f} mana={e['mana']:.0f} regen={e['regen']:.1f} INT={e['INT']:.0f} mag%={e['magicPct']:.0f} cdr={e['cdr']:.0f} cost={e['cost']:.0f}  glifos={gl}")
print('\nA nodos nuevos vs tu build:', sorted(idsA-user)); print('A nodos que pierdes de tu build:', sorted(user-idsA))
print('\nrc A =',to_rc(idsA)); print('rc B =',to_rc(B))
print('\nNotables de A:'); print(describe(PA,idsA,best_glyphs(PA,idsA)[1]))
