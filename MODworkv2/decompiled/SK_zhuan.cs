using UnityEngine;

public class SK_zhuan : MonoBehaviour
{
	public float angle;

	private void Start()
	{
	}

	private void OnEnable()
	{
	}

	private void Update()
	{
		base.transform.Rotate(new Vector3(0f, 0f, 1f), angle * Time.deltaTime);
	}
}
