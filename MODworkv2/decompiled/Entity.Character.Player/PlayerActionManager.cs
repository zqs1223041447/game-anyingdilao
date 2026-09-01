using System;
using FinkFramework.Runtime.Singleton;
using Inputs;
using Inputs.Gamepad;
using Spine;

namespace Entity.Character.Player;

public class PlayerActionManager : SingletonMonoScope<PlayerActionManager>
{
	private ACT_skillBT[] skillBT;

	private ACT_UseBT[] useBT;

	private ACTbar actBar;

	private PlayerManager _playerManager;

	private static readonly ControlAction[] SkillActions = new ControlAction[8]
	{
		ControlAction.Skill1,
		ControlAction.Skill2,
		ControlAction.Skill3,
		ControlAction.Skill4,
		ControlAction.Skill5,
		ControlAction.Skill6,
		ControlAction.Skill7,
		ControlAction.Skill8
	};

	protected override void OnSingletonAwake()
	{
		SingletonMonoGlobal<SessionManager>.Instance.Attach(this, ProcessScope.Game);
		actBar = SingletonMonoScope<ACTbar>.Instance;
		_playerManager = SingletonMonoScope<PlayerManager>.Instance;
		skillBT = actBar.skillBT;
		useBT = actBar.useBT;
	}

	public void TryUseSkillDown(ControlAction action)
	{
		if (!_playerManager)
		{
			if (!SingletonMonoScope<PlayerManager>.HasInstance)
			{
				return;
			}
			_playerManager = SingletonMonoScope<PlayerManager>.Instance;
		}
		if (!_playerManager.IsAlive || _playerManager.IsSkill)
		{
			return;
		}
		int num = ActionToSkillIndex(action);
		if (num < 0 || skillBT == null || num >= skillBT.Length || !skillBT[num] || !skillBT[num].actL)
		{
			return;
		}
		if (!_playerManager.IsAttack)
		{
			if (skillBT[num].Opened && !skillBT[num].actL.IsCD && _playerManager.ManaStat.Cur >= skillBT[num].actL.DT.ManaCost && !ShouldDeferSampleSkillForSameShortcut(action, skillBT[num].actL))
			{
				UseSkill(num);
			}
		}
		else if (!skillBT[num].actL.DT.SampleSkill && skillBT[num].Opened && !skillBT[num].actL.IsCD && _playerManager.ManaStat.Cur >= skillBT[num].actL.DT.ManaCost)
		{
			UseSkill(num);
		}
	}

	public void TryUseSkillHold(ControlAction action)
	{
		if (_playerManager.IsAlive)
		{
			int num = ActionToSkillIndex(action);
			if (num >= 0 && skillBT != null && num < skillBT.Length && (bool)skillBT[num] && (bool)skillBT[num].actL && !_playerManager.IsSkill && !_playerManager.IsAttack && skillBT[num].Opened && !skillBT[num].actL.IsCD && _playerManager.ManaStat.Cur >= skillBT[num].actL.DT.ManaCost && !ShouldDeferSampleSkillForSameShortcut(action, skillBT[num].actL))
			{
				UseSkill(num);
				_playerManager.PauseMousePathForHeldMouseSkill(action);
			}
		}
	}

	public void TryUseItem(int index)
	{
		if (index < 0 || index >= useBT.Length || useBT[index].IsCD)
		{
			return;
		}
		if (SingletonMonoScope<ACTbar>.HasInstance)
		{
			SingletonMonoScope<ACTbar>.Instance.RefreshUseBindingStack();
		}
		if (!useBT[index].Opend || string.IsNullOrEmpty(useBT[index].IndexName))
		{
			return;
		}
		if (useBT[index].stackSize <= 0)
		{
			if (SingletonMonoScope<ACTbar>.HasInstance)
			{
				if (!SingletonMonoScope<ACTbar>.Instance.GetAutoReplaceUseBinding())
				{
					return;
				}
				SingletonMonoScope<ACTbar>.Instance.ExchangeUseFromActbar(index);
				SingletonMonoScope<ACTbar>.Instance.RefreshUseBindingStack();
			}
			if (!useBT[index].Opend || string.IsNullOrEmpty(useBT[index].IndexName) || useBT[index].stackSize <= 0)
			{
				return;
			}
		}
		UseItem(index, useBT[index].IndexName);
	}

	public void UseSkill(int index)
	{
		switch (skillBT[index].actL.DT.type)
		{
		case 0:
		{
			ACT_skillData dT2 = skillBT[index].actL.DT;
			skillBT[index].actL.IsCD = true;
			ApplySkillCastCost(dT2);
			TrackEntry attackTrack2 = _playerManager.AttackTrack;
			TrackEntry skillTrack2 = _playerManager.SkillTrack;
			_playerManager.IScomp = false;
			_playerManager.CurUseSK = index;
			bool flag2 = IsProtectedAttackAnimationSkill(skillBT[index].actL);
			_playerManager.IsAttackAnimationSkill = flag2;
			if (flag2)
			{
				_playerManager.IsSkill = true;
			}
			SingletonMonoScope<Gun>.Instance.QueueCastSnapshot(skillBT[index].actL, AimProvider.GetAimWorldPos(), index + 1);
			_playerManager.PlayerSP(skillBT[index].actL.DT.UseAni);
			SingletonMonoScope<Gun>.Instance.BindQueuedCastSnapshotToTrack(GetNewCastTrack(attackTrack2, skillTrack2));
			break;
		}
		case 1:
		{
			ACT_skillData dT = skillBT[index].actL.DT;
			skillBT[index].actL.IsCD = true;
			ApplySkillCastCost(dT);
			TrackEntry attackTrack = _playerManager.AttackTrack;
			TrackEntry skillTrack = _playerManager.SkillTrack;
			_playerManager.IScomp = true;
			_playerManager.CurUseSK = index;
			bool flag = IsProtectedAttackAnimationSkill(skillBT[index].actL);
			_playerManager.IsAttackAnimationSkill = flag;
			if (flag)
			{
				_playerManager.IsSkill = true;
			}
			SingletonMonoScope<Gun>.Instance.QueueCastSnapshot(skillBT[index].actL, AimProvider.GetAimWorldPos(), index + 1);
			_playerManager.PlayerCP(skillBT[index].actL.DT.UseAni);
			SingletonMonoScope<Gun>.Instance.BindQueuedCastSnapshotToTrack(GetNewCastTrack(attackTrack, skillTrack));
			break;
		}
		}
	}

	private bool IsProtectedAttackAnimationSkill(ACTListSkillBT skill)
	{
		if ((bool)skill && skill.DT != null && skill.DT.UseAni == 0)
		{
			return !skill.DT.SampleSkill;
		}
		return false;
	}

	private TrackEntry GetNewCastTrack(TrackEntry oldAttackTrack, TrackEntry oldSkillTrack)
	{
		if (_playerManager.SkillTrack != oldSkillTrack && _playerManager.SkillTrack != null)
		{
			return _playerManager.SkillTrack;
		}
		if (_playerManager.AttackTrack != oldAttackTrack && _playerManager.AttackTrack != null)
		{
			return _playerManager.AttackTrack;
		}
		return null;
	}

	private void ApplySkillCastCost(ACT_skillData dt)
	{
		_playerManager.ApplySkillCastCost(dt);
	}

	public void UseItem(int index, string indexName)
	{
		SingletonMonoScope<InventoryManager>.Instance.UseItemACT(indexName, index);
	}

	private static int ActionToSkillIndex(ControlAction action)
	{
		int num = ((SingletonMonoScope<ACTbar>.HasInstance && SingletonMonoScope<ACTbar>.Instance.skillBT != null) ? SingletonMonoScope<ACTbar>.Instance.skillBT.Length : 0);
		switch (action)
		{
		case ControlAction.Skill3:
			return 0;
		case ControlAction.Skill4:
			return 1;
		case ControlAction.Skill5:
			return 2;
		case ControlAction.Skill6:
			return 3;
		case ControlAction.Skill7:
			if (num < 8)
			{
				return -1;
			}
			return 4;
		case ControlAction.Skill8:
			if (num < 8)
			{
				return 4;
			}
			return 5;
		case ControlAction.Skill1:
			if (num < 8)
			{
				return 5;
			}
			return 6;
		case ControlAction.Skill2:
			if (num < 8)
			{
				return 6;
			}
			return 7;
		default:
			return -1;
		}
	}

	private bool ShouldDeferSampleSkillForSameShortcut(ControlAction action, ACTListSkillBT currentSkill)
	{
		if (!currentSkill || currentSkill.DT == null || !currentSkill.DT.SampleSkill)
		{
			return false;
		}
		string bindKeyName = InputBind.GetBindKeyName(action);
		if (string.IsNullOrWhiteSpace(bindKeyName))
		{
			return false;
		}
		for (int i = 0; i < SkillActions.Length; i++)
		{
			ControlAction controlAction = SkillActions[i];
			if (controlAction == action)
			{
				continue;
			}
			string bindKeyName2 = InputBind.GetBindKeyName(controlAction);
			if (!string.Equals(bindKeyName, bindKeyName2, StringComparison.OrdinalIgnoreCase))
			{
				continue;
			}
			int num = ActionToSkillIndex(controlAction);
			if (num >= 0 && skillBT != null && num < skillBT.Length)
			{
				ACT_skillBT aCT_skillBT = skillBT[num];
				if ((bool)aCT_skillBT && aCT_skillBT.Opened && (bool)aCT_skillBT.actL && !(aCT_skillBT.actL == currentSkill) && CanReleaseNonSampleSkill(aCT_skillBT.actL))
				{
					return true;
				}
			}
		}
		return false;
	}

	private bool CanReleaseNonSampleSkill(ACTListSkillBT skill)
	{
		if ((bool)skill && skill.DT != null && !skill.DT.SampleSkill && !skill.IsCD)
		{
			return _playerManager.ManaStat.Cur >= skill.DT.ManaCost;
		}
		return false;
	}
}
