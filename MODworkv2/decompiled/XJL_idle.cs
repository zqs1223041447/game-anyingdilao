using FinkFramework.Runtime.Singleton;
using UnityEngine;

public class XJL_idle : XJL_Stat
{
	private XJL go;

	public float timeA;

	public float RD;

	public void Enter(XJL parent)
	{
		timeA = 0f;
		RD = Random.Range(0.5f, 2f);
		go = parent;
	}

	public void Exit()
	{
	}

	public void Update()
	{
		if (go.father.DropOBJ.Count > 0)
		{
			if (Vector2.Distance(go.transform.position, SingletonMonoScope<PlayerManager>.Instance.transform.position) < 5f)
			{
				go.ChangeState(new XJL_walk());
			}
			else
			{
				go.ChangeState(new XJL_FL());
			}
		}
		else if (Vector2.Distance(go.transform.position, SingletonMonoScope<PlayerManager>.Instance.transform.position) < 3f)
		{
			timeA += Time.deltaTime;
			if (timeA >= RD)
			{
				go.ChangeState(new XJL_XL());
				timeA = 0f;
			}
		}
		else
		{
			go.ChangeState(new XJL_FL());
		}
	}
}
