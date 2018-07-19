using System.Collections;
using System.Collections.Generic;
using System.Security.Principal;
using UnityEngine;

namespace Ui
{
	public class BatchNode : UiItem
	{
		public int Count=0;
		private List<Transform> _leaves = new List<Transform>();
		private Transform[] _children;
		// possible extension: any batch movement system
		
		// _children are under this transform. _leaves are under this node (may not under the transform)
		// for the transition system, BatchNode class is used to conviniently move categorites (midgrounds, backgrounds...).
		//  While BatchNode class can be easily extended to manage and move any group of items
		// Just remember to manager _leaves properly. It doesn't really matter what is under this transform
		


		public void UpdatePosition()
		{
			Vector3 addition = Vector3.zero;
			for (int i = 0; i < Count; i++)
			{
				addition += _leaves[i].position;
			}

			if (Count <= 0)
			{
				Count = 0;
				SwitchPosition(addition);
			}
			else
			{
				SwitchPosition(addition/Count);
			}
		}
		

		private void SwitchPosition(Vector3 newPosition)
		{
			_children = new Transform[transform.childCount]; 
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

		public void AddLeaf(Transform newLeaf)
		{
			if (newLeaf == null) return;
			_leaves.Add(newLeaf);
			Count++;
			SwitchPosition((transform.position*(Count-1)+newLeaf.position)/Count);
		}

		public void DeleteLeaf(Transform oldLeaf)
		{
			if (!_leaves.Contains(oldLeaf)) return;
			_leaves.Remove(oldLeaf);
			Count--;
			SwitchPosition((transform.position*(Count+1)-oldLeaf.position)/Count);
		}
	}
}
