using Lean.Pool;
using UnityEngine;

public class SK_JG_Zhuan : MonoBehaviour
{
	public GameObject FX;

	public float DotMulti;

	[HideInInspector]
	public Dicform dic;

	private bool canAT;

	private float timeA;

	public Transform hitA;

	public Transform hitB;

	private void Awake()
	{
		dic = GetComponent<Dicform>();
		hitA = base.transform.Find("hitA");
		hitB = base.transform.Find("hitB");
	}

	private void Start()
	{
	}

	private void OnEnable()
	{
		timeA = 0f;
		canAT = false;
		this.wait(1E-05f, SetStart);
	}

	private void Update()
	{
		if (canAT)
		{
			timeA += Time.deltaTime;
			if (timeA > dic.sp.BuffTime)
			{
				timeA = 0f;
				LeanPool.Spawn(FX, hitA.position, Quaternion.identity);
				LeanPool.Spawn(FX, hitB.position, Quaternion.identity);
				LeanPool.Despawn(this);
			}
		}
	}

	public void SetStart()
	{
		canAT = true;
	}
}
