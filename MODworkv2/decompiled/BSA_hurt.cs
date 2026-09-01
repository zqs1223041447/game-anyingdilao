using FinkFramework.Runtime.Singleton;
using UnityEngine;

public class BSA_hurt : BossAstat
{
	private BossA go;

	private float timeJiTui;

	private float SpeedTmp;

	public void Enter(BossA parent)
	{
		go = parent;
		go.changeST(4);
		if (!go.em.IS_Frozen)
		{
			go.tar.transform.position = go.tar2.transform.position;
		}
		go.em.IsAttack = false;
		timeJiTui = 0f;
		SpeedTmp = go.em.TuiSpeed;
	}

	public void Exit()
	{
		go.em.IsYun = false;
		timeJiTui = 0f;
	}

	public void Update()
	{
		if (go.em.IsAlive)
		{
			if (go.em.IsYun)
			{
				timeJiTui += Time.deltaTime;
				if (timeJiTui < 0.1f)
				{
					SpeedTmp = Mathf.Lerp(SpeedTmp, 0f, Time.deltaTime * 10f);
					go.em.transform.Translate((go.em.transform.position - SingletonMonoScope<PlayerManager>.Instance.transform.position).normalized * (SpeedTmp * Time.deltaTime));
				}
			}
		}
		else
		{
			go.ChangeState(new BSA_die());
		}
	}
}
