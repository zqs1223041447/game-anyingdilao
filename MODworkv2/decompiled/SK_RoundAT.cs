using Lean.Pool;
using UnityEngine;

public class SK_RoundAT : MonoBehaviour
{
	private float timeA;

	public bool CanAT;

	public Dicform dic;

	public float ATtime;

	public SK_Round father;

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
				FXcd = Random.Range(0.1f, 0.3f);
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
			if (dic != null)
			{
				if (dic.sp.ZY)
				{
					if (component.peo.CharacterType == 2 && component.peo.em.IsAlive && !component.peo.em.IsJump && !component.peo.em.IsYS)
					{
						component.peo.EM_Set(dic.sp, father.DotMulti, dic.SubType, Dot_Infect: false, 0, 0f);
						if (father.FX != null && CanFX)
						{
							LeanPool.Spawn(father.FX, component.peo.em.yao.transform.position, Quaternion.identity, component.peo.em.yao.transform);
							CanFX = false;
						}
					}
				}
				else
				{
					if (component.peo.CharacterType == 0 && component.peo.pl.IsAlive)
					{
						component.peo.PL_Set(dic.sp, dic.SubType);
					}
					if (component.peo.CharacterType == 1 && component.peo.cp.IsAlive)
					{
						component.peo.CP_Set(dic.sp, dic.SubType);
					}
				}
			}
		}
		if (collision.CompareTag("Break"))
		{
			collision.GetComponent<BreakOBJ>().Break();
		}
	}
}
