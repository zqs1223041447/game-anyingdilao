using System.Collections.Generic;
using FMODUnity;
using FinkFramework.Runtime.Singleton;
using Lean.Pool;
using UnityEngine;

public class SK_DropArrowRain : MonoBehaviour
{
	public string SoundA;

	public GameObject OBJ;

	public ParticleSystem[] parOne;

	public ParticleSystem[] parLoop;

	[Header("=========")]
	public float DelDelay;

	public float FaSheTime;

	public float range;

	public int number;

	public float size;

	[Header("=========")]
	public bool Single;

	public float DotMulti;

	public bool HasLight;

	private bool CloseLit;

	private List<Vector2> allPos = new List<Vector2>();

	[HideInInspector]
	public LightEXP litEXP;

	[HideInInspector]
	public Dicform dic;

	private float timeA;

	private float timeB;

	private float DamageTime;

	private float ATtime;

	private bool CanAT;

	private int FaSheCount;

	private int numberTmp;

	private void Awake()
	{
		dic = GetComponent<Dicform>();
		if (HasLight)
		{
			litEXP = GetComponent<LightEXP>();
			litEXP.UseSkillTime = true;
		}
	}

	private void Start()
	{
	}

	private void OnEnable()
	{
		timeA = 0f;
		timeB = 0f;
		ATtime = 0f;
		CanAT = false;
		CloseLit = true;
		FaSheCount = 0;
		this.wait(1E-05f, SetStart);
	}

	private void Update()
	{
		if (!SingletonMonoScope<GameDataManager>.HasInstance || !CanAT)
		{
			return;
		}
		if (!Single)
		{
			ATtime += Time.deltaTime;
			if (ATtime >= FaSheTime)
			{
				ATtime = 0f;
				LeanPool.Spawn(OBJ, new Vector3(base.transform.position.x + allPos[FaSheCount].x, base.transform.position.y + allPos[FaSheCount].y, 0f), Quaternion.identity);
				LeanPool.Spawn(OBJ, new Vector3(base.transform.position.x + allPos[number - 1 - FaSheCount].x, base.transform.position.y + allPos[number - 1 - FaSheCount].y, 0f), Quaternion.identity);
				FaSheCount++;
			}
			DamageTime += Time.deltaTime;
			if (DamageTime >= 0.5f)
			{
				EmptyCOL component = LeanPool.Spawn(SingletonMonoScope<GameDataManager>.Instance.SKPB.EmptyCol, base.transform.position, Quaternion.identity).GetComponent<EmptyCOL>();
				Dicform component2 = component.GetComponent<Dicform>();
				component2.sp = dic.sp;
				component2.SetCount(dic.sp.ZY);
				component2.SubType = dic.SubType;
				component.size = size;
				component.Body = true;
				component.DotMulti = DotMulti;
				component.lifeTime = 0.1f;
				DamageTime = 0f;
			}
		}
		timeA += Time.deltaTime;
		if (timeA >= dic.sp.BuffTime && (bool)dic)
		{
			timeA = 0f;
			Stop();
		}
		if (CloseLit)
		{
			return;
		}
		timeB += Time.deltaTime;
		if (timeB >= dic.sp.BuffTime * 5f / 6f)
		{
			if (HasLight)
			{
				litEXP.LightDown = true;
				CloseLit = true;
			}
			timeB = 0f;
		}
	}

	public void SetStart()
	{
		if (parOne.Length != 0)
		{
			for (int i = 0; i < parOne.Length; i++)
			{
				ParticleSystem.MainModule main = parOne[i].main;
				main.startLifetime = dic.sp.BuffTime + dic.sp.BuffTime / 6f;
				parOne[i].Play();
			}
		}
		if (parLoop.Length != 0)
		{
			for (int j = 0; j < parLoop.Length; j++)
			{
				ParticleSystem.MainModule main2 = parLoop[j].main;
				main2.loop = true;
			}
		}
		CanAT = true;
		CloseLit = false;
		if (Single)
		{
			numberTmp = number;
			Drop();
			for (int k = 0; k < numberTmp; k++)
			{
				LeanPool.Spawn(OBJ, new Vector3(base.transform.position.x + allPos[FaSheCount].x, base.transform.position.y + allPos[FaSheCount].y, 0f), Quaternion.identity);
				FaSheCount++;
			}
			this.wait(0.16f, SingleAT);
		}
		else
		{
			numberTmp = Mathf.FloorToInt(dic.sp.BuffTime / FaSheTime) + 20;
			Drop();
		}
		if (SoundA != null)
		{
			RuntimeManager.PlayOneShot(SoundA, base.transform.position);
		}
	}

	public void Drop()
	{
		allPos.Clear();
		int num = numberTmp;
		Vector2[] array = new Vector2[4]
		{
			new Vector2(range, range),
			new Vector2(range, 0f - range),
			new Vector2(0f - range, range),
			new Vector2(0f - range, 0f - range)
		};
		for (int i = 0; i < num; i++)
		{
			Vector2 vector = Random.insideUnitCircle * range;
			float x = Mathf.Abs(vector.x);
			float y = Mathf.Abs(vector.y);
			int num2 = i % array.Length;
			Vector2 item = new Vector2(x, y) * array[num2];
			allPos.Add(item);
		}
	}

	public void SingleAT()
	{
		if (SingletonMonoScope<GameDataManager>.HasInstance)
		{
			EmptyCOL component = LeanPool.Spawn(SingletonMonoScope<GameDataManager>.Instance.SKPB.EmptyCol, base.transform.position, Quaternion.identity).GetComponent<EmptyCOL>();
			Dicform component2 = component.GetComponent<Dicform>();
			component2.sp = dic.sp;
			component2.SetCount(dic.sp.ZY);
			component2.SubType = dic.SubType;
			component.size = size;
			component.Body = true;
			component.DotMulti = DotMulti;
			component.IsGround = false;
			Stop();
		}
	}

	public void Stop()
	{
		if (parLoop.Length != 0)
		{
			for (int i = 0; i < parLoop.Length; i++)
			{
				ParticleSystem.MainModule main = parLoop[i].main;
				main.loop = false;
			}
		}
		CanAT = false;
		this.wait(DelDelay, delegate
		{
			LeanPool.Despawn(this);
		});
	}
}
