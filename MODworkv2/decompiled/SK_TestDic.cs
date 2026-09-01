using FinkFramework.Runtime.Singleton;
using Lean.Pool;
using UnityEngine;

public class SK_TestDic : MonoBehaviour
{
	public GameObject[] OBJ;

	private float timeA;

	public float LifeTime;

	public int FasheType;

	public int FSnumber;

	public float FStime;

	public float angleRange;

	public SkillOBJ_DT_SP sp;

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
		Dicform component = LeanPool.Spawn(OBJ[SingletonMonoScope<Gun>.Instance.Index], base.transform.position, Quaternion.identity).GetComponent<Dicform>();
		component.sp = sp;
		component.SetCount(sp.ZY);
		component.SubType = 0;
		component.dic = SingletonMonoScope<PlayerManager>.Instance.mainCam.ScreenToWorldPoint(Input.mousePosition) - base.transform.position;
		float num = Vector2.Distance(SingletonMonoScope<PlayerManager>.Instance.mainCam.ScreenToWorldPoint(Input.mousePosition), base.transform.position);
		if (num > 7f)
		{
			num = 7f;
		}
		component.speed = num * 2f;
	}
}
