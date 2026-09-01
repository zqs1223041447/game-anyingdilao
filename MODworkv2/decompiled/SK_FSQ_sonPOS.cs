using Lean.Pool;
using UnityEngine;

public class SK_FSQ_sonPOS : MonoBehaviour
{
	public GameObject OBJ;

	public bool UseDicTime;

	public float LifeTime;

	public int FasheType;

	public float range;

	public float FasheTime;

	public bool UseFSQ_count;

	public int FSQ_count;

	public int EveryNumber;

	public float MoveSpeed;

	[Header("=========")]
	public GameObject SubA;

	public GameObject SubB;

	public GameObject EXP;

	public bool CanEXP;

	private int FSnumber;

	private float timeA;

	private float timeB;

	private int FScountTMP;

	private bool CanAT;

	[HideInInspector]
	public Dicform dic;

	private float speedTmp;

	private bool initialized;

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
		timeB = 0f;
		FScountTMP = 0;
		CanAT = false;
		speedTmp = MoveSpeed;
		initialized = false;
	}

	private void Update()
	{
		if (FasheType == 1 && CanAT)
		{
			if (UseDicTime)
			{
				timeB += Time.deltaTime;
				if (timeB > FasheTime)
				{
					for (int i = 0; i < EveryNumber; i++)
					{
						Vector3 vector = Random.insideUnitCircle * range;
						Dicform component = LeanPool.Spawn(OBJ, new Vector3(base.transform.position.x + vector.x, base.transform.position.y + vector.y, base.transform.position.z + vector.z), Quaternion.identity).GetComponent<Dicform>();
						component.sp = dic.sp;
						component.SetCount(dic.sp.ZY);
						component.SubType = dic.SubType;
						component.dic = Vector2.zero;
					}
					timeB = 0f;
				}
			}
			else if (FScountTMP < FSnumber)
			{
				timeB += Time.deltaTime;
				if (timeB > FasheTime)
				{
					for (int j = 0; j < EveryNumber; j++)
					{
						Vector3 vector2 = Random.insideUnitCircle * range;
						Dicform component2 = LeanPool.Spawn(OBJ, new Vector3(base.transform.position.x + vector2.x, base.transform.position.y + vector2.y, base.transform.position.z + vector2.z), Quaternion.identity).GetComponent<Dicform>();
						component2.sp = dic.sp;
						component2.SetCount(dic.sp.ZY);
						component2.SubType = dic.SubType;
						component2.dic = Vector2.zero;
					}
					timeB = 0f;
					FScountTMP++;
				}
			}
			if (MoveSpeed > 0f)
			{
				base.transform.Translate(dic.dic.normalized * (speedTmp * Time.deltaTime));
			}
		}
		if (CanAT)
		{
			timeA += Time.deltaTime;
			if (timeA > LifeTime)
			{
				timeA = 0f;
				speedTmp = 0f;
				LeanPool.Despawn(this);
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
		if (UseDicTime)
		{
			LifeTime = dic.sp.BuffTime;
		}
		if (UseFSQ_count)
		{
			FSnumber = FSQ_count;
		}
		else
		{
			switch (dic.SubType)
			{
			case 0:
				FSnumber = dic.sp.Count_S;
				break;
			case 1:
				FSnumber = dic.sp.Count_AB;
				break;
			case 2:
				FSnumber = dic.sp.Count_AB;
				break;
			}
		}
		CanAT = true;
		if (FasheType == 0)
		{
			for (int i = 0; i < FSnumber; i++)
			{
				Vector3 vector = Random.insideUnitCircle * range;
				Dicform component = LeanPool.Spawn(OBJ, new Vector3(base.transform.position.x + vector.x, base.transform.position.y + vector.y, base.transform.position.z + vector.z), Quaternion.identity).GetComponent<Dicform>();
				component.sp = dic.sp;
				component.SetCount(dic.sp.ZY);
				component.SubType = dic.SubType;
				component.dic = Vector2.zero;
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
		if (CanEXP && EXP != null)
		{
			Dicform component4 = LeanPool.Spawn(EXP, base.transform.position, Quaternion.identity).GetComponent<Dicform>();
			component4.sp = dic.sp;
			component4.SetCount(dic.sp.ZY);
			component4.SubType = dic.SubType;
			component4.Index = dic.Index + 1;
		}
	}

	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (collision.CompareTag("blockWALL"))
		{
			speedTmp = 0f;
		}
	}
}
