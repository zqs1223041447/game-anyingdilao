using Lean.Pool;
using UnityEngine;

public class SK_FlyRound : MonoBehaviour
{
	public SpriteRenderer[] spr;

	public TrailRenderer[] trail;

	public float[] trTime;

	public GameObject[] par;

	public ParticleSystem[] parLoop;

	[Header("=========")]
	public float DotMulti;

	public float LifeTime;

	public float DelDelay;

	private float LifeTimeTmp;

	public float MoveSpeed;

	private float speedTMP;

	[Header("=========")]
	public float starFollowTime;

	public float lerpAngle;

	[Header("=========")]
	public float ExpTimeMin;

	public float ExpTimeMax;

	public GameObject FX;

	[HideInInspector]
	public Dicform dic;

	[HideInInspector]
	public Collider2D MainCOL;

	private bool CanMV;

	private bool canFollow;

	private bool startFollow;

	private bool IsFL;

	private bool CanFX;

	private float FXcd;

	private float starFollowTimeTmp;

	private float range;

	private bool canDAM;

	private float timeA;

	private float timeB;

	private float timeC;

	private float timeD;

	private float timeF;

	private float RDtime;

	private float RDDis;

	[HideInInspector]
	public Transform target;

	private bool initialized;

	private void Awake()
	{
		dic = GetComponent<Dicform>();
		MainCOL = GetComponent<Collider2D>();
	}

	private void OnEnable()
	{
		timeA = 0f;
		timeB = 0f;
		timeC = 0f;
		timeD = 0f;
		timeF = 0f;
		starFollowTimeTmp = Random.Range(starFollowTime * 0.4f, starFollowTime);
		FXcd = Random.Range(ExpTimeMin, ExpTimeMax);
		MainCOL.enabled = false;
		canDAM = false;
		CanFX = true;
		CanMV = false;
		startFollow = false;
		IsFL = false;
		canFollow = false;
		speedTMP = 0f;
		range = 4f;
		initialized = false;
	}

	private void Update()
	{
		if (!CanMV)
		{
			return;
		}
		timeB += Time.deltaTime;
		if (timeB > LifeTimeTmp)
		{
			timeB = 0f;
			TimeStop();
		}
		if (!startFollow)
		{
			timeA += Time.deltaTime;
			if (timeA > starFollowTimeTmp)
			{
				startFollow = true;
				IsFL = true;
				timeA = 0f;
			}
		}
		timeF += Time.deltaTime;
		if (timeF >= FXcd)
		{
			FXcd = Random.Range(ExpTimeMin, ExpTimeMax);
			CanFX = true;
			timeF = 0f;
		}
		if (startFollow)
		{
			if (IsFL)
			{
				FollowMV();
				timeC += Time.deltaTime;
				if (timeC >= RDtime)
				{
					RDtime = Random.Range(0.2f, 0.5f);
					IsFL = false;
					timeC = 0f;
				}
				if (Vector2.Distance(Gun.MousePos, base.transform.position) < 0.7f)
				{
					IsFL = false;
					timeC = 0f;
				}
			}
			else
			{
				SimpleMV();
				timeC += Time.deltaTime;
				if (timeC >= RDtime)
				{
					RDtime = Random.Range(0.2f, 0.5f);
					IsFL = true;
					timeC = 0f;
				}
			}
		}
		else
		{
			SimpleMV();
		}
	}

	public void FollowMV()
	{
		base.transform.position += base.transform.right * (speedTMP * Time.deltaTime);
		base.transform.right = Vector3.Slerp(base.transform.right, Gun.MousePos - base.transform.position, lerpAngle / Vector3.Distance(Gun.MousePos, base.transform.position));
	}

	public void SimpleMV()
	{
		base.transform.Translate(Vector2.right * (speedTMP * Time.deltaTime));
	}

	private void LateUpdate()
	{
		Initialize();
	}

	public void Initialize()
	{
		if (!initialized && CanInitialize())
		{
			initialized = true;
			SetStart();
		}
	}

	private bool CanInitialize()
	{
		Dicform component = GetComponent<Dicform>();
		if (component != null && component.sp == null)
		{
			return false;
		}
		return true;
	}

	public void SetStart()
	{
		if (!dic || !dic.sp)
		{
			return;
		}
		if (par.Length != 0)
		{
			GameObject[] array = par;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].SetActive(value: true);
			}
		}
		if (parLoop.Length != 0)
		{
			ParticleSystem[] array2 = parLoop;
			for (int j = 0; j < array2.Length; j++)
			{
				ParticleSystem.MainModule main = array2[j].main;
				main.loop = true;
			}
		}
		if (trail.Length != 0)
		{
			for (int k = 0; k < trail.Length; k++)
			{
				trail[k].emitting = true;
				trail[k].time = trTime[k];
			}
		}
		if (spr.Length != 0)
		{
			SpriteRenderer[] array3 = spr;
			for (int l = 0; l < array3.Length; l++)
			{
				array3[l].gameObject.SetActive(value: true);
			}
		}
		if (dic.Index == 0)
		{
			if (dic.sp.ZD_time_F == 0f)
			{
				LifeTimeTmp = LifeTime;
			}
			else
			{
				LifeTimeTmp = dic.sp.ZD_time_F;
			}
			if (dic.sp.Speed1 == 0f)
			{
				speedTMP = MoveSpeed * (1f + dic.sp.FlySpeed / 100f);
			}
			else
			{
				speedTMP = dic.sp.Speed1 * (1f + dic.sp.FlySpeed / 100f);
			}
		}
		else
		{
			if (dic.sp.Speed3 == 0f)
			{
				speedTMP = MoveSpeed * (1f + dic.sp.FlySpeed / 100f);
			}
			else
			{
				speedTMP = dic.sp.Speed3 * (1f + dic.sp.FlySpeed / 100f);
			}
			switch (dic.SubType)
			{
			case 0:
				if (dic.sp.ZD_time_S == 0f)
				{
					LifeTimeTmp = LifeTime;
				}
				else
				{
					LifeTimeTmp = dic.sp.ZD_time_S;
				}
				break;
			case 1:
			case 2:
				if (dic.sp.ZD_time_S == 0f)
				{
					LifeTimeTmp = LifeTime;
				}
				else
				{
					LifeTimeTmp = dic.sp.ZD_time_S;
				}
				break;
			}
		}
		MainCOL.enabled = true;
		canDAM = true;
		CanMV = true;
	}

	public void TimeStop()
	{
		if ((bool)FX)
		{
			LeanPool.Spawn(FX, base.transform.position, Quaternion.identity);
		}
		target = null;
		canDAM = false;
		CanMV = false;
		if (par.Length != 0)
		{
			GameObject[] array = par;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].SetActive(value: false);
			}
		}
		if (parLoop.Length != 0)
		{
			ParticleSystem[] array2 = parLoop;
			for (int j = 0; j < array2.Length; j++)
			{
				ParticleSystem.MainModule main = array2[j].main;
				main.loop = false;
			}
		}
		if (spr.Length != 0)
		{
			SpriteRenderer[] array3 = spr;
			for (int k = 0; k < array3.Length; k++)
			{
				array3[k].gameObject.SetActive(value: false);
			}
		}
		if (trail.Length != 0)
		{
			TrailRenderer[] array4 = trail;
			for (int l = 0; l < array4.Length; l++)
			{
				array4[l].emitting = false;
			}
		}
		LeanPool.Despawn(base.gameObject, DelDelay);
	}

	public void Stop()
	{
		target = null;
		canDAM = false;
		CanMV = false;
		if (par.Length != 0)
		{
			GameObject[] array = par;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].SetActive(value: false);
			}
		}
		if (parLoop.Length != 0)
		{
			ParticleSystem[] array2 = parLoop;
			for (int j = 0; j < array2.Length; j++)
			{
				ParticleSystem.MainModule main = array2[j].main;
				main.loop = false;
			}
		}
		if (spr.Length != 0)
		{
			SpriteRenderer[] array3 = spr;
			for (int k = 0; k < array3.Length; k++)
			{
				array3[k].gameObject.SetActive(value: false);
			}
		}
		if (trail.Length != 0)
		{
			TrailRenderer[] array4 = trail;
			for (int l = 0; l < array4.Length; l++)
			{
				array4[l].emitting = false;
			}
		}
		LeanPool.Despawn(base.gameObject, DelDelay);
	}

	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (canDAM && collision.CompareTag("BodyCOL"))
		{
			BodyCOL component = collision.GetComponent<BodyCOL>();
			if (component.peo.CharacterType == 2 && component.peo.em.IsAlive && !component.peo.em.IsJump && !component.peo.em.IsYS)
			{
				component.peo.EM_Set(dic.sp, DotMulti, dic.SubType, Dot_Infect: false, 0, dic.UPDamage);
				if (FX != null && CanFX)
				{
					CanFX = false;
					LeanPool.Spawn(FX, base.transform.position, Quaternion.identity, component.peo.em.yao.transform);
				}
			}
		}
		if (collision.CompareTag("ZoneSK"))
		{
			SK_StromLord component2 = collision.GetComponent<SK_StromLord>();
			if (dic.sp.ZY)
			{
				component2.BuffZD(dic);
			}
			else if (component2.sp.CutSpeedZone > 0 && !dic.CutSpeed)
			{
				speedTMP = speedTMP / 100f * (float)(100 - component2.sp.CutSpeedZone);
				dic.CutSpeed = true;
			}
		}
		if (collision.CompareTag("DoomBall"))
		{
			SK_Doom_Ball component3 = collision.GetComponent<SK_Doom_Ball>();
			if (dic.sp.ZY)
			{
				component3.SetHit(dic, base.transform.right);
			}
			else if (component3.father.sp.TypeDIC_F > 0 && Random.Range(0, 101) < component3.father.sp.TypeDIC_F)
			{
				TimeStop();
			}
		}
		if (collision.CompareTag("Break"))
		{
			collision.GetComponent<BreakOBJ>().Break();
		}
		if (collision.CompareTag("blockFLY"))
		{
			TimeStop();
		}
	}
}
