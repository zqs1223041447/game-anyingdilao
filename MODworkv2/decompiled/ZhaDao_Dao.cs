using UnityEngine;

public class ZhaDao_Dao : MonoBehaviour
{
	public ZhaDao dao;

	private void Awake()
	{
		dao = base.transform.parent.GetComponent<ZhaDao>();
	}

	public void Zha()
	{
		dao.Zha();
	}
}
