using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ui
{
	public class Background : UiItem
	{
		private Material _mat;
		public override void Initialize(Vector3 aimPosition)
		{
			_mat = GetComponent<Renderer>().material;
		}

		
	}
}
