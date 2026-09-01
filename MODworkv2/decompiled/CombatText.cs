using System.Collections;
using Lean.Pool;
using UnityEngine;
using UnityEngine.UI;

public class CombatText : MonoBehaviour
{
	[SerializeField]
	private float speed;

	[SerializeField]
	private float lifeTime;

	[SerializeField]
	private Text text;

	[SerializeField]
	private float time;

	public float Speed => speed;

	public float LifeTime
	{
		get
		{
			if (!(lifeTime > 0f))
			{
				return 0.5f;
			}
			return lifeTime;
		}
	}

	private void Start()
	{
	}

	private void Move()
	{
		base.transform.Translate(Vector2.up * speed * Time.deltaTime);
		time += Time.deltaTime;
		if ((double)time >= 0.5)
		{
			LeanPool.Despawn(base.gameObject);
			time = 0f;
		}
	}

	public IEnumerator FadeOut()
	{
		_ = text.color;
		float rate = 1f / lifeTime;
		float progress = 0f;
		while ((double)progress < 1.0)
		{
			Color color = text.color;
			text.color = color;
			progress += rate * Time.deltaTime;
			yield return null;
		}
		LeanPool.Despawn(base.gameObject);
	}
}
