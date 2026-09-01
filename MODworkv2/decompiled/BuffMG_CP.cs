using System.Collections.Generic;
using FinkFramework.Runtime.Singleton;
using Lean.Pool;
using UnityEngine;

public class BuffMG_CP : MonoBehaviour
{
	public Companion cp;

	public List<Buffer_CP> list = new List<Buffer_CP>();

	public List<DOTobj> dtOBJ = new List<DOTobj>();

	private void Awake()
	{
		cp = base.transform.parent.GetComponent<Companion>();
	}

	public void AddBuff(Buff_CP comp)
	{
		if (!SingletonMonoScope<GameDataManager>.HasInstance)
		{
			return;
		}
		Buffer_CP component = LeanPool.Spawn(SingletonMonoScope<GameDataManager>.Instance.buffer_cp, base.transform.position, Quaternion.identity, base.transform).GetComponent<Buffer_CP>();
		switch (comp.type)
		{
		case 0:
			cp.AttackSpeed_Cut += comp.De_ATSpeedCut;
			cp.MoveSpeed_Cut += comp.De_MVSpeedCut;
			if (comp.DotDamage > 0f && !HasSameDOT(comp))
			{
				dtOBJ.Add(SingletonMonoScope<ACTbar>.Instance.TakeDotFX(cp.body.transform, comp.damageType, cp.size));
			}
			break;
		case 1:
			cp.Damage_Bei += comp.Damage;
			cp.AttackSpeed_Bei += comp.AttackSpeed;
			cp.MoveSpeed_Bei += comp.MoveSpeed;
			cp.Health_Prc_Bei += comp.Health_Prc;
			break;
		}
		list.Add(component);
		component.AddBuff(comp);
		this.wait(0.0001f, SetAni);
	}

	public void DelBuff(Buff_CP comp, Buffer_CP buffer)
	{
		GameObject clone = buffer.gameObject;
		list.Remove(buffer);
		LeanPool.Despawn(clone);
		switch (comp.type)
		{
		case 0:
		{
			cp.AttackSpeed_Cut -= comp.De_ATSpeedCut;
			cp.MoveSpeed_Cut -= comp.De_MVSpeedCut;
			if (HasSameDOT(comp))
			{
				break;
			}
			for (int i = 0; i < dtOBJ.Count; i++)
			{
				if (dtOBJ[i].damageType == comp.damageType)
				{
					GameObject clone2 = dtOBJ[i].gameObject;
					dtOBJ.Remove(dtOBJ[i]);
					LeanPool.Despawn(clone2);
				}
			}
			break;
		}
		case 1:
			cp.Damage_Bei -= comp.Damage;
			cp.AttackSpeed_Bei -= comp.AttackSpeed;
			cp.MoveSpeed_Bei -= comp.MoveSpeed;
			cp.Health_Prc_Bei -= comp.Health_Prc;
			break;
		}
		this.wait(0.0001f, SetAni);
	}

	public void DelAll()
	{
		for (int num = list.Count - 1; num > -1; num--)
		{
			list[num].DelBuff();
		}
	}

	public void DelAllDebuff()
	{
		for (int num = list.Count - 1; num > -1; num--)
		{
			if (list[num].buff.type == 0)
			{
				list[num].DelBuff();
			}
		}
	}

	public bool HasSameDOT(Buff_CP comp)
	{
		if (list.Count > 0)
		{
			int num = 0;
			for (int i = 0; i < list.Count; i++)
			{
				if (list[i].buff.type == 0 && list[i].buff.damageType == comp.damageType)
				{
					num++;
				}
			}
			if (num > 0)
			{
				return true;
			}
			return false;
		}
		return false;
	}

	public void SetAni()
	{
		cp.MoveTrack.TimeScale = cp.MoveSpeed_Last;
		cp.AttackTrack.TimeScale = cp.AttackSpeed_Last;
		cp.SkillTrack.TimeScale = cp.SkillSpeed_Max;
	}
}
