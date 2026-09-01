using FMODUnity;
using Lean.Pool;
using UnityEngine;

public class SK_DropJing : MonoBehaviour
{
	public string SoundCD;

	public string SoundRing;

	public float RingDelay;

	public GameObject exp;

	public float lifeTime;

	public Sprite on;

	public Sprite off;

	[Header("=========")]
	[HideInInspector]
	public Dicform dic;

	[HideInInspector]
	public SpriteRenderer render;

	private float timeA;

	private float timeB;

	private float timeC;

	private bool StartOK;

	private bool RingOK;

	private void Awake()
	{
		render = base.transform.Find("qiu").GetComponent<SpriteRenderer>();
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
		render.sprite = on;
		StartOK = false;
		RingOK = false;
	}

	private void Update()
	{
		if (!StartOK)
		{
			timeB += Time.deltaTime;
			if (timeB >= 0.2f)
			{
				render.sprite = off;
				timeB = 0f;
				StartOK = true;
				if (SoundCD != null)
				{
					RuntimeManager.PlayOneShot(SoundCD, base.transform.position);
				}
			}
		}
		if (!RingOK)
		{
			timeC += Time.deltaTime;
			if (timeC >= RingDelay)
			{
				timeC = 0f;
				RingOK = true;
				if (SoundCD != null)
				{
					RuntimeManager.PlayOneShot(SoundCD, base.transform.position);
				}
			}
		}
		timeA += Time.deltaTime;
		if (timeA >= lifeTime)
		{
			timeA = 0f;
			FaShe();
		}
	}

	public void FaShe()
	{
		Dicform component = LeanPool.Spawn(exp, base.transform.position, base.transform.rotation).GetComponent<Dicform>();
		component.sp = dic.sp;
		component.SetCount(dic.sp.ZY);
		component.SubType = dic.SubType;
		component.Index = dic.Index;
		LeanPool.Despawn(this);
	}

	public void Zha()
	{
		render.sprite = off;
	}
}
