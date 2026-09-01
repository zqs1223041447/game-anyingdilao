using FinkFramework.Runtime.Singleton;
using UnityEngine;

public class XJL_walk : XJL_Stat
{
	private XJL go;

	public float timeA;

	public float timeB;

	public void Enter(XJL parent)
	{
		timeA = 0f;
		timeB = 0f;
		go = parent;
	}

	public void Exit()
	{
	}

	public void Update()
	{
		if (go.father.DropOBJ.Count < 1)
		{
			go.ChangeState(new XJL_idle());
		}
		else if (Vector2.Distance(go.transform.position, SingletonMonoScope<PlayerManager>.Instance.transform.position) < 5f)
		{
			if (!go.HasValidDropTarget())
			{
				go.ChangeState(new XJL_idle());
			}
			else if ((bool)go.tar && Vector2.Distance(go.transform.position, go.tar.position) > 0.2f)
			{
				go.transform.Translate((go.tar.position - go.transform.position).normalized * (go.Movespeed * Time.deltaTime));
			}
		}
		else
		{
			go.ChangeState(new XJL_FL());
		}
	}
}
