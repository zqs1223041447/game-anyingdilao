using FMODUnity;
using Lean.Pool;
using UnityEngine;

public class SK_MountFire : MonoBehaviour
{
	public string SoundA;

	public ParticleSystem[] parOne;

	public ParticleSystem[] parLoop;

	[Header("=========")]
	public GameObject OBJ;

	public float DelDelay;

	[HideInInspector]
	public Dicform dic;

	[HideInInspector]
	public Transform trans;

	private bool canPen;

	private float timeA;

	private float timeB;

	private float spMin;

	private float spMax;

	private int CountMulti;

	private float FStime;

	private void Awake()
	{
		dic = GetComponent<Dicform>();
		trans = base.transform.Find("black").GetComponent<Transform>();
	}

	private void Start()
	{
	}

	private void OnEnable()
	{
		timeA = 0f;
		timeB = 0f;
		canPen = false;
		this.wait(0.0001f, FaShe);
	}

	private void Update()
	{
		if (!canPen)
		{
			return;
		}
		timeA += Time.deltaTime;
		if (timeA >= FStime)
		{
			for (int i = 0; i < CountMulti; i++)
			{
				Dicform component = LeanPool.Spawn(OBJ, trans.position, Quaternion.identity).GetComponent<Dicform>();
				component.speed = Random.Range(spMin, spMax);
				component.dic = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), 0f);
				component.sp = dic.sp;
				component.SetCount(dic.sp.ZY);
				component.SubType = dic.SubType;
				component.Index = dic.Index + 1;
				timeA = 0f;
			}
		}
		timeB += Time.deltaTime;
		if (timeB > dic.sp.BuffTime)
		{
			Stop();
			timeB = 0f;
		}
	}

	public void FaShe()
	{
		canPen = true;
		if (dic.Index == 0)
		{
			CountMulti = dic.sp.CountMulti;
			FStime = dic.sp.FStime1;
			spMin = dic.sp.Speed1;
			spMax = dic.sp.Speed2;
		}
		else
		{
			CountMulti = dic.sp.CountMulti;
			FStime = dic.sp.FStime2;
			spMin = dic.sp.Speed3;
			spMax = dic.sp.Speed4;
		}
		if (parOne.Length != 0)
		{
			for (int i = 0; i < parOne.Length; i++)
			{
				ParticleSystem.MainModule main = parOne[i].main;
				main.startLifetime = dic.sp.BuffTime + 0.5f;
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
		if (dic.sp.Layer_SubA == dic.Index && dic.SubType == 0 && dic.sp.DamageA > 0f)
		{
			for (int k = 0; k < dic.sp.Count_AB; k++)
			{
				Dicform component = LeanPool.Spawn(OBJ, trans.position, Quaternion.identity).GetComponent<Dicform>();
				component.speed = Random.Range(spMin, spMax);
				component.dic = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), 0f);
				component.sp = dic.sp;
				component.SetCount(dic.sp.ZY);
				component.SubType = 1;
				component.Index = dic.Index + 1;
			}
		}
		if (SoundA != null)
		{
			RuntimeManager.PlayOneShot(SoundA, base.transform.position);
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
		canPen = false;
		this.wait(DelDelay, delegate
		{
			LeanPool.Despawn(this);
		});
	}
}
