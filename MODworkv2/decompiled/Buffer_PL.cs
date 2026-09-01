using UnityEngine;

public class Buffer_PL : MonoBehaviour
{
	public BuffMG_PL mg;

	public Buff_PL buff;

	public float BuffTime;

	public float JStime;

	public float timeA;

	private void OnEnable()
	{
		mg = base.transform.parent.GetComponent<BuffMG_PL>();
		JStime = 0f;
		timeA = 0f;
	}

	private void Update()
	{
		JStime += Time.deltaTime;
		if (JStime >= buff.BuffTime)
		{
			DelBuff();
			JStime = 0f;
		}
		if (buff.DotDamage > 0f)
		{
			timeA += Time.deltaTime;
			if (timeA >= 0.5f)
			{
				mg.pl.TakeDotDamage(buff.damageType, buff.DotDamage / 2f, buff.DotChuan);
				timeA = 0f;
			}
		}
	}

	public void AddBuff(Buff_PL bf)
	{
		JStime = 0f;
		buff = bf;
	}

	public void DelBuff()
	{
		mg.DelBuff(buff, this);
	}
}
