using Data.AutoGen.DataClass.Level;
using FinkFramework.Runtime.Singleton;
using Lean.Pool;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

public class Tower : MonoBehaviour
{
	public float Range;

	public GameObject EM;

	public GameObject[] Eye;

	public GameObject[] DieFX;

	public ParticleSystem[] parLoop;

	public GameObject[] OBJ;

	public Color[] color;

	public string IndexName;

	private int MainEL;

	private Transform point;

	private Light2D Lit;

	private LevelManager LV;

	private Enemy em;

	private bool CanAT;

	private void Awake()
	{
		point = base.transform.Find("main/point");
		Lit = base.transform.Find("main/light").GetComponent<Light2D>();
		LV = SingletonMonoScope<LevelManager>.Instance;
	}

	private void OnEnable()
	{
		CanAT = false;
		Lit.enabled = true;
		GameObject[] eye = Eye;
		for (int i = 0; i < eye.Length; i++)
		{
			eye[i].SetActive(value: false);
		}
		if (parLoop.Length != 0)
		{
			ParticleSystem[] array = parLoop;
			for (int j = 0; j < array.Length; j++)
			{
				ParticleSystem.MainModule main = array[j].main;
				main.loop = true;
			}
		}
		if (OBJ.Length != 0)
		{
			eye = OBJ;
			for (int k = 0; k < eye.Length; k++)
			{
				eye[k].SetActive(value: true);
			}
		}
		this.wait(1E-05f, SetStart);
	}

	private void Update()
	{
		if ((bool)em && em.EMstartOK && CanAT && !em.IsAlive)
		{
			SetDie();
		}
	}

	public void SetStart()
	{
		LevelData levelData = LevelManager.GetLevelData(LevelManager.GetCurLevel());
		em = LeanPool.Spawn(EM, new Vector3(base.transform.position.x, base.transform.position.y - 0.02f, base.transform.position.z), Quaternion.identity, base.transform).GetComponent<Enemy>();
		em.Quality = 1;
		em.IndexName = IndexName;
		em.Level = LevelManager.GetCurrentEnemyLevel();
		em.Xp = LV.GetXP(em.Quality, 2f, em.Level);
		em.size = 1;
		em.CompOffset = 1f;
		em.TuiSpeed = 1f;
		em.ItemDropPos = 1f;
		em.MainElement = Random.Range(0, 6);
		switch (em.MainElement)
		{
		case 0:
			em.MainELType = DamageType.fire;
			break;
		case 1:
			em.MainELType = DamageType.frozen;
			break;
		case 2:
			em.MainELType = DamageType.thunder;
			break;
		case 3:
			em.MainELType = DamageType.poison;
			break;
		case 4:
			em.MainELType = DamageType.physics;
			break;
		case 5:
			em.MainELType = DamageType.shadow;
			break;
		}
		MainEL = em.MainElement;
		Eye[MainEL].SetActive(value: true);
		if (OBJ.Length != 0)
		{
			GameObject[] oBJ = OBJ;
			for (int i = 0; i < oBJ.Length; i++)
			{
				oBJ[i].GetComponent<SpriteRenderer>().color = color[MainEL];
			}
		}
		if (parLoop.Length != 0)
		{
			ParticleSystem[] array = parLoop;
			for (int j = 0; j < array.Length; j++)
			{
				ParticleSystem.MainModule main = array[j].main;
				main.loop = true;
			}
		}
		em.Health_Base = Mathf.Floor(levelData.Tower_Health * Mathf.Pow(LV.HealthMulti, em.Level) * LevelManager.GetEnemyHealthCurveMultiplier(em.Level) * LevelManager.GetHeal());
		em.Health_Bei = 0f;
		em.GeDang = 0f;
		em.Damage_Base = Mathf.Floor(levelData.Tower_Damage * Mathf.Pow(LV.DamageMulti, em.Level) * LevelManager.GetDMG());
		em.Damage_Bei = 0f;
		em.FireAnti = 100f;
		em.FrozenAnti = 100f;
		em.ThunderAnti = 100f;
		em.PoisonAnti = 100f;
		em.PhysicsAnti = 100f;
		em.ShadowAnti = 100f;
		em.Chuan = 50f;
		em.Health_Prc = 0f;
		em.DamageAnti = 0f;
		em.FlySpeed = 0f;
		em.DotDamage = 0f;
		em.DotTime = 0f;
		em.AntiSlow = 0f;
		em.DotTimeCut = 0f;
		em.Range_Base = Range;
		em.Range_Anger = 2f;
		em.Range_Far = 10f;
		em.Range_ATplayer_multi = 1.05f;
		em.Can_DieBoom = false;
		em.Die_Index = 0;
		em.DieType = 0;
		em.DiePos = 0;
		em.DieFX_TimeDelay = 0.1f;
		em.DieDelay = 3f;
		em.IS_Boss = false;
		em.IS_Comp = false;
		em.IS_FS = false;
		CanAT = true;
		em.EnemyType = 100;
		for (int k = 0; k < em.SSIndex.Length; k++)
		{
			em.SSIndex[k] = 0;
		}
		if (Random.Range(0, 101) < 20)
		{
			em.Health_Bei += 50f;
			em.Xp *= 2;
			em.SSIndex[0] = Random.Range(1, 5);
			switch (em.SSIndex[0])
			{
			case 1:
				em.Health_Bei += 100f;
				break;
			case 2:
				em.Damage_Bei += 100f;
				break;
			case 3:
				em.AttackSpeed_Bei += 50f;
				em.Range_Base *= 1.5f;
				break;
			case 4:
				em.Health_Prc += 2f;
				break;
			case 0:
				break;
			}
		}
	}

	public void SetDie()
	{
		CanAT = false;
		Lit.enabled = false;
		if (parLoop.Length != 0)
		{
			ParticleSystem[] array = parLoop;
			for (int i = 0; i < array.Length; i++)
			{
				ParticleSystem.MainModule main = array[i].main;
				main.loop = false;
			}
		}
		if (OBJ.Length != 0)
		{
			GameObject[] oBJ = OBJ;
			for (int j = 0; j < oBJ.Length; j++)
			{
				oBJ[j].SetActive(value: false);
			}
		}
		LeanPool.Spawn(DieFX[MainEL], point.transform.position, Quaternion.identity, point.transform);
	}
}
