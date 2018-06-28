using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ui
{
	public class Background : UiItem
	{
		private Material _mat;


		private void OnMouseDown()
		{
			Story.instance.OnClick(0);
		}
	}
}
