using UnityEngine;

public class FootCOL : MonoBehaviour
{
	[HideInInspector]
	public People peo;

	private void Awake()
	{
		peo = base.transform.parent.transform.Find("People").GetComponent<People>();
	}

	private void OnEnable()
	{
		peo = base.transform.parent.transform.Find("People").GetComponent<People>();
	}
}
