using Lean.Pool;
using UnityEngine;

public class SK_JumpD : MonoBehaviour
{
	public GameObject OBJ;

	public GameObject[] par;

	public float speed;

	[Header("=========")]
	public float FStime;

	public float SpeedMin;

	public float SpeedMax;

	[Header("=========")]
	public GameObject SubA;

	public GameObject SubB;

	[HideInInspector]
	public Animator ani;

	[HideInInspector]
	public Dicform dic;

	private bool CanMV;

	private float timeA;

	private int CountTmp;

	private float speedTMP;

	private bool initialized;

	private void Awake()
	{
		ani = GetComponent<Animator>();
		dic = GetComponent<Dicform>();
	}

	private void Start()
	{
	}

	private void OnEnable()
	{
		timeA = 0f;
		CountTmp = 0;
		CanMV = false;
		if (par.Length != 0)
		{
			for (int i = 0; i < par.Length; i++)
			{
				par[i].SetActive(value: true);
			}
		}
		ani.SetBool("stop", value: false);
		initialized = false;
	}

	private void Update()
	{
		if (!CanMV)
		{
			return;
		}
		base.transform.Translate(dic.dic.normalized * (speedTMP * Time.deltaTime));
		if (dic.sp.Layer_SubB == dic.Index && dic.SubType == 0 && dic.sp.DamageB > 0f && SubB != null)
		{
			timeA += Time.deltaTime;
			if (timeA >= FStime)
			{
				Dicform component = LeanPool.Spawn(SubB, base.transform.position, Quaternion.identity).GetComponent<Dicform>();
				component.sp = dic.sp;
				component.SetCount(dic.sp.ZY);
				component.SubType = 2;
				component.Index = dic.Index + 1;
				component.dic = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f));
				component.speed = Random.Range(SpeedMin, SpeedMax);
				timeA = 0f;
			}
		}
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
		CanMV = true;
		if (dic.Index == 0)
		{
			if (dic.sp.Speed1 == 0f)
			{
				speedTMP = speed;
			}
			else
			{
				speedTMP = dic.sp.Speed1;
			}
		}
		else if (dic.sp.Speed3 == 0f)
		{
			speedTMP = speed;
		}
		else
		{
			speedTMP = dic.sp.Speed3;
		}
	}

	public void Zha()
	{
		if (!CanMV)
		{
			return;
		}
		if (CountTmp < dic.sp.Count_ATtarget - 1)
		{
			Dicform component = LeanPool.Spawn(OBJ, base.transform.position, Quaternion.identity).GetComponent<Dicform>();
			component.sp = dic.sp;
			component.SetCount(dic.sp.ZY);
			component.SubType = dic.SubType;
			component.Index = dic.Index + 1;
			if (dic.sp.Layer_SubA == dic.Index && dic.SubType == 0 && dic.sp.DamageA > 0f && SubA != null)
			{
				Dicform component2 = LeanPool.Spawn(SubA, base.transform.position, Quaternion.identity).GetComponent<Dicform>();
				component2.sp = dic.sp;
				component2.SetCount(dic.sp.ZY);
				component2.SubType = 1;
				component2.Index = dic.Index + 1;
			}
			CountTmp++;
		}
		else
		{
			Dicform component3 = LeanPool.Spawn(OBJ, base.transform.position, Quaternion.identity).GetComponent<Dicform>();
			component3.sp = dic.sp;
			component3.SetCount(dic.sp.ZY);
			component3.SubType = dic.SubType;
			component3.Index = dic.Index + 1;
			if (dic.sp.Layer_SubA == dic.Index && dic.SubType == 0 && dic.sp.DamageA > 0f && SubA != null)
			{
				Dicform component4 = LeanPool.Spawn(SubA, base.transform.position, Quaternion.identity).GetComponent<Dicform>();
				component4.sp = dic.sp;
				component4.SetCount(dic.sp.ZY);
				component4.SubType = 1;
				component4.Index = dic.Index + 1;
			}
			Stop();
		}
	}

	public void Stop()
	{
		ani.SetBool("stop", value: true);
		CanMV = false;
		if (par.Length != 0)
		{
			for (int i = 0; i < par.Length; i++)
			{
				par[i].SetActive(value: false);
			}
		}
		this.wait(1f, delegate
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
