using FMODUnity;
using Lean.Pool;
using UnityEngine;

public class SK_FSQ_SonA : MonoBehaviour
{
	public string SoundA;

	public GameObject FX;

	public GameObject OBJ;

	public ParticleSystem[] par;

	public int FasheType;

	public float angleRange;

	public float LifeTime;

	public float DelDelay;

	public float FaSheDelay;

	public bool UseDicCount;

	public int FaSheCount;

	public int CountMulti;

	public float FStime;

	public float JianGe;

	[Header("=========")]
	public float MoveSpeed;

	public bool Slow;

	public float LerpSpeed;

	[HideInInspector]
	public Dicform dic;

	private bool CanAT;

	private bool CanMV;

	private float timeA;

	private float timeB;

	private float timeC;

	private int FScountTMP;

	private float MoveSpeedTmp;

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
		timeC = 0f;
		FScountTMP = 0;
		CanAT = false;
		CanMV = false;
		MoveSpeedTmp = MoveSpeed;
		if (par.Length != 0)
		{
			for (int i = 0; i < par.Length; i++)
			{
				ParticleSystem.MainModule main = par[i].main;
				main.loop = true;
			}
		}
		initialized = false;
	}

	private void Update()
	{
		if (CanMV)
		{
			base.transform.Translate(dic.dic.normalized * (MoveSpeedTmp * Time.deltaTime));
			if (Slow)
			{
				MoveSpeedTmp = Mathf.Lerp(MoveSpeedTmp, 0f, Time.deltaTime * LerpSpeed);
			}
		}
		if (CanAT)
		{
			if (FasheType > 4 && FScountTMP < FaSheCount)
			{
				timeB += Time.deltaTime;
				if (timeB > FStime)
				{
					switch (FasheType)
					{
					case 5:
					{
						for (int j = 0; j < CountMulti; j++)
						{
							Dicform component2 = LeanPool.Spawn(OBJ, base.transform.position, Quaternion.Euler(0f, 0f, Random.Range(0f, 360f))).GetComponent<Dicform>();
							component2.sp = dic.sp;
							component2.SetCount(dic.sp.ZY);
							component2.SubType = dic.SubType;
							component2.Index = dic.Index;
						}
						break;
					}
					case 6:
					{
						Vector3 right = base.transform.right;
						float num = Mathf.Atan2(right.y, right.x) * 57.29578f;
						for (int i = 0; i < CountMulti; i++)
						{
							Dicform component = LeanPool.Spawn(OBJ, base.transform.position, Quaternion.Euler(0f, 0f, num + Random.Range(0f - angleRange, angleRange))).GetComponent<Dicform>();
							component.sp = dic.sp;
							component.SetCount(dic.sp.ZY);
							component.SubType = dic.SubType;
							component.Index = dic.Index;
						}
						break;
					}
					}
					timeB = 0f;
					FScountTMP++;
				}
			}
			if (FasheType < 5)
			{
				timeC += Time.deltaTime;
				if (timeC > FaSheDelay)
				{
					timeC = 0f;
					CanAT = false;
					FaShe();
				}
			}
		}
		timeA += Time.deltaTime;
		if (timeA > LifeTime)
		{
			timeA = 0f;
			Stop();
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
		CanAT = true;
		if (UseDicCount)
		{
			switch (dic.SubType)
			{
			case 0:
				FaSheCount = dic.sp.Count_S;
				break;
			case 1:
				FaSheCount = dic.sp.Count_AB;
				break;
			case 2:
				FaSheCount = dic.sp.Count_AB;
				break;
			}
		}
		if (FX != null)
		{
			LeanPool.Spawn(FX, base.transform.position, Quaternion.identity);
		}
		if (SoundA != null)
		{
			RuntimeManager.PlayOneShot(SoundA, base.transform.position);
		}
	}

	public void FaShe()
	{
		if (FasheType >= 5)
		{
			return;
		}
		switch (FasheType)
		{
		case 0:
		{
			Vector3 right = base.transform.right;
			float num = Mathf.Atan2(right.y, right.x) * 57.29578f;
			if (FaSheCount % 2 == 1)
			{
				Dicform component2 = LeanPool.Spawn(OBJ, base.transform.position, Quaternion.Euler(0f, 0f, num)).GetComponent<Dicform>();
				component2.sp = dic.sp;
				component2.SetCount(dic.sp.ZY);
				component2.SubType = dic.SubType;
				component2.Index = dic.Index;
				if (FaSheCount > 1)
				{
					for (int j = 0; j < FaSheCount / 2; j++)
					{
						GameObject obj = LeanPool.Spawn(OBJ, base.transform.position, Quaternion.Euler(0f, 0f, num + angleRange * (float)(j + 1)));
						GameObject gameObject = LeanPool.Spawn(OBJ, base.transform.position, Quaternion.Euler(0f, 0f, num - angleRange * (float)(j + 1)));
						Dicform component3 = obj.GetComponent<Dicform>();
						component3.sp = dic.sp;
						component3.SetCount(dic.sp.ZY);
						component3.SubType = dic.SubType;
						component3.Index = dic.Index;
						Dicform component4 = gameObject.GetComponent<Dicform>();
						component4.sp = dic.sp;
						component4.SetCount(dic.sp.ZY);
						component4.SubType = dic.SubType;
						component4.Index = dic.Index;
					}
				}
			}
			else
			{
				for (int k = 0; k < FaSheCount / 2; k++)
				{
					GameObject obj2 = LeanPool.Spawn(OBJ, base.transform.position, Quaternion.Euler(0f, 0f, num + angleRange * (float)(k + 1) - angleRange / 2f));
					GameObject gameObject2 = LeanPool.Spawn(OBJ, base.transform.position, Quaternion.Euler(0f, 0f, num - angleRange * (float)(k + 1) + angleRange / 2f));
					Dicform component5 = obj2.GetComponent<Dicform>();
					component5.sp = dic.sp;
					component5.SetCount(dic.sp.ZY);
					component5.SubType = dic.SubType;
					component5.Index = dic.Index;
					Dicform component6 = gameObject2.GetComponent<Dicform>();
					component6.sp = dic.sp;
					component6.SetCount(dic.sp.ZY);
					component6.SubType = dic.SubType;
					component6.Index = dic.Index;
				}
			}
			break;
		}
		case 1:
		{
			Vector3 vector = base.transform.right * JianGe;
			float z = Mathf.Atan2(vector.y, vector.x) * 57.29578f;
			if (FaSheCount % 2 == 1)
			{
				Dicform component8 = LeanPool.Spawn(OBJ, base.transform.position, Quaternion.Euler(0f, 0f, z)).GetComponent<Dicform>();
				component8.sp = dic.sp;
				component8.SubType = dic.SubType;
				component8.Index = dic.Index;
				if (FaSheCount > 1)
				{
					for (int m = 0; m < FaSheCount / 2; m++)
					{
						Vector3 position = new Vector3(base.transform.position.x + vector.y * (float)(m + 1), base.transform.position.y - vector.x * (float)(m + 1), 0f);
						Vector3 position2 = new Vector3(base.transform.position.x - vector.y * (float)(m + 1), base.transform.position.y + vector.x * (float)(m + 1), 0f);
						GameObject obj3 = LeanPool.Spawn(OBJ, position, Quaternion.Euler(0f, 0f, z));
						GameObject gameObject3 = LeanPool.Spawn(OBJ, position2, Quaternion.Euler(0f, 0f, z));
						Dicform component9 = obj3.GetComponent<Dicform>();
						component9.sp = dic.sp;
						component9.SetCount(dic.sp.ZY);
						component9.Index = dic.Index;
						component9.SubType = dic.SubType;
						Dicform component10 = gameObject3.GetComponent<Dicform>();
						component10.sp = dic.sp;
						component10.SetCount(dic.sp.ZY);
						component10.SubType = dic.SubType;
						component10.Index = dic.Index;
					}
				}
			}
			else
			{
				for (int n = 0; n < FaSheCount / 2; n++)
				{
					Vector3 position3 = new Vector3(base.transform.position.x + vector.y / 2f + vector.y * (float)n, base.transform.position.y - vector.x / 2f - vector.x * (float)n, 0f);
					Vector3 position4 = new Vector3(base.transform.position.x - vector.y / 2f - vector.y * (float)n, base.transform.position.y + vector.x / 2f + vector.x * (float)n, 0f);
					GameObject obj4 = LeanPool.Spawn(OBJ, position3, Quaternion.Euler(0f, 0f, z));
					GameObject gameObject4 = LeanPool.Spawn(OBJ, position4, Quaternion.Euler(0f, 0f, z));
					Dicform component11 = obj4.GetComponent<Dicform>();
					component11.sp = dic.sp;
					component11.SetCount(dic.sp.ZY);
					component11.SubType = dic.SubType;
					component11.Index = dic.Index;
					Dicform component12 = gameObject4.GetComponent<Dicform>();
					component12.sp = dic.sp;
					component12.SetCount(dic.sp.ZY);
					component12.SubType = dic.SubType;
					component12.Index = dic.Index;
				}
			}
			break;
		}
		case 2:
		{
			Vector3 right2 = base.transform.right;
			float num2 = Mathf.Atan2(right2.y, right2.x) * 57.29578f;
			for (int num3 = 0; num3 < FaSheCount; num3++)
			{
				Dicform component13 = LeanPool.Spawn(OBJ, base.transform.position, Quaternion.Euler(0f, 0f, num2 + Random.Range(0f - angleRange, angleRange))).GetComponent<Dicform>();
				component13.sp = dic.sp;
				component13.SetCount(dic.sp.ZY);
				component13.SubType = dic.SubType;
				component13.Index = dic.Index;
			}
			break;
		}
		case 3:
		{
			for (int l = 0; l < FaSheCount; l++)
			{
				Dicform component7 = LeanPool.Spawn(OBJ, base.transform.position, Quaternion.Euler(0f, 0f, 360 / FaSheCount * (l + 1))).GetComponent<Dicform>();
				component7.sp = dic.sp;
				component7.SetCount(dic.sp.ZY);
				component7.SubType = dic.SubType;
				component7.Index = dic.Index;
			}
			break;
		}
		case 4:
		{
			for (int i = 0; i < FaSheCount; i++)
			{
				Dicform component = LeanPool.Spawn(OBJ, base.transform.position, Quaternion.Euler(0f, 0f, Random.Range(0, 360))).GetComponent<Dicform>();
				component.sp = dic.sp;
				component.SetCount(dic.sp.ZY);
				component.SubType = dic.SubType;
				component.Index = dic.Index;
			}
			break;
		}
		}
	}

	public void Stop()
	{
		if (par.Length != 0)
		{
			for (int i = 0; i < par.Length; i++)
			{
				ParticleSystem.MainModule main = par[i].main;
				main.loop = false;
			}
		}
		CanAT = false;
		CanMV = false;
		this.wait(DelDelay, delegate
		{
			LeanPool.Despawn(this);
		});
	}
}
