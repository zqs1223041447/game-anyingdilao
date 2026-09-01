using UnityEngine;

public class SK_Triger : MonoBehaviour
{
	public SK_Thunder_LZ td;

	private void Awake()
	{
		td = GetComponentInParent<SK_Thunder_LZ>();
	}

	private void Start()
	{
	}

	private void OnEnable()
	{
	}

	private void Update()
	{
	}

	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (collision.CompareTag("blockFLY"))
		{
			td.dic.dic = Vector2.zero;
		}
	}
}
