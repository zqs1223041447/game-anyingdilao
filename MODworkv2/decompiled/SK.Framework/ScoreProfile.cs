using UnityEngine;

namespace SK.Framework;

[CreateAssetMenu]
public class ScoreProfile : ScriptableObject
{
	public ScoreInfo[] scores = new ScoreInfo[0];
}
