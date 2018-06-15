using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Mime;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

namespace Ui
{
	public class Option : MonoBehaviour
	{
		[Serializable]public struct Cost
		{
			public string Name;
			public int Value; // TMP: this will be changed into int later
		};
		
		[Serializable]public struct OptionCon
		{
			public int IndexToInOption;
			public string Containing;
			public List<Cost> Costs;
			//public Cost[] Costs;
		};
		
		private TextMeshPro _textMesh;
		public Dialogue _dia;
		[SerializeField]private int _index;
		private Color _oriColor;
		private BoxCollider _col;
		[SerializeField] private TextMeshPro[] _tags;
		private List<Cost> _costs=new List<Cost>();




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
			_costs.Clear();
			for (int i = 0; i < 4; i++) // TMP: replace with a variable later
			{
				_tags[i].text = "";
			}
		}
		
		public void SetOption(OptionCon con)
		{
			_textMesh.text = con.Containing;
			_costs.Clear();
			for (int i = 0; i < 4; i++) // TMP: replace with a variable later
			{
				if (i < con.Costs.Count)
				{
					_costs.Add(con.Costs[i]);
					_tags[i].text=con.Costs[i].Name+con.Costs[i].Value.ToString();
				}
				else
				{
					_tags[i].text = "";
				}
			}
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
			for (int i = 0; i < _costs.Count; i++)
			{
				UiManager.instance.ModifyStat(_costs[i].Name,_costs[i].Value);
			}
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
