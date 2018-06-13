using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;

namespace Ui
{
	public class Option : MonoBehaviour
	{
		private TextMeshPro _textMesh;
		public Dialogue _dia;
		[SerializeField]private int _index;
		private Color _oriColor;
		private BoxCollider _col;




		void Awake()
		{
			_textMesh = GetComponent<TextMeshPro>();
			_dia = GetComponentInParent<Dialogue>();
			_oriColor = _textMesh.color;
			_col = GetComponent<BoxCollider>();
			
		}


		
		public void ClearOption()
		{
			_textMesh.text = "";
			_col.enabled = false;
		}
		
		public void SetOption(string text)
		{
			_textMesh.text = text;
			StartCoroutine(DelayUpdateCollider());

		}

		void OnMouseEnter()
		{
			_textMesh.color = new Color(1f,0.5f,0);
			transform.localScale *= 1.2f;
		}

		void OnMouseExit()
		{
			_textMesh.color = _oriColor;
			transform.localScale /= 1.2f;
		}



		void OnMouseDown()
		{
			Story.instance.OnClick(_index);
		}



		private IEnumerator DelayUpdateCollider()
		{
			yield return null;
			yield return new WaitForSecondsRealtime(0.1f);
			_col.enabled = true;
			_col.center = _textMesh.bounds.center;
			_col.size = _textMesh.bounds.size;

		}
	}
	
	
}
