using FMODUnity;
using FinkFramework.Runtime.Singleton;
using Lean.Pool;
using UnityEngine;

public class SK_Circle_Move : MonoBehaviour
{
	private float MoveSpeed;

	private float timeA;

	private float timeB;

	private bool CanAT;

	[HideInInspector]
	public SkillOBJ_DT_SP sp;

	private float speedTmp;

	[HideInInspector]
	public StudioEventEmitter emt;

	[HideInInspector]
	public CircleCollider2D col;

	public Vector3 Dic;

	private GameDataManager _gameDataManager;

	private bool initialized;

	private void Awake()
	{
		sp = GetComponent<SkillOBJ_DT_SP>();
		emt = GetComponent<StudioEventEmitter>();
		col = GetComponent<CircleCollider2D>();
		_gameDataManager = SingletonMonoScope<GameDataManager>.Instance;
	}

	private void Start()
	{
	}

	private void OnEnable()
	{
		timeA = 0f;
		timeB = 0f;
		CanAT = false;
		initialized = false;
	}

	private void Update()
	{
		if (!CanAT)
		{
			return;
		}
		timeB += Time.deltaTime;
		if (timeB > sp.FStime1)
		{
			for (int i = 0; i < sp.CountMulti; i++)
			{
				Vector3 vector = Random.insideUnitCircle * sp.Range1;
				Dicform component = LeanPool.Spawn(_gameDataManager.SKPB.POS[sp.ZD_F].OBJ[sp.MainEL], new Vector3(base.transform.position.x + vector.x, base.transform.position.y + vector.y, base.transform.position.z + vector.z), Quaternion.identity).GetComponent<Dicform>();
				component.sp = sp;
				component.SetCount(sp.ZY);
				component.SubType = 0;
				component.Index = 0;
				component.dic = Vector2.zero;
			}
			timeB = 0f;
		}
		if (MoveSpeed > 0f)
		{
			base.transform.Translate(new Vector3(Dic.x, Dic.y, 0f).normalized * (speedTmp * Time.deltaTime));
		}
		timeA += Time.deltaTime;
		if (timeA > sp.BuffTime)
		{
			timeA = 0f;
			speedTmp = 0f;
			LeanPool.Despawn(this);
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
		MoveSpeed = 10f / sp.Range1;
		speedTmp = MoveSpeed;
		if (sp.Range1 > 0.3f)
		{
			col.radius = sp.Range1 - 0.2f;
		}
		else
		{
			col.radius = 0.1f;
		}
		CanAT = true;
		Dic = sp.TargetPos - base.transform.position;
		emt.EventReference = _gameDataManager.SKPB.SoundRain[sp.Sound];
		emt.Play();
	}

	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (collision.CompareTag("blockWALL"))
		{
			speedTmp = 0f;
		}
	}
}
