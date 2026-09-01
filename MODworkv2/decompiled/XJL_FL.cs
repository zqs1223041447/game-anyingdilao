using FinkFramework.Runtime.Singleton;
using UnityEngine;

public class XJL_FL : XJL_Stat
{
	private XJL go;

	public float time;

	private float RD;

	public void Enter(XJL parent)
	{
		go = parent;
		RD = Random.Range(0.5f, 1f);
	}

	public void Exit()
	{
		time = 0f;
	}

	public void Update()
	{
		if (Vector2.Distance(go.transform.position, SingletonMonoScope<PlayerManager>.Instance.transform.position) > RD)
		{
			go.transform.Translate((SingletonMonoScope<PlayerManager>.Instance.transform.position - go.transform.position).normalized * (go.Movespeed * Time.deltaTime));
		}
		else
		{
			go.ChangeState(new XJL_idle());
		}
	}
}
