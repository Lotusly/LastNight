using System.Collections;
using System.Collections.Generic;
using System.Security.Principal;
using UnityEngine;

namespace Ui
{
	public class BatchNode : UiItem
	{
		public int Count=0;
		private Transform[] _children;

		public void SwitchPosition(Vector3 newPosition)
		{
			_children = new Transform[Count]; // Count must be up to date
			for (int i = 0; i < Count; i++)
			{
				_children[i] = transform.GetChild(i);
			}
			transform.DetachChildren();
			transform.position = newPosition;
			for (int i = 0; i < Count; i++)
			{
				_children[i].parent = transform;
			}

		}
	}
}
