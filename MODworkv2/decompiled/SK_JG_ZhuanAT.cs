using Lean.Pool;
using UnityEngine;

public class SK_JG_ZhuanAT : MonoBehaviour
{
	public Collider2D MainCore;

	public SK_JG_Zhuan father;

	private bool CanFX;

	private float FXcd;

	private float timeB;

	private void Awake()
	{
		MainCore = GetComponent<Collider2D>();
		father = GetComponentInParent<SK_JG_Zhuan>();
	}

	private void Start()
	{
	}

	private void OnEnable()
	{
		CanFX = true;
		FXcd = Random.Range(0.1f, 0.3f);
		MainCore.enabled = false;
		this.wait(1E-05f, SetStart);
	}

	private void Update()
	{
		timeB += Time.deltaTime;
		if (timeB >= FXcd)
		{
			FXcd = Random.Range(0.1f, 0.3f);
			CanFX = true;
			timeB = 0f;
		}
	}

	public void SetStart()
	{
		MainCore.enabled = true;
	}

	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (collision.CompareTag("BodyCOL"))
		{
			BodyCOL component = collision.GetComponent<BodyCOL>();
			if (father.dic.sp.ZY)
			{
				if (component.peo.CharacterType == 2 && component.peo.em.IsAlive && !component.peo.em.IsJump && !component.peo.em.IsYS)
				{
					component.peo.EM_Set(father.dic.sp, father.DotMulti, 0, Dot_Infect: false, 0, father.dic.UPDamage);
					if (CanFX)
					{
						CanFX = false;
						LeanPool.Spawn(father.FX, component.peo.em.yao.transform.position, Quaternion.identity, component.peo.em.yao.transform);
					}
				}
			}
			else
			{
				if (component.peo.CharacterType == 0 && component.peo.pl.IsAlive)
				{
					component.peo.PL_Set(father.dic.sp, father.dic.SubType);
					if (CanFX)
					{
						CanFX = false;
						LeanPool.Spawn(father.FX, component.peo.pl.yao.transform.position, Quaternion.identity, component.peo.pl.yao.transform);
					}
				}
				if (component.peo.CharacterType == 1 && component.peo.cp.IsAlive)
				{
					component.peo.CP_Set(father.dic.sp, father.dic.SubType);
					if (CanFX)
					{
						CanFX = false;
						LeanPool.Spawn(father.FX, component.peo.cp.yao.transform.position, Quaternion.identity, component.peo.cp.yao.transform);
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
