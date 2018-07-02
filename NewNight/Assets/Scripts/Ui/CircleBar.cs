using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ui
{



	public class CircleBar : StatBar
	{

		void OnMouseDown()
		{
			GetComponent<Animator>().SetTrigger("ROTATE");
		}
	}

}
