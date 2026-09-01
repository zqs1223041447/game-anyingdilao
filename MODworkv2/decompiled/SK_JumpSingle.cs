using Lean.Pool;
using UnityEngine;

public class SK_JumpSingle : MonoBehaviour
{
	public GameObject OBJ;

	public float delDelay;

	public GameObject[] parOBJ;

	public float SpeedMulit;

	[Header("=========")]
	public GameObject SubA;

	public GameObject SubB;

	[HideInInspector]
	public Dicform dic;

	private bool CanAT;

	private void Awake()
	{
		dic = GetComponent<Dicform>();
	}

	private void Start()
	{
	}

	private void OnEnable()
	{
		CanAT = false;
		dic.speed = 0f;
		if (parOBJ.Length != 0)
		{
			for (int i = 0; i < parOBJ.Length; i++)
			{
				parOBJ[i].SetActive(value: true);
			}
		}
		this.wait(0.0001f, FaShe);
	}

	private void Update()
	{
		if (CanAT)
		{
			base.transform.Translate(dic.dic.normalized * (dic.speed * SpeedMulit * Time.deltaTime));
		}
	}

	public void FaShe()
	{
		CanAT = true;
	}

	public void Zha()
	{
		Dicform component = LeanPool.Spawn(OBJ, base.transform.position, Quaternion.identity).GetComponent<Dicform>();
		component.sp = dic.sp;
		component.SetCount(dic.sp.ZY);
		component.SubType = dic.SubType;
		component.Index = dic.Index + 1;
		dic.dic = Vector2.zero;
		if (parOBJ.Length != 0)
		{
			for (int i = 0; i < parOBJ.Length; i++)
			{
				parOBJ[i].SetActive(value: false);
			}
		}
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
		this.wait(delDelay, delegate
		{
			LeanPool.Despawn(this);
		});
	}

	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (collision.CompareTag("blockFLY"))
		{
			dic.dic = Vector2.zero;
		}
	}
}
