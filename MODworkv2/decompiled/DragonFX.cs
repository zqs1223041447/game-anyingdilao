using FinkFramework.Runtime.Utils;
using Lean.Pool;
using UnityEngine;

public class DragonFX : MonoBehaviour
{
	public Enemy em;

	public GameObject[] DZ;

	public SK_DZ Keng;

	private bool canFX;

	private void Awake()
	{
		em = GetComponent<Enemy>();
	}

	private void OnEnable()
	{
		canFX = false;
		Keng = null;
		this.wait(0.001f, SetStart);
	}

	private void Update()
	{
		if (!canFX)
		{
			return;
		}
		if (!em)
		{
			canFX = false;
		}
		else if (!em.IsAlive)
		{
			if ((bool)Keng)
			{
				Keng.Stop();
				Keng = null;
			}
			canFX = false;
		}
	}

	public void SetStart()
	{
		if (!em)
		{
			return;
		}
		if (DZ == null || DZ.Length == 0)
		{
			LogUtil.Warn(base.name + " 的 DZ 数组未配置", this);
			return;
		}
		if (em.MainElement < 0 || em.MainElement >= DZ.Length)
		{
			LogUtil.Warn($"{base.name} 的 em.MainElement 越界: {em.MainElement}", this);
			return;
		}
		if (!DZ[em.MainElement])
		{
			LogUtil.Warn($"{base.name} 的 DZ[{em.MainElement}] 为空", this);
			return;
		}
		GameObject gameObject = LeanPool.Spawn(DZ[em.MainElement], new Vector3(base.transform.position.x, base.transform.position.y - 0.02f, base.transform.position.z), Quaternion.identity, base.transform);
		if ((bool)gameObject)
		{
			Keng = gameObject.GetComponent<SK_DZ>();
			if (!Keng)
			{
				LogUtil.Warn(DZ[em.MainElement].name + " 上没有 SK_DZ 组件", gameObject);
			}
			else
			{
				canFX = true;
			}
		}
	}
}
