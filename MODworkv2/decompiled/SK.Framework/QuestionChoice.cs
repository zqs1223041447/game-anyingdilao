using System;
using UnityEngine;

namespace SK.Framework;

[Serializable]
public class QuestionChoice
{
	public string text;

	public Sprite pic;

	public QuestionChoice(string text, Sprite pic)
	{
		this.text = text;
		this.pic = pic;
	}
}
