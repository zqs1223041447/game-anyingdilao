using UnityEngine;

public class GroundTile : MonoBehaviour
{
	private SpriteRenderer render;

	public Sprite[] sp;

	private void Start()
	{
		render = GetComponent<SpriteRenderer>();
		render.sprite = sp[Random.Range(0, 10)];
	}
}
