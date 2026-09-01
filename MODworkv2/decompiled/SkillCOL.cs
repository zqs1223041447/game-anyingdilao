using Lean.Pool;
using UnityEngine;

public class SkillCOL : MonoBehaviour
{
	public CircleCollider2D col;

	public float size;

	private float timeA;

	private float timeB;

	public float lifeTime;

	public SK_Road father;

	private void Start()
	{
		col = GetComponent<CircleCollider2D>();
	}

	private void OnEnable()
	{
		col.radius = size;
		timeA = 0f;
		timeB = 0f;
	}

	private void Update()
	{
		timeA += Time.deltaTime;
		if (timeA >= lifeTime)
		{
			timeA = 0f;
			father.colList.Remove(this);
			LeanPool.Despawn(this);
		}
	}

	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (collision.CompareTag("FootCOL"))
		{
			FootCOL component = collision.GetComponent<FootCOL>();
			if (component.peo.CharacterType == 2 && component.peo.em.IsAlive && !component.peo.em.IsJump && !component.peo.em.IsYS)
			{
				father.Add(component);
			}
		}
	}

	private void OnTriggerStay2D(Collider2D collision)
	{
		timeB += Time.deltaTime;
		if (!(timeB >= 0.1f))
		{
			return;
		}
		timeB = 0f;
		if (collision.CompareTag("FootCOL"))
		{
			FootCOL component = collision.GetComponent<FootCOL>();
			if (component.peo.CharacterType == 2 && component.peo.em.IsAlive && !component.peo.em.IsJump && !component.peo.em.IsYS)
			{
				father.Add(component);
			}
		}
	}

	private void OnTriggerExit2D(Collider2D collision)
	{
		if (collision.CompareTag("FootCOL"))
		{
			FootCOL component = collision.GetComponent<FootCOL>();
			if (component.peo.CharacterType == 2)
			{
				father.Del(component);
			}
		}
	}
}
