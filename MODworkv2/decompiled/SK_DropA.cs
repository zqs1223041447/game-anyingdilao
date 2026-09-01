using FMODUnity;
using Lean.Pool;
using UnityEngine;

public class SK_DropA : MonoBehaviour
{
	public string SoundDP;

	public string SoundCD;

	public GameObject EXP;

	public GameObject FX;

	public TrailRenderer[] trail;

	public float[] trTime;

	public GameObject[] par;

	public SpriteRenderer render;

	public Sprite on;

	public Sprite off;

	public float lifeTime;

	public float angle;

	[Header("=========")]
	public GameObject SubA;

	public GameObject SubB;

	[Header("=========")]
	[HideInInspector]
	public Dicform dic;

	private float timeA;

	private void Awake()
	{
		dic = GetComponent<Dicform>();
	}

	private void Start()
	{
	}

	private void OnEnable()
	{
		timeA = 0f;
		if (on != null)
		{
			render.sprite = on;
		}
		base.transform.rotation = Quaternion.Euler(0f, 0f, Random.Range(angle, 0f - angle));
		if (par.Length != 0)
		{
			for (int i = 0; i < par.Length; i++)
			{
				par[i].SetActive(value: true);
			}
		}
		if (trail.Length != 0)
		{
			for (int j = 0; j < trail.Length; j++)
			{
				trail[j].emitting = true;
				trail[j].time = trTime[j];
			}
		}
		if (SoundDP != null)
		{
			RuntimeManager.PlayOneShot(SoundDP, base.transform.position);
		}
	}

	private void Update()
	{
		timeA += Time.deltaTime;
		if (timeA >= lifeTime)
		{
			timeA = 0f;
			if (FX != null)
			{
				Dicform component = LeanPool.Spawn(FX, base.transform.position, Quaternion.identity).GetComponent<Dicform>();
				component.sp = dic.sp;
				component.SetCount(dic.sp.ZY);
				component.SubType = dic.SubType;
				component.Index = dic.Index + 1;
			}
			LeanPool.Despawn(this);
		}
	}

	public void Zha()
	{
		if (render != null)
		{
			render.sprite = off;
		}
		if (par.Length != 0)
		{
			for (int i = 0; i < par.Length; i++)
			{
				par[i].SetActive(value: false);
			}
		}
		Dicform component = LeanPool.Spawn(EXP, base.transform.position, Quaternion.identity).GetComponent<Dicform>();
		component.sp = dic.sp;
		component.SetCount(dic.sp.ZY);
		component.SubType = dic.SubType;
		component.Index = dic.Index + 1;
		if (dic.SubType == 0)
		{
			if (dic.sp.Layer_SubA == dic.Index && dic.SubType == 0 && dic.sp.DamageA > 0f && SubA != null)
			{
				Dicform component2 = LeanPool.Spawn(SubA, base.transform.position, Quaternion.identity).GetComponent<Dicform>();
				component2.sp = dic.sp;
				component2.SetCount(dic.sp.ZY);
				component2.SubType = 1;
				component2.Index = dic.Index + 1;
			}
			if (dic.sp.Layer_SubB == dic.Index && dic.SubType == 0 && dic.sp.DamageB > 0f && SubB != null)
			{
				Dicform component3 = LeanPool.Spawn(SubB, base.transform.position, Quaternion.identity).GetComponent<Dicform>();
				component3.sp = dic.sp;
				component3.SetCount(dic.sp.ZY);
				component3.SubType = 2;
				component3.Index = dic.Index + 1;
			}
		}
		if (trail.Length != 0)
		{
			for (int j = 0; j < trail.Length; j++)
			{
				trail[j].emitting = false;
			}
		}
		if (SoundCD != null)
		{
			RuntimeManager.PlayOneShot(SoundCD, base.transform.position);
		}
	}
}
