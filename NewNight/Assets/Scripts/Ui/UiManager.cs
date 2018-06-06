using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using UnityEngine;

namespace Ui
{

	public class UiManager : Supportive.Singleton<UiManager>
	{

		[SerializeField] private Transform _generalParent;
		[SerializeField] private Transform _characters;
		[SerializeField] private Transform _portraits;
		[SerializeField] private Transform _dialogues;
		[SerializeField] private Transform _foregroundItems;
		[SerializeField] private Transform _props;
		[SerializeField] private Transform _backgroundParent;
		[SerializeField] private UiCamera _camera;

		private bool exitable = false;
		private Dictionary<string, Transform> _nameToParent;

		[SerializeField] private UiMask _backgroundMask;
		
		private Renderer[] _backgrounds;
		private MaterialPropertyBlock _block;
		[SerializeField] private Material[] _backgroundMats;

		
		//TEST
		public Texture _tmpTex;
		
		
		void Start()
		{
			_nameToParent = new Dictionary<string, Transform>()
			{
				{ "Characters",_characters},
				{"BackDecorations",_props},
				{"FrontDecorations",_foregroundItems},
				{"Dialogues",_dialogues},
				{"Backgrounds",_backgroundParent}
				
			};
			_backgrounds=new Renderer[4];
			_block=new MaterialPropertyBlock();
			
			_backgrounds[0]=Generate("Backgrounds",0,new Vector3(0,0,30),false).gameObject.GetComponent<Renderer>();


			_tmpTex = _backgrounds[0].material.mainTexture;
			

			//print(_block.GetFloat("_StencilRead"));
			_backgrounds[0].material = _backgroundMats[0];
			
			//_backgrounds[0].GetPropertyBlock(_block);
			_block.SetTexture("_MainTex",_tmpTex);
			//_block.SetFloat("_Lighting",1);
			_backgrounds[0].SetPropertyBlock(_block);

			_backgroundMask.Initialize();

			Story.instance.Initialize();
		}

		private void PlaceBackground(int index, int stencilLayer, Vector3 worldPosition, float lighting)
		{
			_backgrounds[stencilLayer]=Generate("Backgrounds", index, worldPosition, false).gameObject.GetComponent<Renderer>();
			_tmpTex = _backgrounds[stencilLayer].material.mainTexture;
			_backgrounds[stencilLayer].material = _backgroundMats[stencilLayer];
			_block.Clear();
			_block.SetTexture("_MainTex",_tmpTex);
			_block.SetFloat("_Lighting",lighting);
			_backgrounds[stencilLayer].SetPropertyBlock(_block);
		}

		public void SwitchBackground(int index)
		{
			PlaceBackground(index,1,new Vector3(0,0,15),-.3f);
			PlaceBackground(index,2,new Vector3(0,0,15), 0.3f);
			PlaceBackground(index,3,new Vector3(0,0,30), 0f);
			_backgroundMask.SwitchBackground();
			
		}

		public void GenerateBackground(int index)
		{
			
		}


		public UiItem Generate(string pathName, int index, Vector3 position, bool inScreenSpace)
		{
			string path="Assets/Resources/"+pathName;
			// these error detects can be replaced by neater class initialization
			if (!Directory.Exists(path))
			{
				Debug.LogError("UiManager try to generate item from unexisted directory: "+path);
				return null;
			}
			int numberOfFiles = Directory.GetFiles(path,"*.prefab").Length;
			if (numberOfFiles < 1)
			{
				Debug.LogError("UiManager try to generate an item from empty directory: "+path);
				return null;
			}

			if (index < 0)
			{
				index = Random.Range(0,numberOfFiles);
			}

			path = path + "/" + index.ToString()+".prefab";
			
			if (!File.Exists(path))
			{
				Debug.LogError("UiManager try to generate an unexisted item: "+path);
				return null;
			}
			
			

			Vector3 positionInWorld = position;
			if (inScreenSpace)
			{
				positionInWorld = Coordinate.instance.Screen2Space(positionInWorld);
			}

			GameObject newItem = Instantiate(Resources.Load<GameObject>(pathName+"/"+index.ToString()));
			newItem.transform.position = positionInWorld;
			
			if (!_nameToParent.ContainsKey(pathName))
			{
				Debug.LogWarning("UiManager generates an item that doesn't have registered  category/parent: "+pathName);
				newItem.transform.parent = _generalParent;
			}
			else
			{
				newItem.transform.parent = _nameToParent[pathName];
			}

			newItem.GetComponent<UiItem>().Initialize();
			return newItem.GetComponent<UiItem>();

		}

		public UiItem GenerateForegroundItem(int index, Vector3 position, Vector3 potentialPosition, bool inScreenSpace)
		{
			string path = "Assets/Resources/ForegroundItems/" + index.ToString()+".prefab";
			if (!File.Exists(path))
			{
				Debug.LogError("UiManager try to generate an unexisted item: "+path);
				return null;
			}
			
			Vector3 positionInWorld = position;
			if (inScreenSpace)
			{
				positionInWorld = Coordinate.instance.Screen2Space(position);
			}
			
			GameObject itemObject = Instantiate(Resources.Load<GameObject>("ForegroundItems/"+index.ToString()),_foregroundItems);
			itemObject.transform.position = positionInWorld;
			ForegroundItem item = itemObject.GetComponent<ForegroundItem>();
			if (item == null)
			{
				Debug.LogError("Resources/ForegroundItems/"+index.ToString()+" does not have component ForgroundItem");
				return null;
			}
			item.Initialize(inScreenSpace?potentialPosition:Coordinate.instance.Space2Screen(potentialPosition));
			return item;
		}

		

		public void ZoomIn(Character focus)
		{
			exitable = true;
			_camera.Transfer(new Vector3(0,0,3),false,false);
			MoveOutGroup(_characters,focus);
			MoveOutGroup(_foregroundItems,focus);
			focus.transform.parent = _portraits;
			
		}

		public void ZoomOut()
		{
			if (exitable)
			{
				exitable = false;
				_camera.MoveBack();
				MoveBackGroup(_portraits,_characters);
				MoveBackGroup(_foregroundItems);
			}

		}

		private void MoveOutGroup(Transform parent, UiItem focus)
		{
			int numChildren = parent.GetChildCount();
			for (int i = 0; i < numChildren; i++)
			{
				UiItem item = parent.GetChild(i).gameObject.GetComponent<UiItem>();
				if (item != null)
				{
					item.MoveOut(focus);
				}
				else
				{
					Debug.LogError("UiManager try to MoveOutkGroup a parent that has non-UiItem child");
				}
			}
		}

		private void MoveBackGroup(Transform parent, Transform switchToParent = null)
		{
			int numChildren = parent.GetChildCount();
			for (int i = 0; i < numChildren; i++)
			{
				UiItem item = parent.GetChild(i).gameObject.GetComponent<UiItem>();
				if (item != null)
				{
					item.MoveBack();
					if (switchToParent != null) item.transform.parent = switchToParent;
				}
				else
				{
					Debug.LogError("UiManager try to MoveBackGroup a parent that has non-UiItem child");
				}
			}
		} 
		
		
	}
}
