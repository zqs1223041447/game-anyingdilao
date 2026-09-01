using System.Collections.Generic;
using Core;
using Core.Settings;
using Dialog;
using FinkFramework.Runtime.Singleton;
using Mijing;
using SaveSystem;
using UI.Panels;
using UnityEngine.Events;

namespace Level.LevelStates;

public class BossLevelManager : SingletonMonoScene<BossLevelManager>
{
	private readonly HashSet<Boss> currentBosses = new HashSet<Boss>();

	private bool bossBattleStarted;

	public int AliveBossCount => currentBosses.Count;

	public event UnityAction OnAllBossDefeated;

	protected override void Awake()
	{
		base.Awake();
		currentBosses.Clear();
	}

	public void RegisterBoss(Boss boss)
	{
		if ((bool)boss && (!SingletonMonoScope<LevelManager>.HasInstance || LevelManager.GetIsBoss()))
		{
			currentBosses.Add(boss);
			bossBattleStarted = true;
		}
	}

	public void UnregisterBoss(Boss boss)
	{
		if (!boss || !currentBosses.Remove(boss) || !bossBattleStarted || currentBosses.Count != 0 || !SingletonMonoScope<LevelManager>.HasInstance)
		{
			return;
		}
		string curLevel = LevelManager.GetCurLevel();
		bool flag = SaveManager.RuntimeData.DefeatedBossLevelIds.Add(curLevel);
		if (!flag)
		{
			TryUnlockMijingOnFinalBossDefeated();
		}
		if (LevelManager.GetIsFinal() && flag)
		{
			if (SingletonMonoScope<MijingManager>.HasInstance)
			{
				if (SettingsLoader.Instance.MijingToggle)
				{
					GameManager.ShowTip(LOC.MM.GetLevel("mijing_unlock"), TipType.Success, 4f);
					if (SingletonMonoScope<DialogManager>.HasInstance)
					{
						SingletonMonoScope<DialogManager>.Instance.MarkTriggered("mijing_unlocked");
					}
					SaveManager.RuntimeData.UnlockedMijing = true;
				}
				else
				{
					GameManager.ShowTip(LOC.MM.GetLevel("test_over"), TipType.Success, 4f);
				}
			}
			if (SingletonMonoScope<ItemManager>.HasInstance && !LevelManager.GetIsMijing())
			{
				SingletonMonoScope<ItemManager>.Instance.BossDropFirst(boss.transform, boss.em.ItemDropPos, isChapterFinal: true);
			}
			SingletonMonoScope<AutoSaveManager>.Instance.TrySaveWithIcon();
		}
		else if (flag)
		{
			if (LevelManager.GetIsCurChapterFinal())
			{
				SaveManager.RuntimeData.UnlockedLevelIds.Add(LevelManager.GetFirstMainLevelIdInNextChapter(curLevel));
				SaveManager.RuntimeData.UnlockedChapterIds.Add(LevelManager.GetChapterId(curLevel) + 1);
				GameManager.ShowTip(LOC.MM.GetLevel("new_lock"), TipType.Success, 4f);
				if (SingletonMonoScope<ItemManager>.HasInstance && !LevelManager.GetIsMijing())
				{
					SingletonMonoScope<ItemManager>.Instance.BossDropFirst(boss.transform, boss.em.ItemDropPos, isChapterFinal: true);
				}
				SingletonMonoScope<AutoSaveManager>.Instance.TrySaveWithIcon();
			}
			else if (!LevelManager.GetIsCurChapterFinal())
			{
				if (SingletonMonoScope<ItemManager>.HasInstance && !LevelManager.GetIsMijing())
				{
					SingletonMonoScope<ItemManager>.Instance.BossDropFirst(boss.transform, boss.em.ItemDropPos, isChapterFinal: false);
				}
				SingletonMonoScope<AutoSaveManager>.Instance.TrySaveWithIcon();
			}
		}
		this.OnAllBossDefeated?.Invoke();
	}

	private static void TryUnlockMijingOnFinalBossDefeated()
	{
		if (!LevelManager.GetIsFinal() || !SingletonMonoScope<MijingManager>.HasInstance)
		{
			return;
		}
		if (!SettingsLoader.Instance.MijingToggle)
		{
			GameManager.ShowTip(LOC.MM.GetLevel("test_over"), TipType.Success, 4f);
		}
		else if (!SaveManager.RuntimeData.UnlockedMijing)
		{
			GameManager.ShowTip(LOC.MM.GetLevel("mijing_unlock"), TipType.Success, 4f);
			if (SingletonMonoScope<DialogManager>.HasInstance)
			{
				SingletonMonoScope<DialogManager>.Instance.MarkTriggered("mijing_unlocked");
			}
			SaveManager.RuntimeData.UnlockedMijing = true;
		}
	}

	private void OnDestroy()
	{
		this.OnAllBossDefeated = null;
	}
}
