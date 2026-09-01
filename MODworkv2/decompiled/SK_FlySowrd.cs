using FinkFramework.Runtime.Singleton;
using Lean.Pool;
using UnityEngine;

public class SK_FlySowrd : MonoBehaviour
{
	public float DotMulti;

	[HideInInspector]
	public Collider2D MainCOL;

	[HideInInspector]
	public SK_FlySowrdFSQ father;

	[HideInInspector]
	public Transform point;

	[HideInInspector]
	public Enemy target;

	private bool Follow;

	[HideInInspector]
	public float speed;

	private float timeA;

	private float timeB;

	private bool CanFX;

	private float FXcd;

	private bool CanAT;

	private PlayerManager PL;

	private bool _isStopping;

	[HideInInspector]
	public bool countedPrefab;

	[HideInInspector]
	public int countedPrefabType = -1;

	private bool initialized;

	private void Awake()
	{
		MainCOL = GetComponent<Collider2D>();
		PL = SingletonMonoScope<PlayerManager>.Instance;
	}

	private void OnEnable()
	{
		Follow = false;
		timeA = 0f;
		timeB = 0f;
		CanFX = false;
		CanAT = false;
		_isStopping = false;
		countedPrefab = false;
		countedPrefabType = -1;
		if ((bool)MainCOL)
		{
			MainCOL.enabled = false;
		}
		FXcd = Random.Range(0.3f, 0.5f);
		initialized = false;
	}

	private void Update()
	{
		if (!CanAT || _isStopping)
		{
			return;
		}
		if (!father || !point)
		{
			Stop();
			return;
		}
		if (!PL || !PL.IsAlive)
		{
			Back();
		}
		else
		{
			if ((bool)target && !target.IsAlive)
			{
				target = null;
			}
			if ((bool)target)
			{
				Battle();
			}
			else if (father.em != null && father.em.Count > 0)
			{
				int index = Random.Range(0, father.em.Count);
				target = father.em[index];
			}
			else
			{
				Back();
			}
		}
		if (!CanFX)
		{
			timeB += Time.deltaTime;
			if (timeB >= FXcd)
			{
				FXcd = Random.Range(0.3f, 0.5f);
				CanFX = true;
				timeB = 0f;
			}
		}
	}

	public void Back()
	{
		if (!father || !point)
		{
			Stop();
			return;
		}
		float num = Vector2.Distance(base.transform.position, point.position);
		if (num < 0.1f)
		{
			base.transform.position = point.position;
			Vector3 right = point.right;
			float num2 = Mathf.Atan2(right.y, right.x) * 57.29578f;
			base.transform.rotation = Quaternion.Euler(0f, 0f, num2 + 90f);
		}
		else
		{
			base.transform.position += base.transform.right * (speed * Time.deltaTime);
			float num3 = Mathf.Max(num, 0.01f);
			base.transform.right = Vector3.Slerp(base.transform.right, point.position - base.transform.position, father.lerpAngle / num3);
		}
	}

	public void Battle()
	{
		if (!father || !target || !target.IsAlive)
		{
			target = null;
			Back();
			return;
		}
		Vector3 position = target.yao.transform.position;
		if (Follow)
		{
			float num = Vector2.Distance(base.transform.position, position);
			if (num < 0.2f)
			{
				Follow = false;
			}
			base.transform.position += base.transform.right * (speed * Time.deltaTime);
			float num2 = Mathf.Max(num, 0.01f);
			base.transform.right = Vector3.Slerp(base.transform.right, position - base.transform.position, father.lerpAngle / num2);
		}
		else
		{
			base.transform.Translate(Vector2.right * (speed * Time.deltaTime));
			timeA += Time.deltaTime;
			float num3 = Mathf.Max(speed, 0.01f);
			if (timeA >= 1f / num3)
			{
				timeA = 0f;
				Follow = true;
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
		if ((bool)father && (bool)point)
		{
			CanAT = true;
			if ((bool)MainCOL)
			{
				MainCOL.enabled = true;
			}
		}
	}

	private void OnDisable()
	{
		ReleasePrefabCount();
		SK_FlySowrdFSQ sK_FlySowrdFSQ = father;
		father = null;
		target = null;
		point = null;
		if ((bool)MainCOL)
		{
			MainCOL.enabled = false;
		}
		if ((bool)sK_FlySowrdFSQ)
		{
			sK_FlySowrdFSQ.NotifySwordDespawn(this);
		}
	}

	public void Stop()
	{
		if (!_isStopping)
		{
			_isStopping = true;
			CanAT = false;
			if ((bool)MainCOL)
			{
				MainCOL.enabled = false;
			}
			SK_FlySowrdFSQ sK_FlySowrdFSQ = father;
			ReleasePrefabCount();
			father = null;
			target = null;
			point = null;
			if ((bool)sK_FlySowrdFSQ)
			{
				sK_FlySowrdFSQ.NotifySwordDespawn(this);
			}
			LeanPool.Despawn(this);
		}
	}

	private void ReleasePrefabCount()
	{
		if (countedPrefab)
		{
			if ((bool)PL && countedPrefabType >= 0)
			{
				PL.PrefabCount(countedPrefabType, add: false);
			}
			countedPrefab = false;
			countedPrefabType = -1;
		}
	}

	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (_isStopping || !father || !father.sp || !collision.CompareTag("BodyCOL"))
		{
			return;
		}
		BodyCOL component = collision.GetComponent<BodyCOL>();
		if ((bool)component && !(component.peo == null) && (bool)component.peo.em && component.peo.CharacterType == 2 && component.peo.em.IsAlive && !component.peo.em.IsJump && !component.peo.em.IsYS)
		{
			component.peo.EM_Set(father.sp, DotMulti, 0, Dot_Infect: false, 0, 0f);
			if (CanFX && father.FX != null && father.sp.MainEL >= 0 && father.sp.MainEL < father.FX.Length && (bool)father.FX[father.sp.MainEL])
			{
				LeanPool.Spawn(father.FX[father.sp.MainEL], component.peo.em.yao.transform.position, Quaternion.identity, component.peo.em.yao.transform);
				CanFX = false;
			}
			int num = Mathf.Min(father.em.Count, 5);
			if (num > 0)
			{
				target = father.em[Random.Range(0, num)];
			}
			else
			{
				target = null;
			}
		}
	}
}
