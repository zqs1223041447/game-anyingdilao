using System;
using FinkFramework.Runtime.Singleton;
using Inputs;
using Inputs.Cursors;
using Scenes;
using UnityEngine;
using UnityEngine.UI;

public class UI_EnemyTip : ScopedSingletonMono<UI_EnemyTip>
{
	[Header("UI")]
	public EnemyStat stat;

	public Text text;

	public Text[] SPCtext;

	public Text ELtext;

	[Header("状态")]
	public bool open;

	public float hideDelay = 1f;

	private Enemy mouseHoverEnemy;

	private Enemy hitFocusEnemy;

	private float hitFocusRemain;

	private Enemy currentTarget;

	private float hideTimer;

	private void Start()
	{
		SingletonMonoScope<GameUIManager>.Instance.ShowEnemyTip(0);
		open = false;
		hideTimer = 0f;
		hitFocusRemain = 0f;
	}

	private void Update()
	{
		if ((bool)mouseHoverEnemy && !IsEnemyValid(mouseHoverEnemy))
		{
			mouseHoverEnemy = null;
		}
		if ((bool)hitFocusEnemy && !IsEnemyValid(hitFocusEnemy))
		{
			hitFocusEnemy = null;
			hitFocusRemain = 0f;
		}
		if ((bool)currentTarget && !IsEnemyValid(currentTarget))
		{
			currentTarget = null;
		}
		if (hitFocusRemain > 0f)
		{
			hitFocusRemain -= Time.deltaTime;
			if (hitFocusRemain <= 0f)
			{
				hitFocusRemain = 0f;
				hitFocusEnemy = null;
			}
		}
		Enemy enemy = ResolveTarget();
		if ((bool)enemy)
		{
			hideTimer = 0f;
			if (!open)
			{
				open = true;
				SingletonMonoScope<GameUIManager>.Instance.ShowEnemyTip(1);
			}
			if (enemy != currentTarget)
			{
				currentTarget = enemy;
				stat.Initialize(enemy.HealthStat.CurrentValue, enemy.HealthStat.MaxValue);
				ChangeText(enemy);
				return;
			}
			stat.CurrentValue = enemy.HealthStat.CurrentValue;
			if (!Mathf.Approximately(stat.MaxValue, enemy.HealthStat.MaxValue))
			{
				stat.MaxValue = enemy.HealthStat.MaxValue;
			}
		}
		else if (open)
		{
			hideTimer += Time.deltaTime;
			if (hideTimer >= hideDelay)
			{
				HideEnemy();
			}
		}
	}

	public static void TryShowByGamepadHit(Enemy enemy, PlayerManager pl, Companion cp, float duration = 1.2f)
	{
		if ((bool)enemy && SingletonMonoScope<UI_EnemyTip>.HasInstance && (bool)pl && !cp && SingletonMonoGlobal<CurrentInputManager>.HasInstance && SingletonMonoGlobal<CurrentInputManager>.Instance.IsGamepadCurrent() && !CursorInputManager.IsUsingVirtualMouse)
		{
			SingletonMonoScope<UI_EnemyTip>.Instance.ShowByGamepadHit(enemy, duration);
		}
	}

	private Enemy ResolveTarget()
	{
		if ((bool)mouseHoverEnemy && IsEnemyValid(mouseHoverEnemy))
		{
			return mouseHoverEnemy;
		}
		if ((bool)hitFocusEnemy && hitFocusRemain > 0f && IsEnemyValid(hitFocusEnemy))
		{
			return hitFocusEnemy;
		}
		return null;
	}

	private static bool IsEnemyValid(Enemy em)
	{
		if ((bool)em && em.IsAlive && !em.IsJump && !em.IsYS)
		{
			return !em.IS_Boss;
		}
		return false;
	}

	public void NotifyMouseHoverEnter(Enemy em)
	{
		if (IsEnemyValid(em))
		{
			mouseHoverEnemy = em;
			hideTimer = 0f;
		}
	}

	public void NotifyMouseHoverExit(Enemy em)
	{
		if ((bool)em && mouseHoverEnemy == em)
		{
			mouseHoverEnemy = null;
		}
	}

	public void ShowByGamepadHit(Enemy em, float duration = 1.2f)
	{
		if (IsEnemyValid(em))
		{
			hitFocusEnemy = em;
			hitFocusRemain = duration;
			hideTimer = 0f;
			if (!open)
			{
				open = true;
				SingletonMonoScope<GameUIManager>.Instance.ShowEnemyTip(1);
			}
		}
	}

	public void HideEnemy()
	{
		stat.CurrentValue = 0f;
		stat.MaxValue = 0f;
		SingletonMonoScope<GameUIManager>.Instance.ShowEnemyTip(0);
		open = false;
		hideTimer = 0f;
		currentTarget = null;
	}

	public void ChangeText(Enemy target)
	{
		if (!target)
		{
			return;
		}
		switch (target.MainElement)
		{
		case 0:
			ELtext.text = LOC.MM.GetMain("ElementType") + " : <color=" + DamageColor.Colors[DamageType.fire] + ">" + LOC.MM.GetMain(SWS.El_Name(target.MainElement)) + "</color>";
			break;
		case 1:
			ELtext.text = LOC.MM.GetMain("ElementType") + " : <color=" + DamageColor.Colors[DamageType.frozen] + ">" + LOC.MM.GetMain(SWS.El_Name(target.MainElement)) + "</color>";
			break;
		case 2:
			ELtext.text = LOC.MM.GetMain("ElementType") + " : <color=" + DamageColor.Colors[DamageType.thunder] + ">" + LOC.MM.GetMain(SWS.El_Name(target.MainElement)) + "</color>";
			break;
		case 3:
			ELtext.text = LOC.MM.GetMain("ElementType") + " : <color=" + DamageColor.Colors[DamageType.poison] + ">" + LOC.MM.GetMain(SWS.El_Name(target.MainElement)) + "</color>";
			break;
		case 4:
			ELtext.text = LOC.MM.GetMain("ElementType") + " : <color=" + DamageColor.Colors[DamageType.physics] + ">" + LOC.MM.GetMain(SWS.El_Name(target.MainElement)) + "</color>";
			break;
		case 5:
			ELtext.text = LOC.MM.GetMain("ElementType") + " : <color=" + DamageColor.Colors[DamageType.shadow] + ">" + LOC.MM.GetMain(SWS.El_Name(target.MainElement)) + "</color>";
			break;
		}
		text.text = $"{LOC.MM.GetEnemy(target.IndexName)}   LV.{target.Level}";
		int num = Mathf.Min(SPCtext.Length, target.SSIndex.Length);
		for (int i = 0; i < num; i++)
		{
			switch (target.EnemyType)
			{
			case 0:
			case 1:
			case 2:
			case 3:
			case 4:
			case 5:
			case 6:
			case 7:
			case 8:
			case 9:
			case 10:
			case 11:
			case 12:
			case 13:
			case 14:
			case 15:
			case 16:
			case 17:
			case 18:
			case 19:
			case 20:
			case 21:
			case 22:
			case 23:
			case 24:
			case 25:
			case 26:
			case 27:
			case 28:
			case 29:
			case 30:
			case 31:
			case 32:
			case 50:
			case 80:
				SimpleEM(target, i);
				break;
			case 100:
				TowerEM(target, i);
				break;
			}
		}
	}

	public void SimpleEM(Enemy target, int i)
	{
		switch (target.SSIndex[i])
		{
		case 0:
			SPCtext[i].gameObject.SetActive(value: false);
			break;
		case 1:
			SPCtext[i].gameObject.SetActive(value: true);
			SPCtext[i].text = LOC.MM.GetMain("Crazy_SS");
			break;
		case 2:
			SPCtext[i].gameObject.SetActive(value: true);
			SPCtext[i].text = LOC.MM.GetMain("Force_SS");
			break;
		case 3:
			SPCtext[i].gameObject.SetActive(value: true);
			SPCtext[i].text = LOC.MM.GetMain("Curse_SS");
			break;
		case 4:
			SPCtext[i].gameObject.SetActive(value: true);
			SPCtext[i].text = LOC.MM.GetMain("Quick_SS");
			break;
		case 5:
			SPCtext[i].gameObject.SetActive(value: true);
			SPCtext[i].text = LOC.MM.GetMain("Magic_SS");
			break;
		case 6:
			SPCtext[i].gameObject.SetActive(value: true);
			SPCtext[i].text = LOC.MM.GetMain("Copy_SS");
			break;
		case 7:
			SPCtext[i].gameObject.SetActive(value: true);
			SPCtext[i].text = LOC.MM.GetMain("Strong_SS");
			break;
		case 8:
			SPCtext[i].gameObject.SetActive(value: true);
			SPCtext[i].text = LOC.MM.GetMain("Recover_SS");
			break;
		case 9:
			SPCtext[i].gameObject.SetActive(value: true);
			SPCtext[i].text = LOC.MM.GetMain("StoneSkin_SS");
			break;
		case 10:
			SPCtext[i].gameObject.SetActive(value: true);
			SPCtext[i].text = LOC.MM.GetMain("MagicAnti_SS");
			break;
		case 11:
			SPCtext[i].gameObject.SetActive(value: true);
			SPCtext[i].text = LOC.MM.GetMain("MY_SS");
			break;
		case 12:
			SPCtext[i].gameObject.SetActive(value: true);
			SPCtext[i].text = LOC.MM.GetMain("Die_SS");
			break;
		case 13:
			SPCtext[i].gameObject.SetActive(value: true);
			switch (target.MainELType)
			{
			case DamageType.fire:
				if (target.Quality > 2)
				{
					SPCtext[i].text = "<color=" + DamageColor.Colors[DamageType.fire] + ">" + LOC.MM.GetMain("Fire_SS") + "</color>";
				}
				else
				{
					SPCtext[i].text = "<color=" + DamageColor.Colors[DamageType.fire] + ">" + LOC.MM.GetMain("Fire_S") + "</color>";
				}
				break;
			case DamageType.frozen:
				if (target.Quality > 2)
				{
					SPCtext[i].text = "<color=" + DamageColor.Colors[DamageType.frozen] + ">" + LOC.MM.GetMain("Frozen_SS") + "</color>";
				}
				else
				{
					SPCtext[i].text = "<color=" + DamageColor.Colors[DamageType.frozen] + ">" + LOC.MM.GetMain("Frozen_S") + "</color>";
				}
				break;
			case DamageType.thunder:
				if (target.Quality > 2)
				{
					SPCtext[i].text = "<color=" + DamageColor.Colors[DamageType.thunder] + ">" + LOC.MM.GetMain("Thunder_SS") + "</color>";
				}
				else
				{
					SPCtext[i].text = "<color=" + DamageColor.Colors[DamageType.thunder] + ">" + LOC.MM.GetMain("Thunder_S") + "</color>";
				}
				break;
			case DamageType.poison:
				if (target.Quality > 2)
				{
					SPCtext[i].text = "<color=" + DamageColor.Colors[DamageType.poison] + ">" + LOC.MM.GetMain("Poison_SS") + "</color>";
				}
				else
				{
					SPCtext[i].text = "<color=" + DamageColor.Colors[DamageType.poison] + ">" + LOC.MM.GetMain("Poison_S") + "</color>";
				}
				break;
			case DamageType.physics:
				if (target.Quality > 2)
				{
					SPCtext[i].text = "<color=" + DamageColor.Colors[DamageType.physics] + ">" + LOC.MM.GetMain("Physics_SS") + "</color>";
				}
				else
				{
					SPCtext[i].text = "<color=" + DamageColor.Colors[DamageType.physics] + ">" + LOC.MM.GetMain("Physics_S") + "</color>";
				}
				break;
			case DamageType.shadow:
				if (target.Quality > 2)
				{
					SPCtext[i].text = "<color=" + DamageColor.Colors[DamageType.shadow] + ">" + LOC.MM.GetMain("Shadow_SS") + "</color>";
				}
				else
				{
					SPCtext[i].text = "<color=" + DamageColor.Colors[DamageType.shadow] + ">" + LOC.MM.GetMain("Shadow_S") + "</color>";
				}
				break;
			default:
				throw new ArgumentOutOfRangeException();
			}
			break;
		case 14:
			SPCtext[i].gameObject.SetActive(value: true);
			switch (target.LQtype)
			{
			case 0:
				SPCtext[i].text = LOC.MM.GetMain("Aura_Battle");
				break;
			case 1:
				SPCtext[i].text = LOC.MM.GetMain("Aura_Chuan");
				break;
			case 2:
				SPCtext[i].text = LOC.MM.GetMain("Aura_ShanBi");
				break;
			case 3:
				SPCtext[i].text = LOC.MM.GetMain("Aura_MinJie");
				break;
			case 4:
				SPCtext[i].text = LOC.MM.GetMain("Aura_FangYu");
				break;
			case 5:
				SPCtext[i].text = LOC.MM.GetMain("Aura_Recover");
				break;
			case 6:
				break;
			}
			break;
		case 15:
			SPCtext[i].gameObject.SetActive(value: true);
			SPCtext[i].text = LOC.MM.GetMain("Comp_SS");
			break;
		case 16:
			SPCtext[i].gameObject.SetActive(value: true);
			SPCtext[i].text = LOC.MM.GetMain("MultiAT_SS");
			break;
		}
	}

	public void TowerEM(Enemy target, int i)
	{
		switch (target.SSIndex[i])
		{
		case 0:
			SPCtext[i].gameObject.SetActive(value: false);
			break;
		case 1:
			SPCtext[i].gameObject.SetActive(value: true);
			SPCtext[i].text = LOC.MM.GetMain("JBKC_SS");
			break;
		case 2:
			SPCtext[i].gameObject.SetActive(value: true);
			SPCtext[i].text = LOC.MM.GetMain("ZHL_SS");
			break;
		case 3:
			SPCtext[i].gameObject.SetActive(value: true);
			SPCtext[i].text = LOC.MM.GetMain("JJSZ_SS");
			break;
		case 4:
			SPCtext[i].gameObject.SetActive(value: true);
			SPCtext[i].text = LOC.MM.GetMain("KSBJ_SS");
			break;
		}
	}
}
