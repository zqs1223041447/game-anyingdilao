using FinkFramework.Runtime.Singleton;
using Lean.Pool;
using UnityEngine;

public class SK_RoundAT_SQS : MonoBehaviour
{
	private float timeA;

	public bool CanAT;

	public Dicform dic;

	public float ATtime;

	public SK_Round_SQS father;

	private float FXcd;

	private bool CanFX;

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
		FXcd = Random.Range(0.1f, 0.3f);
		CanFX = true;
		CanAT = false;
	}

	private void Update()
	{
		if (CanAT)
		{
			timeA += Time.deltaTime;
			if (timeA >= FXcd)
			{
				FXcd = Random.Range(0.2f, 0.4f);
				CanFX = true;
				timeA = 0f;
			}
		}
	}

	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (!CanAT)
		{
			return;
		}
		if (collision.CompareTag("BodyCOL"))
		{
			BodyCOL component = collision.GetComponent<BodyCOL>();
			if (dic != null && component.peo.CharacterType == 2 && component.peo.em.IsAlive && !component.peo.em.IsJump && !component.peo.em.IsYS)
			{
				component.peo.EM_Set(dic.sp, father.DotMulti, dic.SubType, Dot_Infect: false, 0, 0f);
				if (father.FX != null && CanFX)
				{
					LeanPool.Spawn(father.FX, component.peo.em.yao.transform.position, Quaternion.identity, component.peo.em.yao.transform);
					if (dic.sp.Layer_SubA == dic.Index && dic.SubType == 0 && dic.sp.DamageA > 0f && father.SubA != null && Random.Range(0, 101) <= 30)
					{
						Dicform component2 = LeanPool.Spawn(father.SubA, base.transform.position, Quaternion.identity).GetComponent<Dicform>();
						component2.sp = dic.sp;
						component2.SetCount(dic.sp.ZY);
						component2.SubType = 1;
						component2.Index = dic.Index + 1;
					}
					CanFX = false;
				}
				if (Random.Range(0, 101) < 5)
				{
					SingletonMonoScope<ACTbar>.Instance.CreatACT_Hit(dic.sp.skillName, component.peo.em, base.transform.position - father.transform.position);
				}
			}
		}
		if (collision.CompareTag("Break"))
		{
			collision.GetComponent<BreakOBJ>().Break();
		}
	}
}
