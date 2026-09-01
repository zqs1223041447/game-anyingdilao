using System;
using System.Collections.Generic;

namespace Dialog;

[Serializable]
public class DialogData
{
	public string DialogId;

	public List<DialogLineData> Lines = new List<DialogLineData>();
}
