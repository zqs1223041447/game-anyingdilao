using UnityEngine;

public class Buffer_CP : MonoBehaviour
{
	public BuffMG_CP mg;

	public Buff_CP buff;

	public float BuffTime;

	public float JStime;

	public float timeA;

	private void Awake()
	{
	}

	private void Start()
	{
	}

	private void OnEnable()
	{
		mg = base.transform.parent.GetComponent<BuffMG_CP>();
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
				mg.cp.TakeDotDamage(buff.damageType, buff.DotDamage / 2f, buff.DotChuan);
				timeA = 0f;
			}
		}
	}

	public void AddBuff(Buff_CP comp)
	{
		JStime = 0f;
		buff = comp;
	}

	public void DelBuff()
	{
		mg.DelBuff(buff, this);
	}
}
