
using System.Collections.Generic;
using Supportive;
using UnityEngine;
using System.Collections;



namespace Ui
{

	public class UiManager : Supportive.Singleton<UiManager>
	{
		struct Item // TEMP : this will be replaced by the new transition system
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
		[SerializeField] private Transform _props; // this transform contains both props (static picture) and midground (floating picture)
		[SerializeField] private Transform _backgroundParent;
		public UiCamera _camera;

		private bool exitable = false;
		private bool switchingBackground = false;

		[SerializeField] private UiMask _backgroundMask;
		
		private Renderer[] _backgrounds;
		private MaterialPropertyBlock _block;
		[SerializeField] private Material[] _backgroundMats;

		
		private Texture _tmpTex;

		private List<Item> _potentialItems; // used to switch props

		[SerializeField] private StatBar _statBar;

		[SerializeField] private SceneManager _sceneManager;
		// TEMP
		[SerializeField] private Dialogue _singleDialogue;

		
		
		void Start()
		{			
			_potentialItems = new List<Item>();
			_backgrounds=new Renderer[4];
			_block=new MaterialPropertyBlock();

			_backgroundMask.Initialize();

			Story.instance.Initialize();
			
		}

	

		private void PlaceBackground(int index, int stencilLayer, Vector3 worldPosition,  float lighting)
		{
			_backgrounds[stencilLayer]=GenerateInPresentScene("Backgrounds/"+index.ToString(), worldPosition, false).gameObject.GetComponent<Renderer>();
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

		public void FadeOutPresentScene(string name, int method=0, int[] arguments = null)
		{
			
		}
		
		public void FadeInScene(string name, Transitions.TransitionParameterBlock methods)
		{
			
			_sceneManager.SwitchScene(name);
			
			
		}
		
		

		

		public void SwitchBackground(int index, Vector2 direction=new Vector2())
		{
			
			if (!switchingBackground)
			{
				//TEST
				Test.instance.index = (Test.instance.index + 1) % 2;
				
				float randomSeed = Random.value-0.5f;
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
				UiItem prop = GenerateInPresentScene("Props"+_potentialItems[i].Index.ToString(), _potentialItems[i].PositionInScreen + direction, true);
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


		public bool CreateScene(string name)
		{
			return _sceneManager.NewScene(name);
		}

		public bool DestroyScene(string name)
		{
			return _sceneManager.DeleteScene(name);
		}

		
		public UiItem GenerateInScene(string sceneName, string objectPath, Vector3 position, bool inScreenSpace,
			string kindName = "", bool initialize = true)
		{
			if (inScreenSpace) position = Coordinate.instance.Screen2Space(position);
			return  _sceneManager.GenerateItem(sceneName,objectPath,position,kindName,initialize);
		}

		public UiItem GenerateInPresentScene(string objectPath, Vector3 position, bool inScreenSpace,
			string kindName = "", bool initialize = true)
		{
			if (inScreenSpace) position = Coordinate.instance.Screen2Space(position);
			return _sceneManager.GenerateItem(_sceneManager.GetPresentSceneName(), objectPath, position, kindName, initialize);
		}

		


		/* Now only has one single dialogue
		public Dialogue GenerateDialogue(int index, Vector3 positionInScreen, Vector3 potentialPositionInScreen)
		{
			Dialogue item = (Dialogue) Generate("Dialogues", index, positionInScreen, true, false);
			if (item == null)
			{
				Debug.LogError("UiManager.GenerateDialogue: Resources/Dialogues/"+index.ToString()+" does not exist");
				return null;
			}
			item.Initialize(potentialPositionInScreen);
			return item;
		} */

		public void SetDialogueCon(Dialogue.DialogueContaining newDiaCon, bool updateNow=true)
		{
			_singleDialogue.SetDialogueContaining(newDiaCon,updateNow);
		}

		public void ClearDialogue()
		{
			_singleDialogue.ClearDialogue();
		}

		public void ModifyStat(string name, int addition)
		{
			_statBar.ModifyStat(name,addition);
		}

		public bool CheckStat(string name, int addition)
		{
			return _statBar.CheckStat(name, addition);
		}

		public void ZoomIn(Character focus)
		{
			exitable = true;
			_camera.Transfer(new Vector3(0,0,3),false,false);
			MoveOutGroup(_characters,focus);
			MoveOutGroup(_foregroundItems,focus);
			focus.transform.parent = _portraits;
			// TEMP
			Story.instance.OnClick(0);
			_singleDialogue.MoveOut();
			
		}
		
		// TEMP
		public void CallDialogue()
		{
			Story.instance.OnClick(0);
			_singleDialogue.MoveOut();
		}

		public void ZoomOut()
		{
			if (exitable)
			{
				exitable = false;
				_camera.MoveBack();
				MoveBackGroup(_portraits,_characters);
				MoveBackGroup(_foregroundItems);
				// TEMP
				_singleDialogue.MoveBack();
			}

		}

		private void MoveOutGroup(Transform parent, UiItem focus)
		{
			int numChildren = parent.childCount;
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
			int numChildren = parent.childCount;
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
