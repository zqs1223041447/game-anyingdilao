using FMODUnity;
using Lean.Pool;
using UnityEngine;

public class SK_EXPangle : MonoBehaviour
{
	public string SoundA;

	public float SetColTime;

	public float LifeTime;

	public float DotMulti;

	private bool CanAT;

	private float timeA;

	private float timeB;

	[HideInInspector]
	public Dicform dic;

	[HideInInspector]
	public Collider2D MainCOL;

	private void Awake()
	{
		dic = GetComponent<Dicform>();
		MainCOL = GetComponent<Collider2D>();
	}

	private void Start()
	{
	}

	private void OnEnable()
	{
		timeA = 0f;
		timeB = 0f;
		CanAT = true;
		MainCOL.enabled = false;
		this.wait(SetColTime, SetStart);
	}

	private void Update()
	{
		timeA += Time.deltaTime;
		if (timeA > LifeTime)
		{
			timeA = 0f;
			LeanPool.Despawn(this);
		}
		if (CanAT)
		{
			timeB += Time.deltaTime;
			if (timeB > SetColTime + 0.1f)
			{
				MainCOL.enabled = false;
				CanAT = false;
				timeB = 0f;
			}
		}
	}

	public void SetStart()
	{
		MainCOL.enabled = true;
		if (SoundA != null)
		{
			RuntimeManager.PlayOneShot(SoundA, base.transform.position);
		}
	}

	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (!collision.CompareTag("BodyCOL"))
		{
			return;
		}
		BodyCOL component = collision.GetComponent<BodyCOL>();
		if (!(dic != null))
		{
			return;
		}
		if (dic.sp.ZY)
		{
			if (component.peo.CharacterType == 2 && component.peo.em.IsAlive && !component.peo.em.IsJump && !component.peo.em.IsYS)
			{
				component.peo.EM_Set(dic.sp, DotMulti, dic.SubType, Dot_Infect: false, 0, dic.UPDamage);
			}
			return;
		}
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
