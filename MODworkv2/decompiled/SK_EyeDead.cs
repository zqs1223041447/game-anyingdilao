using FMODUnity;
using Lean.Pool;
using UnityEngine;

public class SK_EyeDead : MonoBehaviour
{
	public string SoundA;

	public GameObject obj;

	private float timeA;

	private float timeB;

	public float FaSheTime;

	[HideInInspector]
	public Dicform dic;

	private bool CanAT;

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
		CanAT = false;
		timeA = 0f;
		timeB = 0f;
		initialized = false;
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
		CanAT = true;
		if (SoundA != null)
		{
			RuntimeManager.PlayOneShot(SoundA, base.transform.position);
		}
	}

	private void Update()
	{
		if (CanAT)
		{
			timeA += Time.deltaTime;
			if (timeA >= FaSheTime)
			{
				timeA = 0f;
				CanAT = false;
				FaShe();
			}
		}
		timeB += Time.deltaTime;
		if (timeB >= 5f)
		{
			timeB = 0f;
			LeanPool.Despawn(this);
		}
	}

	public void FaShe()
	{
		Dicform component = LeanPool.Spawn(obj, base.transform.position, Quaternion.identity).GetComponent<Dicform>();
		component.sp = dic.sp;
		component.SetCount(dic.sp.ZY);
		component.SubType = dic.SubType;
		component.Index = dic.Index;
	}
}
