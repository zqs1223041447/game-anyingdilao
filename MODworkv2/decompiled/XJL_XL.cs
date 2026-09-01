using FinkFramework.Runtime.Singleton;
using UnityEngine;

public class XJL_XL : XJL_Stat
{
	private XJL go;

	public float time;

	private float RD;

	private float x;

	private float y;

	public void Enter(XJL parent)
	{
		go = parent;
		x = Random.Range(-1f, 1f);
		y = Random.Range(-1f, 1f);
		RD = Random.Range(0.2f, 1f);
	}

	public void Exit()
	{
		time = 0f;
	}

	public void Update()
	{
		if (Vector2.Distance(go.transform.position, SingletonMonoScope<PlayerManager>.Instance.transform.position) < 5f)
		{
			go.transform.Translate(new Vector2(x, y).normalized * (go.Movespeed / 2f * Time.deltaTime));
			time += Time.deltaTime;
			if (time > RD)
			{
				go.ChangeState(new XJL_idle());
				time = 0f;
			}
		}
		else
		{
			go.ChangeState(new XJL_FL());
		}
	}
}
