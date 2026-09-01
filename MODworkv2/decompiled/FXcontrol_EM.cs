using FinkFramework.Runtime.Singleton;
using Lean.Pool;
using Spine.Unity;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

public class FXcontrol_EM : MonoBehaviour
{
	private static readonly int dislove = Shader.PropertyToID("_Dislove");

	private static readonly int flip = Shader.PropertyToID("_Flip");

	private static readonly int mainMix = Shader.PropertyToID("_MainMix");

	private static readonly int mainHue = Shader.PropertyToID("_MainHue");

	private static readonly int mainSat = Shader.PropertyToID("_MainSat");

	private static readonly int mainColor = Shader.PropertyToID("_MainColor");

	private static readonly int disloveColor = Shader.PropertyToID("_DisloveColor");

	private static readonly int alphaColor = Shader.PropertyToID("_AlphaColor");

	private static readonly int mainAlpha = Shader.PropertyToID("_MainAlpha");

	private static readonly int fxSat = Shader.PropertyToID("_FXSat");

	private static readonly int fxColor = Shader.PropertyToID("_FXColor");

	public SkeletonAnimation ani;

	public Light2D lit;

	public AnimationCurve DeadCurve;

	private bool isDead;

	private float Deadtime;

	private byte SDalpha;

	private float JStime;

	public AnimationCurve LightCurve;

	private float JStimeA;

	private float JStimeB;

	private float JStimeC;

	private float timeD;

	private float timeE;

	private bool CanDaoDis;

	private bool CanDaoClear;

	[HideInInspector]
	public SpriteRenderer SD;

	[HideInInspector]
	public Enemy em;

	private bool StartOK;

	[HideInInspector]
	public SKprefab PB;

	private void Awake()
	{
		em = GetComponent<Enemy>();
		SD = base.transform.Find("shadow").gameObject.GetComponent<SpriteRenderer>();
		isDead = false;
		PB = SingletonMonoScope<GameDataManager>.Instance.SKPB;
	}

	private void OnEnable()
	{
		StartOK = false;
		JStimeA = 0f;
		JStimeB = 0f;
		JStimeC = 0f;
		timeD = 0f;
		timeE = 0f;
		CanDaoDis = false;
		CanDaoClear = false;
		JStime = 0f;
		SDalpha = 150;
		SD.color = new Color32(byte.MaxValue, byte.MaxValue, byte.MaxValue, SDalpha);
		Deadtime = 0f;
		isDead = false;
		this.wait(1E-05f, SetStart);
	}

	public void SetStart()
	{
		StartOK = true;
		if (em.IS_Boss)
		{
			lit = base.transform.Find("main/FX yao").gameObject.GetComponent<Light2D>();
			lit.enabled = true;
			lit.intensity = LightCurve.Evaluate(JStimeA);
		}
		if (em.SpineType == 0 && (bool)em.SpineRender)
		{
			em.mpb.SetFloat(dislove, 1f);
			em.mpb.SetInt(flip, em.Flip);
			em.mpb.SetVector(mainMix, em.MainMix);
			em.mpb.SetInt(mainHue, em.MainHue);
			em.mpb.SetFloat(mainSat, em.MainSat);
			em.mpb.SetColor(mainColor, em.MainColor);
			em.mpb.SetColor(disloveColor, em.DisloveColor);
			em.mpb.SetColor(alphaColor, em.AlphaColor);
			em.mpb.SetFloat(mainAlpha, 1f);
			em.mpb.SetFloat(fxSat, 1f);
			em.mpb.SetColor(fxColor, Color.white);
			em.SpineRender.SetPropertyBlock(em.mpb);
		}
	}

	private void Update()
	{
		if (!StartOK)
		{
			return;
		}
		if (!isDead)
		{
			if (!em.IsAlive)
			{
				SetDie(em.DieType);
			}
			return;
		}
		if (em.IS_FS || em.IS_Comp)
		{
			CheckDel();
			return;
		}
		switch (em.DieType)
		{
		case 0:
			CheckDel();
			break;
		case 1:
			CheckDel();
			break;
		case 2:
			BossLit();
			DieSlow();
			break;
		case 3:
			CheckDel();
			break;
		case 4:
			BossLit();
			if (em.CanLie)
			{
				CheckDel();
			}
			else
			{
				DelayDel();
			}
			break;
		case 5:
			BossLit();
			if (em.CanLie)
			{
				CheckDel();
			}
			else
			{
				DelayDel();
			}
			break;
		case 6:
			BossLit();
			if (em.CanLie)
			{
				CheckDel();
			}
			else
			{
				DelayDel();
			}
			break;
		}
	}

	public void BossLit()
	{
		if (!em.IS_Boss)
		{
			return;
		}
		if (lit.intensity > 0f)
		{
			JStimeA += Time.deltaTime;
			JStimeB += Time.deltaTime;
			if (JStimeB > 0.1f)
			{
				JStimeB = 0f;
				if (lit != null)
				{
					lit.intensity = LightCurve.Evaluate(JStimeA);
				}
			}
		}
		else
		{
			lit.intensity = 0f;
		}
	}

	public void DieSlow()
	{
		if (!CanDaoClear)
		{
			if (DeadCurve.Evaluate(Deadtime) > 0f)
			{
				Deadtime += Time.deltaTime;
				JStimeC += Time.deltaTime;
				if (JStimeC > 0.08f)
				{
					em.mpb?.SetFloat(dislove, DeadCurve.Evaluate(Deadtime));
					em.SpineRender.SetPropertyBlock(em.mpb);
					JStimeC = 0f;
				}
				if (SDalpha != 0)
				{
					JStime += Time.deltaTime;
					if (JStime > 0.15f)
					{
						SDalpha -= 15;
						SD.color = new Color32(byte.MaxValue, byte.MaxValue, byte.MaxValue, SDalpha);
						JStime = 0f;
					}
				}
			}
			else
			{
				em.mpb.SetFloat(dislove, 0f);
				em.SpineRender.SetPropertyBlock(em.mpb);
				CanDaoClear = true;
			}
			return;
		}
		timeE += Time.deltaTime;
		if ((double)timeE > 0.3)
		{
			timeE = 0f;
			if (em.yao.transform.childCount == 0 && em.body.transform.childCount == 0 && em.head.transform.childCount == 0)
			{
				LeanPool.Despawn(this);
			}
		}
	}

	public void DelayDel()
	{
		if (!CanDaoDis)
		{
			timeD += Time.deltaTime;
			if (timeD > em.DieDelay)
			{
				timeD = 0f;
				CanDaoDis = true;
			}
			return;
		}
		if (!CanDaoClear)
		{
			if (DeadCurve.Evaluate(Deadtime) > 0f)
			{
				Deadtime += Time.deltaTime;
				JStimeC += Time.deltaTime;
				if (JStimeC > 0.08f)
				{
					em.mpb?.SetFloat(mainAlpha, DeadCurve.Evaluate(Deadtime));
					em.SpineRender.SetPropertyBlock(em.mpb);
					JStimeC = 0f;
				}
				if (SDalpha != 0)
				{
					JStime += Time.deltaTime;
					if (JStime > 0.15f)
					{
						SDalpha -= 15;
						SD.color = new Color32(byte.MaxValue, byte.MaxValue, byte.MaxValue, SDalpha);
						JStime = 0f;
					}
				}
			}
			else
			{
				CanDaoClear = true;
			}
			return;
		}
		timeE += Time.deltaTime;
		if ((double)timeE > 0.3)
		{
			timeE = 0f;
			if (em.yao.transform.childCount == 0 && em.body.transform.childCount == 0 && em.head.transform.childCount == 0)
			{
				LeanPool.Despawn(this);
			}
		}
	}

	public void CheckDel()
	{
		timeE += Time.deltaTime;
		if ((double)timeE > 0.3)
		{
			timeE = 0f;
			if (em.yao.transform.childCount == 0 && em.body.transform.childCount == 0 && em.head.transform.childCount == 0)
			{
				LeanPool.Despawn(this);
			}
		}
	}

	public void SetDie(int A)
	{
		if (em.IS_FS || em.IS_Comp)
		{
			FSdie();
		}
		else if (em.EnemyType == 100)
		{
			this.wait(1f, delegate
			{
				LeanPool.Despawn(this);
			});
		}
		else
		{
			switch (A)
			{
			case 0:
				EXP();
				break;
			case 2:
				Dis();
				break;
			case 4:
				if (em.CanLie)
				{
					Lie();
				}
				break;
			case 5:
				if (em.CanLie)
				{
					Lie();
				}
				else if (Random.Range(0, 101) < 20)
				{
					Dao();
				}
				break;
			case 6:
				if (em.CanLie)
				{
					Lie();
				}
				else
				{
					Dao();
				}
				break;
			}
		}
		isDead = true;
	}

	public void EXP()
	{
		object obj = em.DiePos switch
		{
			0 => LeanPool.Spawn(PB.DieSP[em.Die_Index].OBJ[em.DieColor], em.transform.position, Quaternion.identity).GetComponent<Die_FX>(), 
			1 => LeanPool.Spawn(PB.DieSP[em.Die_Index].OBJ[em.DieColor], em.body.transform.position, Quaternion.identity).GetComponent<Die_FX>(), 
			2 => LeanPool.Spawn(PB.DieSP[em.Die_Index].OBJ[em.DieColor], em.head.transform.position, Quaternion.identity).GetComponent<Die_FX>(), 
			_ => LeanPool.Spawn(PB.DieSP[em.Die_Index].OBJ[em.DieColor], em.transform.position, Quaternion.identity).GetComponent<Die_FX>(), 
		};
		((Die_FX)obj).type = em.DiePos;
		((Die_FX)obj).SPtype = em.SPtype;
		if (em.SpineType == 0)
		{
			em.mpb.SetFloat(mainAlpha, 0f);
			em.SpineRender.SetPropertyBlock(em.mpb);
		}
		em.canvas.alpha = 0f;
		SD.color = new Color32(byte.MaxValue, byte.MaxValue, byte.MaxValue, 0);
		if (em.IS_Boss && (bool)lit)
		{
			lit.intensity = 0f;
		}
	}

	public void Dis()
	{
		switch (em.DiePos)
		{
		case 0:
			this.wait(em.DieFX_TimeDelay, delegate
			{
				LeanPool.Spawn(PB.DieSP[em.Die_Index].OBJ[em.DieColor], em.body.transform.position, Quaternion.identity, em.body.transform);
			});
			break;
		case 1:
			this.wait(em.DieFX_TimeDelay, delegate
			{
				LeanPool.Spawn(PB.DieSP[em.Die_Index].OBJ[em.DieColor], em.yao.transform.position, Quaternion.identity, em.body.transform);
			});
			break;
		case 2:
			this.wait(em.DieFX_TimeDelay, delegate
			{
				LeanPool.Spawn(PB.DieSP[em.Die_Index].OBJ[em.DieColor], em.head.transform.position, Quaternion.identity, em.body.transform);
			});
			break;
		}
		em.canvas.alpha = 0f;
	}

	public void Dao()
	{
		object obj = em.DiePos switch
		{
			0 => LeanPool.Spawn(PB.DieSP[em.Die_Index].OBJ[em.DieColor], em.transform.position, Quaternion.identity).GetComponent<Die_FX>(), 
			1 => LeanPool.Spawn(PB.DieSP[em.Die_Index].OBJ[em.DieColor], em.body.transform.position, Quaternion.identity).GetComponent<Die_FX>(), 
			2 => LeanPool.Spawn(PB.DieSP[em.Die_Index].OBJ[em.DieColor], em.head.transform.position, Quaternion.identity).GetComponent<Die_FX>(), 
			_ => LeanPool.Spawn(PB.DieSP[em.Die_Index].OBJ[em.DieColor], em.transform.position, Quaternion.identity).GetComponent<Die_FX>(), 
		};
		((Die_FX)obj).type = em.DiePos;
		((Die_FX)obj).SPtype = em.SPtype;
		em.canvas.alpha = 0f;
	}

	public void Lie()
	{
		object obj = em.LiePos switch
		{
			0 => LeanPool.Spawn(PB.DieSP[em.Lie_Index].OBJ[em.DieColor], em.transform.position, Quaternion.identity).GetComponent<Die_FX>(), 
			1 => LeanPool.Spawn(PB.DieSP[em.Lie_Index].OBJ[em.DieColor], em.yao.transform.position, Quaternion.identity).GetComponent<Die_FX>(), 
			2 => LeanPool.Spawn(PB.DieSP[em.Lie_Index].OBJ[em.DieColor], em.head.transform.position, Quaternion.identity).GetComponent<Die_FX>(), 
			_ => LeanPool.Spawn(PB.DieSP[em.Lie_Index].OBJ[em.DieColor], em.transform.position, Quaternion.identity).GetComponent<Die_FX>(), 
		};
		((Die_FX)obj).type = em.LiePos;
		((Die_FX)obj).SPtype = em.SPtype;
		if (em.SpineType == 0)
		{
			em.mpb.SetFloat(mainAlpha, 0f);
			em.SpineRender.SetPropertyBlock(em.mpb);
		}
		em.canvas.alpha = 0f;
		SD.color = new Color32(byte.MaxValue, byte.MaxValue, byte.MaxValue, 0);
	}

	public void FSdie()
	{
		LeanPool.Spawn(PB.DieSP[em.FSDie_Index].OBJ[em.MainElement], em.yao.transform.position, Quaternion.identity);
		if (em.SpineType == 0)
		{
			em.mpb.SetFloat(mainAlpha, 0f);
			em.SpineRender.SetPropertyBlock(em.mpb);
		}
		em.canvas.alpha = 0f;
		SD.color = new Color32(byte.MaxValue, byte.MaxValue, byte.MaxValue, 0);
	}
}
