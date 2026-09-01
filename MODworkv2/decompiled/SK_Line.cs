using FinkFramework.Runtime.Singleton;
using Lean.Pool;
using UnityEngine;

public class SK_Line : MonoBehaviour
{
	[HideInInspector]
	public Dicform dic;

	private float timeB;

	private int count;

	private float speedTmp;

	private GameDataManager _gameDataManager;

	public bool CanAT;

	private bool initialized;

	private void Awake()
	{
		dic = GetComponent<Dicform>();
		_gameDataManager = SingletonMonoScope<GameDataManager>.Instance;
	}

	private void Start()
	{
	}

	private void OnEnable()
	{
		timeB = 0f;
		count = 0;
		CanAT = false;
		initialized = false;
	}

	private void Update()
	{
		if (!CanAT)
		{
			return;
		}
		base.transform.Translate(Vector2.right * (speedTmp * Time.deltaTime));
		if (count >= dic.sp.Count_ORB)
		{
			return;
		}
		timeB += Time.deltaTime;
		if (timeB >= dic.sp.ORB_time)
		{
			timeB = 0f;
			if (dic.sp.Dic_F != 0)
			{
				Dicform component = LeanPool.Spawn(_gameDataManager.SKPB.POS[dic.sp.Dic_F].OBJ[dic.sp.MainEL], base.transform.position, Quaternion.identity).GetComponent<Dicform>();
				component.sp = dic.sp;
				component.SetCount(dic.sp.ZY);
				component.SubType = dic.SubType;
				component.Index = dic.Index;
			}
			count++;
			if (count == dic.sp.Count_ORB)
			{
				LeanPool.Despawn(this);
				CanAT = false;
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
		CanAT = true;
		speedTmp = 12f;
	}

	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (collision.CompareTag("blockFLY"))
		{
			speedTmp = 0f;
		}
	}
}
