using System.Collections.Generic;
using FinkFramework.Runtime.ResLoad;
using FinkFramework.Runtime.Singleton;
using Lean.Pool;
using UnityEngine;
using UnityEngine.UI;

public class DamgeTextManager : SingletonMonoScope<DamgeTextManager>
{
	private sealed class ActiveCombatText
	{
		public Text Text;

		public Transform Transform;

		public Vector2 SourcePosition;

		public DamageType Type;

		public float Damage;

		public float Speed;

		public float LifeTime;

		public float Elapsed;

		public float LastMergeTime;
	}

	private static readonly float[] DamageNumberDivisors = new float[8] { 1E+24f, 1E+21f, 1E+18f, 1E+15f, 1E+12f, 1E+09f, 1000000f, 1000f };

	private static readonly string[] DamageNumberUnits = new string[8] { "Y", "Z", "E", "P", "T", "G", "M", "K" };

	private GameObject combatTextPrefab;

	private GameObject templeTextPrefab;

	private const int MaxActiveCombatTexts = 80;

	private const float MergeWindow = 0.08f;

	private const float MergeDistance = 0.25f;

	private readonly List<ActiveCombatText> activeCombatTexts = new List<ActiveCombatText>(80);

	private bool _sctToggle;

	private float _sctScale;

	protected override void OnSingletonAwake()
	{
		SingletonMonoGlobal<SessionManager>.Instance.Attach(this, ProcessScope.Game);
		SettingDataManager instance = Singleton<SettingDataManager>.Instance;
		instance.Init();
		SetSCTScale(instance.Interface.damage_scale);
		SetSCTToggle(instance.Interface.damage_text);
	}

	public void SetSCTToggle(bool toggle)
	{
		_sctToggle = toggle;
	}

	public void SetSCTScale(float scale)
	{
		_sctScale = Mathf.Clamp(scale, 1f, 3f);
	}

	private void EnsurePrefabsLoaded()
	{
		if (!combatTextPrefab)
		{
			combatTextPrefab = Singleton<ResManager>.Instance.Load<GameObject>("res://UI/Components/SCT/SCTText");
		}
		if (!templeTextPrefab)
		{
			templeTextPrefab = Singleton<ResManager>.Instance.Load<GameObject>("res://UI/Components/SCT/TempleText");
		}
	}

	private void Update()
	{
		float deltaTime = Time.deltaTime;
		for (int num = activeCombatTexts.Count - 1; num >= 0; num--)
		{
			ActiveCombatText activeCombatText = activeCombatTexts[num];
			if (activeCombatText == null || !activeCombatText.Transform || !activeCombatText.Transform.gameObject.activeInHierarchy)
			{
				activeCombatTexts.RemoveAt(num);
			}
			else
			{
				activeCombatText.Elapsed += deltaTime;
				if (activeCombatText.Elapsed >= activeCombatText.LifeTime)
				{
					LeanPool.Despawn(activeCombatText.Transform.gameObject);
					activeCombatTexts.RemoveAt(num);
				}
				else
				{
					activeCombatText.Transform.Translate(Vector2.up * activeCombatText.Speed * deltaTime);
				}
			}
		}
	}

	public void CreatCombatText(Vector2 position, float number, DamageType type, bool crit)
	{
		if (_sctToggle)
		{
			EnsurePrefabsLoaded();
			position.y += 0.8f;
			if (!TryMergeCombatText(position, number, type) && activeCombatTexts.Count < 80)
			{
				GameObject obj = LeanPool.Spawn(combatTextPrefab, base.transform);
				Text component = obj.GetComponent<Text>();
				CombatText component2 = obj.GetComponent<CombatText>();
				Transform transform = component.transform;
				transform.position = position + new Vector2(Random.Range(-0.1f, 0.1f), 0f);
				float t = Mathf.InverseLerp(1f, 3f, _sctScale);
				component.fontSize = Mathf.RoundToInt(Mathf.Lerp(14f, 43f, t));
				component.color = GetDamageColor(type);
				component.text = FormatDamageText(number);
				activeCombatTexts.Add(new ActiveCombatText
				{
					Text = component,
					Transform = transform,
					SourcePosition = position,
					Type = type,
					Damage = number,
					Speed = (component2 ? component2.Speed : 0.3f),
					LifeTime = (component2 ? component2.LifeTime : 0.5f),
					LastMergeTime = Time.time
				});
			}
		}
	}

	private bool TryMergeCombatText(Vector2 position, float number, DamageType type)
	{
		float time = Time.time;
		ActiveCombatText activeCombatText = null;
		float num = 0.25f;
		for (int num2 = activeCombatTexts.Count - 1; num2 >= 0; num2--)
		{
			ActiveCombatText activeCombatText2 = activeCombatTexts[num2];
			if (activeCombatText2 != null && activeCombatText2.Type == type && (bool)activeCombatText2.Transform && activeCombatText2.Transform.gameObject.activeInHierarchy && !(time - activeCombatText2.LastMergeTime > 0.08f))
			{
				float num3 = Vector2.Distance(position, activeCombatText2.SourcePosition);
				if (num3 <= num)
				{
					num = num3;
					activeCombatText = activeCombatText2;
				}
			}
		}
		if (activeCombatText == null)
		{
			return false;
		}
		activeCombatText.Damage += number;
		activeCombatText.SourcePosition = (activeCombatText.SourcePosition + position) * 0.5f;
		activeCombatText.LastMergeTime = time;
		activeCombatText.Text.text = FormatDamageText(activeCombatText.Damage);
		return true;
	}

	private static Color GetDamageColor(DamageType type)
	{
		return type switch
		{
			DamageType.fire => Color.red, 
			DamageType.frozen => new Color32(80, 230, byte.MaxValue, byte.MaxValue), 
			DamageType.thunder => Color.yellow, 
			DamageType.poison => Color.green, 
			DamageType.physics => Color.white, 
			DamageType.shadow => new Color32(243, 148, byte.MaxValue, byte.MaxValue), 
			_ => Color.white, 
		};
	}

	public static string FormatDamageNumber(float number)
	{
		if (number <= 0f)
		{
			return "0";
		}
		if (number <= 1000f)
		{
			return $"{Mathf.Floor(number)}";
		}
		for (int i = 0; i < DamageNumberDivisors.Length; i++)
		{
			float num = DamageNumberDivisors[i];
			if (!(number <= num))
			{
				float num2 = number / num;
				string arg = DamageNumberUnits[i];
				if (!(num2 <= 100f))
				{
					return $"{Mathf.Floor(num2)} {arg}";
				}
				return $"{num2:N1} {arg}";
			}
		}
		return $"{Mathf.Floor(number / 1E+24f)} Y";
	}

	private static string FormatDamageText(float number)
	{
		return FormatDamageNumber(number);
	}
}
