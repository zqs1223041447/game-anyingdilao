using System.Collections;
using Lean.Pool;
using UnityEngine;
using UnityEngine.UI;

public class TempleText : MonoBehaviour
{
	[SerializeField]
	private float speed;

	[SerializeField]
	private float lifeTime;

	[SerializeField]
	private Text text;

	[SerializeField]
	private float time;

	private void Start()
	{
	}

	private void Update()
	{
		base.transform.Translate(Vector2.up * speed * Time.deltaTime);
		time += Time.deltaTime;
		if (time >= 2f)
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
		LeanPool.Despawn(this);
	}
}
