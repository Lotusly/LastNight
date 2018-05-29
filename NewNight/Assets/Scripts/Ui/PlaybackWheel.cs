using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ui
{
    public class PlaybackWheel : UiItem
    {

        [SerializeField] private Rotate _rotatingPart;

        void Start () {
		    SetPosition(new Vector3(-1.1f,-1,4),true,false);
            EnableFollowCamera();
        }
	
        private void OnMouseEnter()
        {
            _rotatingPart.EnableRotating();
        }

        private void OnMouseExit()
        {
            _rotatingPart.DisableRotating();
        }
    }

}

