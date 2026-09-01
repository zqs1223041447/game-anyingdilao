using FinkFramework.Runtime.Singleton;
using Lean.Pool;
using UnityEngine;

public class SK_TestPos : MonoBehaviour
{
	public GameObject[] OBJ;

	public bool HasAngle;

	public float LifeTime;

	public int FSnumber;

	public float FStime;

	public float angleRange;

	[HideInInspector]
	public Transform trans;

	[HideInInspector]
	public SkillOBJ_DT_SP sp;

	private float timeA;

	private void Awake()
	{
		sp = GetComponent<SkillOBJ_DT_SP>();
	}

	private void OnEnable()
	{
		timeA = 0f;
		this.wait(0.0001f, FaShe);
	}

	private void Update()
	{
		timeA += Time.deltaTime;
		if (timeA > LifeTime)
		{
			timeA = 0f;
			LeanPool.Despawn(this);
		}
	}

	public void FaShe()
	{
		Vector3 vector = SingletonMonoScope<PlayerManager>.Instance.mainCam.ScreenToWorldPoint(Input.mousePosition) - SingletonMonoScope<PlayerManager>.Instance.transform.position;
		float z = Mathf.Atan2(vector.y, vector.x) * 57.29578f;
		Dicform component = ((!HasAngle) ? LeanPool.Spawn(OBJ[SingletonMonoScope<Gun>.Instance.Index], base.transform.position, Quaternion.identity) : LeanPool.Spawn(OBJ[SingletonMonoScope<Gun>.Instance.Index], base.transform.position, Quaternion.Euler(0f, 0f, z))).GetComponent<Dicform>();
		component.sp = sp;
		component.SetCount(sp.ZY);
		component.SubType = 0;
	}
}
