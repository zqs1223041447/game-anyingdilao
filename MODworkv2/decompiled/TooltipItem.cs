using UnityEngine;
using UnityEngine.UI;

public class TooltipItem : MonoBehaviour
{
	public Text HealthText;

	public Text ManaText;

	public Stat healthStat;

	public Stat ManaStat;

	private float currentHealth;

	private float maxHealth;

	private float currentMana;

	private float maxMana;

	protected void Awake()
	{
		ShowAll();
	}

	private void Start()
	{
		ShowAll();
	}

	private void Update()
	{
		RefreshUI();
	}

	public void RefreshUI()
	{
		if ((bool)healthStat && HealthText.gameObject.activeSelf)
		{
			float num = Mathf.Floor(healthStat.Max);
			float value = Mathf.Floor(healthStat.Cur);
			value = Mathf.Clamp(value, 0f, num);
			HealthText.text = $"{value}/{num}";
		}
		if ((bool)ManaStat && ManaText.gameObject.activeSelf)
		{
			float num2 = Mathf.Floor(ManaStat.Max);
			float value2 = Mathf.Floor(ManaStat.Cur);
			value2 = Mathf.Clamp(value2, 0f, num2);
			ManaText.text = $"{value2}/{num2}";
		}
	}

	public void HideAll()
	{
		HealthText.gameObject.SetActive(value: false);
		ManaText.gameObject.SetActive(value: false);
	}

	public void ShowAll()
	{
		HealthText.gameObject.SetActive(value: true);
		ManaText.gameObject.SetActive(value: true);
		RefreshUI();
	}

	public void ShowHealth()
	{
		HealthText.gameObject.SetActive(value: true);
		RefreshUI();
	}

	public void ShowMana()
	{
		ManaText.gameObject.SetActive(value: true);
		RefreshUI();
	}
}
