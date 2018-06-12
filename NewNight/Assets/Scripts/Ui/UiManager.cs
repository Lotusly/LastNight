
using System.Collections.Generic;
using System.IO;
using Supportive;
using UnityEngine;


namespace Ui
{

	public class UiManager : Supportive.Singleton<UiManager>
	{
		struct Item
		{
			public string Name;
			public int Index;
			public Vector3 PositionInScreen;
		};
		
	

		[SerializeField] private Transform _generalParent;
		[SerializeField] private Transform _characters;
		[SerializeField] private Transform _portraits;
		[SerializeField] private Transform _dialogues;
		[SerializeField] private Transform _foregroundItems;
		[SerializeField] private Transform _props;
		[SerializeField] private Transform _backgroundParent;
		public UiCamera _camera;

		private bool exitable = false;
		private bool switchingBackground = false;
		private Dictionary<string, Transform> _nameToParent;

		[SerializeField] private UiMask _backgroundMask;
		
		private Renderer[] _backgrounds;
		private MaterialPropertyBlock _block;
		[SerializeField] private Material[] _backgroundMats;

		
		private Texture _tmpTex;

		private List<Item> _potentialItems;

		
		
		
		void Start()
		{
			_nameToParent = new Dictionary<string, Transform>()
			{
				{ "Characters",_characters},
				{"Props",_props},
				{"FrontDecorations",_foregroundItems},
				{"Dialogues",_dialogues},
				{"Backgrounds",_backgroundParent}
				
			};

			_potentialItems = new List<Item>();
			_backgrounds=new Renderer[4];
			_block=new MaterialPropertyBlock();
			UiItem tmp = Generate("Backgrounds", 0, new Vector3(0, 0, 30), false);
			_backgrounds[0]=tmp.GetComponent<Renderer>();

			_tmpTex = _backgrounds[0].material.mainTexture;
			
			_backgrounds[0].material = _backgroundMats[0];
			_block.SetTexture("_MainTex",_tmpTex);
			_backgrounds[0].SetPropertyBlock(_block);

			_backgroundMask.Initialize();

			Story.instance.Initialize();
		}

		private void PlaceBackground(int index, int stencilLayer, Vector3 worldPosition,  float lighting)
		{
			_backgrounds[stencilLayer]=Generate("Backgrounds", index, worldPosition, false).gameObject.GetComponent<Renderer>();
			_tmpTex = _backgrounds[stencilLayer].material.mainTexture;
			_backgrounds[stencilLayer].material = _backgroundMats[stencilLayer];
			_block.Clear();
			_block.SetTexture("_MainTex",_tmpTex);
			_block.SetFloat("_Lighting",lighting);
			_backgrounds[stencilLayer].SetPropertyBlock(_block);
		}

		public void SetSwitchMask(Vector2 interval)
		{
			_backgroundMask.SetInterval(interval);
		}

		public void SwitchBackground(int index, Vector2 direction=new Vector2())
		{
			
			if (!switchingBackground)
			{
				//TEST
				Test.instance.index = (Test.instance.index + 1) % 2;
				
				float randomSeed = Random.value-0.5f;
				print(randomSeed);
				switchingBackground = true;
				
				
				
				PlaceBackground(index, 1, new Vector3(0, 0, 20-randomSeed*5),  -.4f);
				_backgrounds[1].gameObject.GetComponent<UiItem>().Transfer(_backgrounds[1].transform.position+new Vector3(0,0,1)*5*randomSeed,false,false);
				PlaceBackground(index, 2, new Vector3(0, 0, 20-randomSeed*5),  0.4f);
				_backgrounds[2].gameObject.GetComponent<UiItem>().Transfer(_backgrounds[2].transform.position+new Vector3(0,0,1)*5*randomSeed,false,false);
				PlaceBackground(index, 3, new Vector3(0, 0, 30-randomSeed*7.5f), 0f);
				_backgrounds[3].gameObject.GetComponent<UiItem>().Transfer(_backgrounds[3].transform.position+new Vector3(0,0,1)*7.5f*randomSeed,false,false);

				
				
				if (direction.x > direction.y)
				{
					if (direction.x > -direction.y)
					{
						PropsSwitch(Vector3.right*2);
						_backgrounds[0].gameObject.GetComponent<UiItem>().Transfer(
							Coordinate.instance.Space2Screen(_backgrounds[0].transform.position) - Vector3.right*2, true, false, false, 2,
							0.7f);
					}
					else
					{
						PropsSwitch(Vector3.down*2);
						_backgrounds[0].gameObject.GetComponent<UiItem>().Transfer(
							Coordinate.instance.Space2Screen(_backgrounds[0].transform.position) - Vector3.down*2, true, false, false, 2,
							0.7f);
					}
				}
				else
				{
					if (direction.x > -direction.y)
					{
						PropsSwitch(Vector3.up*2);
						_backgrounds[0].gameObject.GetComponent<UiItem>().Transfer(
							Coordinate.instance.Space2Screen(_backgrounds[0].transform.position) - Vector3.up*2, true, false, false, 2, 0.7f);
					}
					else
					{
						PropsSwitch(Vector3.left*2);
						_backgrounds[0].gameObject.GetComponent<UiItem>().Transfer(
							Coordinate.instance.Space2Screen(_backgrounds[0].transform.position) - Vector3.left*2, true, false, false, 2,
							0.7f);
					}
				}
				_backgroundMask.SwitchBackground(direction);
				
			}

		}

		public void ClearPotentialProp()
		{
			_potentialItems.Clear();
		}

		public void PutPotentialProp(int index, Vector3 positionInScreen) // position muast in screen space. By default, put out of the right edge
		{
			Item newItem = new Item();
			newItem.Name = "Props";
			newItem.Index = index;
			newItem.PositionInScreen = positionInScreen;
			_potentialItems.Add(newItem);
			
			
		}

		private void GeneratePotentialProps(Vector3 direction)
		{
			for (int i = 0; i < _potentialItems.Count; i++)
			{
				UiItem prop = Generate("Props", _potentialItems[i].Index, _potentialItems[i].PositionInScreen + direction, true);
			}
		}

		private void PropsSwitch(Vector3 direction)
		{
			UiItem[] props = _props.GetComponentsInChildren<UiItem>();
			for (int i = 0; i < props.Length; i++)
			{
				props[i].AfterArrival.AddListener(props[i].DestroyHandler);
			}
			GeneratePotentialProps(direction);
			props=_props.GetComponentsInChildren<UiItem>();
			for (int i = 0; i < props.Length; i++)
			{
				props[i].Transfer(Coordinate.instance.Space2Screen(props[i].transform.position)-direction,true,false,false,0,2f);
			}
		}

		public void AfterBackground()
		{
			_backgrounds[0].transform.SetPositionAndRotation(_backgrounds[3].transform.position,_backgrounds[3].transform.rotation);
			_backgrounds[0].transform.localScale = _backgrounds[3].transform.localScale;
			_backgrounds[0].gameObject.GetComponent<UiItem>().EnableFollowObject(_backgrounds[3].gameObject.GetComponent<UiItem>());
			_backgrounds[3].GetPropertyBlock(_block);
			_backgrounds[0].SetPropertyBlock(_block);
			_backgroundMask.Reset();
			Destroy(_backgrounds[1].gameObject);
			Destroy(_backgrounds[2].gameObject);
			Destroy(_backgrounds[3].gameObject);
			switchingBackground = false;
			// destroy remaining old props
			UiItem[] props = _props.GetComponentsInChildren<UiItem>();
			for (int i = 0; i < props.Length; i++)
			{
				props[i].AfterArrival.Invoke();
			}
		}


		public UiItem Generate(string pathName, int index, Vector3 position, bool inScreenSpace)
		{


			if (index < 0)
			{
				Debug.LogError("error: try to generate item with negative index");
				return null;
			}
			

			Vector3 positionInWorld = position;
			if (inScreenSpace)
			{
				positionInWorld = Coordinate.instance.Screen2Space(positionInWorld);
			}
			GameObject newItem = Resources.Load<GameObject>(pathName + "/" + index.ToString());
			if (newItem == null)
			{
				Debug.LogError("error: try to generate item that doesn't exist");
				return null;
			}
			newItem = Instantiate(newItem);
			newItem.transform.position = positionInWorld;
			
			if (!_nameToParent.ContainsKey(pathName))
			{
				Debug.LogWarning("warning: UiManager generates an item that doesn't have registered  category/parent: "+pathName);
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
