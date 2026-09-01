using FinkFramework.Runtime.Singleton;
using Lean.Pool;
using UnityEngine;

public class SK_FSQ_follow : MonoBehaviour
{
	public GameObject OBJ;

	public float speed;

	public float ATtime;

	public int ATcount;

	public float angle;

	[HideInInspector]
	public Dicform dic;

	private int count;

	private float timeB;

	private void Awake()
	{
		dic = GetComponent<Dicform>();
	}

	private void Start()
	{
	}

	private void OnEnable()
	{
		timeB = 0f;
		count = 0;
	}

	private void Update()
	{
		base.transform.right = Vector3.Slerp(base.transform.right, SingletonMonoScope<PlayerManager>.Instance.transform.position - base.transform.position, angle / Vector3.Distance(SingletonMonoScope<PlayerManager>.Instance.transform.position, base.transform.position));
		base.transform.position += base.transform.right * speed * Time.deltaTime;
		timeB += Time.deltaTime;
		if (timeB >= ATtime && count < ATcount)
		{
			timeB = 0f;
			Dicform component = LeanPool.Spawn(OBJ, base.transform.position, Quaternion.identity).GetComponent<Dicform>();
			component.sp = dic.sp;
			component.SetCount(dic.sp.ZY);
			component.SubType = 0;
			count++;
			if (count == ATcount)
			{
				LeanPool.Despawn(this);
			}
		}
	}
}
