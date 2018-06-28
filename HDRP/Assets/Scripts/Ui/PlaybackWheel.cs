using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ui
{
    public class PlaybackWheel : UiItem
    {

        [SerializeField] private Rotate _rotatingPart;

        void Start()
        {
            Initialize();
        }

        
        public override void Initialize(Vector3 aimPosition=new Vector3())
        {
            SetPosition(new Vector3(-1.1f,-1,4),true,false,true);
            UpdateOriginPosition();
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

