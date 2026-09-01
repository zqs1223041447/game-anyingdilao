using FMODUnity;
using Lean.Pool;
using UnityEngine;

public class SK_Round_SQS : MonoBehaviour
{
	public string SoundA;

	public Transform[] trans;

	public GameObject FX;

	public GameObject spark;

	[Header("=========")]
	public float angle;

	public float DotMulti;

	[Header("=========")]
	public GameObject SubA;

	[HideInInspector]
	public SkillOBJ_DT_SP sp;

	[HideInInspector]
	public SK_BuffA mg;

	[HideInInspector]
	public Transform core;

	private bool canAT;

	private float timeA;

	private void Awake()
	{
		sp = GetComponent<SkillOBJ_DT_SP>();
		mg = GetComponent<SK_BuffA>();
		core = base.transform.Find("core");
	}

	private void Start()
	{
	}

	private void OnEnable()
	{
		timeA = 0f;
		canAT = false;
		this.wait(1E-05f, SetStart);
	}

	private void Update()
	{
		core.Rotate(new Vector3(0f, 0f, 1f), angle * Time.deltaTime);
		if (canAT)
		{
			timeA += Time.deltaTime;
			if (timeA > sp.BuffTime)
			{
				timeA = 0f;
				Stop();
			}
			if (mg.NeedStop)
			{
				Stop();
			}
		}
	}

	public void SetStart()
	{
		canAT = true;
		for (int i = 0; i < trans.Length; i++)
		{
			SK_RoundAT_SQS component = trans[i].GetComponent<SK_RoundAT_SQS>();
			component.father = this;
			component.CanAT = true;
			Dicform component2 = trans[i].gameObject.GetComponent<Dicform>();
			component2.sp = sp;
			component2.SetCount(sp.ZY);
			component2.SubType = 0;
			component2.Index = 0;
		}
		if (SoundA != null)
		{
			RuntimeManager.PlayOneShot(SoundA, base.transform.position);
		}
	}

	public void Stop()
	{
		disThunder();
	}

	public void disThunder()
	{
		canAT = false;
		for (int i = 0; i < trans.Length; i++)
		{
			trans[i].GetComponent<SK_RoundAT_SQS>().CanAT = false;
			LeanPool.Spawn(spark, trans[i].position, Quaternion.identity);
		}
		LeanPool.Despawn(this);
	}
}
