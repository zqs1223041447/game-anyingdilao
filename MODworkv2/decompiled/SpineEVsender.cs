using UnityEngine;

public class SpineEVsender : MonoBehaviour
{
	public DICI_col par;

	private void Start()
	{
		par = base.transform.parent.gameObject.GetComponent<DICI_col>();
		new AnimationEvent().stringParameter = "attack";
	}

	public void attack()
	{
		par.Damage();
	}
}
