using System.Collections;
using System.Collections.Generic;
using System.Security.Principal;
using UnityEngine;

namespace Ui
{
	/// <summary>
	/// BatchNode is a class used to batch-manage UiItem objects.
	/// It records a bunch of UiItem objects in its variable _leaves.
	/// Its transform.position is expected to be at the center of all the objects in _leaves. 
	/// </summary>
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
		

		// make sure that transform.position is the center of _leaves
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
		
		
		// directly change transform.position without influencing its children
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

		// add a leaf to _leaves. this function keep the transform.position correctly at the center.
		public void AddLeaf(Transform newLeaf)
		{
			if (newLeaf == null) return;
			_leaves.Add(newLeaf);
			Count++;
			SwitchPosition((transform.position*(Count-1)+newLeaf.position)/Count);
		}

		// delete a leaf from _leaves. this function keep the transform.position correctly at the center.
		public void DeleteLeaf(Transform oldLeaf)
		{
			if (!_leaves.Contains(oldLeaf)) return;
			_leaves.Remove(oldLeaf);
			Count--;
			SwitchPosition((transform.position*(Count+1)-oldLeaf.position)/Count);
		}

		// put all of its leaves under its transform. call this function before moving it, so all the leaves will be moved with it.
		public void AssemblyLeaves()
		{
			foreach (var leaf in _leaves)
			{
				leaf.transform.parent = transform;
			}
		}
	}
}
