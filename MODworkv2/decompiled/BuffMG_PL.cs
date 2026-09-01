using System.Collections.Generic;
using FinkFramework.Runtime.Singleton;
using Lean.Pool;
using UnityEngine;

public class BuffMG_PL : MonoBehaviour
{
	public PlayerManager pl;

	public List<Buffer_PL> list = new List<Buffer_PL>();

	public DOTobj[] dtOBJ;

	private void Awake()
	{
		pl = base.transform.parent.GetComponent<PlayerManager>();
		for (int i = 0; i < dtOBJ.Length; i++)
		{
			dtOBJ[i].DotType = i;
		}
	}

	public void ADDA()
	{
		Buff_PL buff_PL = new Buff_PL();
		buff_PL.type = 0;
		buff_PL.BuffTime = 10f;
		buff_PL.damageType = DamageType.physics;
		buff_PL.De_ATSpeedCut = 80f;
		buff_PL.De_MVSpeedCut = 80f;
		buff_PL.EL_Damage = 80f;
		buff_PL.De_MVSpeedCut = 80f;
		buff_PL.DotDamage = 80f;
		buff_PL.Damage = 80f;
		AddBuff(buff_PL);
	}

	public void AddBuff(Buff_PL buff)
	{
		if (buff == null || !SingletonMonoScope<GameDataManager>.HasInstance || (buff.type == 0 && (bool)pl && pl.IsBurnLifeEffectActive))
		{
			return;
		}
		ApplyPlayerDebuffTimeCut(buff);
		Buffer_PL component = LeanPool.Spawn(SingletonMonoScope<GameDataManager>.Instance.buffer_pl, base.transform.position, Quaternion.identity, base.transform).GetComponent<Buffer_PL>();
		switch (buff.type)
		{
		case 0:
			pl.ATSpeed_Tmp_Cut += buff.De_ATSpeedCut;
			pl.MVSpeed_Tmp_Cut += buff.De_MVSpeedCut;
			if (!HasSameDOT(buff))
			{
				switch (buff.damageType)
				{
				case DamageType.fire:
					dtOBJ[0].gameObject.SetActive(value: true);
					break;
				case DamageType.frozen:
					dtOBJ[1].gameObject.SetActive(value: true);
					break;
				case DamageType.thunder:
					dtOBJ[2].gameObject.SetActive(value: true);
					break;
				case DamageType.poison:
					dtOBJ[3].gameObject.SetActive(value: true);
					break;
				case DamageType.physics:
					dtOBJ[4].gameObject.SetActive(value: true);
					break;
				case DamageType.shadow:
					dtOBJ[5].gameObject.SetActive(value: true);
					break;
				}
			}
			break;
		case 1:
			pl.Damage_Bei_Tmp += buff.Damage;
			switch (buff.damageType)
			{
			case DamageType.fire:
				pl.FireDamage_Bei_Tmp += buff.EL_Damage;
				pl.FireChuan_Tmp += buff.Chuan;
				break;
			case DamageType.frozen:
				pl.FrozenDamage_Bei_Tmp += buff.EL_Damage;
				pl.FrozenChuan_Tmp += buff.Chuan;
				break;
			case DamageType.thunder:
				pl.ThunderDamage_Bei_Tmp += buff.EL_Damage;
				pl.ThunderChuan_Tmp += buff.Chuan;
				break;
			case DamageType.poison:
				pl.PoisonDamage_Bei_Tmp += buff.EL_Damage;
				pl.PoisonChuan_Tmp += buff.Chuan;
				break;
			case DamageType.physics:
				pl.PhysicsDamage_Bei_Tmp += buff.EL_Damage;
				pl.PhysicsChuan_Tmp += buff.Chuan;
				break;
			case DamageType.shadow:
				pl.ShadowDamage_Bei_Tmp += buff.EL_Damage;
				pl.ShadowChuan_Tmp += buff.Chuan;
				break;
			}
			pl.BJrate_Tmp += buff.BJrate;
			pl.JYrate_Tmp += buff.JYrate;
			pl.GeDang_Tmp += buff.GeDang;
			pl.ATSpeed_Tmp += buff.AttackSpeed;
			pl.MVSpeed_Tmp += buff.MoveSpeed;
			pl.Damage_Anti_Tmp += buff.DamageAnti;
			pl.Health_Percent_Tmp += buff.Health_Prc;
			break;
		}
		list.Add(component);
		component.AddBuff(buff);
	}

	private void ApplyPlayerDebuffTimeCut(Buff_PL buff)
	{
		if (buff != null && buff.type == 0 && (bool)pl && !(buff.BuffTime <= 0f))
		{
			float dOTcut_Last = pl.DOTcut_Last;
			if (!(dOTcut_Last <= 0f))
			{
				buff.BuffTime -= buff.BuffTime * dOTcut_Last / 100f;
			}
		}
	}

	public void DelBuff(Buff_PL bf, Buffer_PL buffer)
	{
		GameObject clone = buffer.gameObject;
		list.Remove(buffer);
		LeanPool.Despawn(clone);
		switch (bf.type)
		{
		case 0:
			pl.ATSpeed_Tmp_Cut -= bf.De_ATSpeedCut;
			pl.MVSpeed_Tmp_Cut -= bf.De_MVSpeedCut;
			if (!HasSameDOT(bf))
			{
				switch (bf.damageType)
				{
				case DamageType.fire:
					dtOBJ[0].gameObject.SetActive(value: false);
					break;
				case DamageType.frozen:
					dtOBJ[1].gameObject.SetActive(value: false);
					break;
				case DamageType.thunder:
					dtOBJ[2].gameObject.SetActive(value: false);
					break;
				case DamageType.poison:
					dtOBJ[3].gameObject.SetActive(value: false);
					break;
				case DamageType.physics:
					dtOBJ[4].gameObject.SetActive(value: false);
					break;
				case DamageType.shadow:
					dtOBJ[5].gameObject.SetActive(value: false);
					break;
				}
			}
			break;
		case 1:
			pl.Damage_Bei_Tmp -= bf.Damage;
			switch (bf.damageType)
			{
			case DamageType.fire:
				pl.FireDamage_Bei_Tmp -= bf.EL_Damage;
				pl.FireChuan_Tmp -= bf.Chuan;
				break;
			case DamageType.frozen:
				pl.FrozenDamage_Bei_Tmp -= bf.EL_Damage;
				pl.FrozenChuan_Tmp -= bf.Chuan;
				break;
			case DamageType.thunder:
				pl.ThunderDamage_Bei_Tmp -= bf.EL_Damage;
				pl.ThunderChuan_Tmp -= bf.Chuan;
				break;
			case DamageType.poison:
				pl.PoisonDamage_Bei_Tmp -= bf.EL_Damage;
				pl.PoisonChuan_Tmp -= bf.Chuan;
				break;
			case DamageType.physics:
				pl.PhysicsDamage_Bei_Tmp -= bf.EL_Damage;
				pl.PhysicsChuan_Tmp -= bf.Chuan;
				break;
			case DamageType.shadow:
				pl.ShadowDamage_Bei_Tmp -= bf.EL_Damage;
				pl.ShadowChuan_Tmp -= bf.Chuan;
				break;
			}
			pl.BJrate_Tmp -= bf.BJrate;
			pl.JYrate_Tmp -= bf.JYrate;
			pl.GeDang_Tmp -= bf.GeDang;
			pl.ATSpeed_Tmp -= bf.AttackSpeed;
			pl.MVSpeed_Tmp -= bf.MoveSpeed;
			pl.Damage_Anti_Tmp -= bf.DamageAnti;
			pl.Health_Percent_Tmp -= bf.Health_Prc;
			break;
		}
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

	public bool DelOneDebuff()
	{
		for (int num = list.Count - 1; num >= 0; num--)
		{
			if ((bool)list[num] && list[num].buff != null && list[num].buff.type == 0)
			{
				list[num].DelBuff();
				return true;
			}
		}
		return false;
	}

	public bool HasSameDOT(Buff_PL bf)
	{
		if (list.Count > 0)
		{
			foreach (Buffer_PL item in list)
			{
				if (item.buff.type == 0 && item.buff.damageType == bf.damageType)
				{
					return true;
				}
			}
			return false;
		}
		return false;
	}

	public bool HasDebuff()
	{
		int num = 0;
		for (int i = 0; i < list.Count; i++)
		{
			if (list[i].buff.type == 0)
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

	public void ClearSkillBuff()
	{
		if (list.Count <= 0)
		{
			return;
		}
		int num = list.Count;
		for (int i = 0; i < num; i++)
		{
			if (list[i].buff.IsSkillBuff)
			{
				list[i].DelBuff();
				num--;
				i--;
			}
		}
	}

	public bool HasSameBuff(string BuffName)
	{
		int num = 0;
		foreach (Buffer_PL item in list)
		{
			if (item.buff.IndexName == BuffName)
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

	public int GetBuffKindCount()
	{
		HashSet<string> hashSet = new HashSet<string>();
		for (int i = 0; i < list.Count; i++)
		{
			Buffer_PL buffer_PL = list[i];
			if ((bool)buffer_PL && buffer_PL.buff != null && buffer_PL.buff.type == 1)
			{
				string item = (string.IsNullOrEmpty(buffer_PL.buff.IndexName) ? string.Concat(buffer_PL.buff.damageType, ":", buffer_PL.buff.Damage, ":", buffer_PL.buff.AttackSpeed, ":", buffer_PL.buff.MoveSpeed) : buffer_PL.buff.IndexName);
				hashSet.Add(item);
			}
		}
		return hashSet.Count;
	}
}
