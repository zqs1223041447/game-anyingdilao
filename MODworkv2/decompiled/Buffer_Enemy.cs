using UnityEngine;

public class Buffer_Enemy : MonoBehaviour
{
	public BuffMG_EM mg;

	public Buff_Enemy buff;

	public float BuffTime;

	public float JStime;

	private void Awake()
	{
	}

	private void Start()
	{
	}

	private void OnEnable()
	{
		mg = base.transform.parent.GetComponent<BuffMG_EM>();
		JStime = 0f;
	}

	private void Update()
	{
		JStime += Time.deltaTime;
		if (JStime >= buff.BuffTime)
		{
			DelBuff();
			JStime = 0f;
		}
	}

	public void AddBuff(Buff_Enemy enemy, float DotTimeCut)
	{
		JStime = 0f;
		if (enemy.type == 0)
		{
			enemy.BuffTime *= 1f - DotTimeCut / 100f;
		}
		buff = enemy;
	}

	public void DelBuff()
	{
		mg.DelBuff(buff, this);
	}
}
