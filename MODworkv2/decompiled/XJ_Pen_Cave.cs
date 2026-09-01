using FinkFramework.Runtime.Singleton;
using Lean.Pool;
using UnityEngine;

public class XJ_Pen_Cave : MonoBehaviour
{
	public int type;

	public float Damage;

	public float ATtime;

	private float JStime;

	public GameObject obj;

	[SerializeField]
	private GameObject point;

	[SerializeField]
	private Collider2D col;

	public PlayerManager playerManager;

	public int AudioAT;

	private void Start()
	{
		point = base.transform.Find("point").gameObject;
		playerManager = SingletonMonoScope<PlayerManager>.Instance;
	}

	private void Update()
	{
		JStime += Time.deltaTime;
		if (JStime >= ATtime)
		{
			JStime = 0f;
			if (Vector2.Distance(base.transform.position, playerManager.transform.position) < 13f)
			{
				LeanPool.Spawn(obj, point.transform.position, Quaternion.identity, point.transform);
			}
		}
	}
}
