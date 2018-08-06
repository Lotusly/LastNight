
using System.Collections.Generic;
using Supportive;
using UnityEngine;
using System.Collections;


namespace Ui
{
	// Singleton: only exist one 
	public class UiManager : Singleton<UiManager>
	{
		
		
		[SerializeField] private Transform _generalParent;
		[SerializeField] private Transform _characters;
		[SerializeField] private Transform _portraits;
		[SerializeField] private Transform _dialogues;
		[SerializeField] private Transform _foregroundItems;
		public UiCamera _camera;

		private bool exitable = false;
		private bool switchingBackground = false;

		[SerializeField] private UiMask _backgroundMask;
		
		private Renderer[] _backgrounds;
		private MaterialPropertyBlock _block;
		[SerializeField] private Material[] _backgroundMats;

		
		private Texture _tmpTex;

		[SerializeField] private StatBar _statBar;

		[SerializeField] private SceneManager _sceneManager;
		// For now use single dialogue
		[SerializeField] private Dialogue _singleDialogue;

		
		
		void Start()
		{		
			// followings are dated transition setup
			//_backgrounds=new Renderer[4];
			//_block=new MaterialPropertyBlock();
			//_backgroundMask.Initialize();
			
			Story.instance.Initialize();
			
		}


		#region transition
		//-------------------------------------------------------------------------------------------------------
		// Perform transition
		//-----------------------------------------------------------------------------------------------------
		
		/// <summary>
		/// Call this fucntion to fade out a certain scene (not necessarily the present scene)
		/// </summary>
		/// <param name="name">The name of the scene to fade out</param>
		/// <param name="block">Parameters that defines the transition</param>
		public void FadeOutScene(string name, TransitionForm.TransitionParameterBlock block)
		{
			SceneManager.instance.TransferScene(name,block,true);
		}
		
		
		/// <summary>
		/// Call this function to fade in a scene
		/// </summary>
		/// <param name="name">The name of the scene to fade out</param>
		/// <param name="block">Parameters that defines the transition</param>
		public void FadeInScene(string name, TransitionForm.TransitionParameterBlock block)
		{
			
			SceneManager.instance.SwitchScene(name);
			SceneManager.instance.TransferScene(name, block);
					
		}
		
		/// <summary>
		/// Fade out the present scene
		/// </summary>
		/// <param name="block">Parameters that defines the transition</param>
		public void FadeOutPresentScene(TransitionForm.TransitionParameterBlock block)
		{
			SceneManager.instance.TransferScene(SceneManager.instance.GetPresentSceneName(), block,true);
		}
		#endregion
		
		#region scene management
		//---------------------------------------------------------------------------------------------------------------
		// Manager scenes by calling functions in SceneManager
		//------------------------------------------------------------------------------------------------------------

		public bool CreateScene(string name)
		{
			return _sceneManager.NewScene(name);
		}

		public bool DestroyScene(string name)
		{
			return _sceneManager.DeleteScene(name);
		}


		public bool SwitchScene(string name)
		{
			return _sceneManager.SwitchScene(name);
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
		#endregion
		
		#region dialogue control
		//-------------------------------------------------------------------------------------------------------
		// Manager dialogue contents
		//-----------------------------------------------------------------------------------------------------

		public void SetDialogueCon(Dialogue.DialogueContaining newDiaCon, bool updateNow=true)
		{
			_singleDialogue.SetDialogueContaining(newDiaCon,updateNow);
		}

		public void ClearDialogue()
		{
			_singleDialogue.ClearDialogue();
		}
		#endregion

		#region statebar control
		//-------------------------------------------------------------------------------------------------------
		// Manager statbar
		//-----------------------------------------------------------------------------------------------------
		public void ModifyStat(string name, int addition)
		{
			_statBar.ModifyStat(name,addition);
		}

		public bool CheckStat(string name, int addition)
		{
			return _statBar.CheckStat(name, addition);
		}
		#endregion

		
		#region dated batch control
		//-----------------------------------------------------------------------------------------------------------------
		// An dated system for items batch control
		// Call the MoveOut/MoveBack function in UiItem class, to conveniently move it between two positions.
		//--------------------------------------------------------------------------------------------------------------
		
		public void ZoomIn(Character focus)
		{
			exitable = true;
			_camera.Transfer(new Vector3(0,0,3),false);
			MoveOutGroup(_characters,focus);
			MoveOutGroup(_foregroundItems,focus);
			focus.transform.parent = _portraits;
			// TEMP
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
		#endregion
		
		
		
		#region dated transition
		//--------------------------------------------------------------------------------------------------
		// The old version of transition.
		// Now cannot work anymore
		// https://dejobaan.slack.com/files/UARV9SNAU/FB7527SQ2/aftereffect4.gif
		//-------------------------------------------------------------------------------------------------
		
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
		
		/// <summary>
		/// Perform the transition.
		/// </summary>
		/// <param name="index">Which background to transfer to.</param>
		/// <param name="direction">Transfer in which direction.</param>
		public void SwitchBackground(int index, Vector2 direction=new Vector2())
		{
			if (!switchingBackground)
			{			
				float randomSeed = Random.value-0.5f;
				switchingBackground = true;	
				PlaceBackground(index, 1, new Vector3(0, 0, 20-randomSeed*5),  -.4f);
				_backgrounds[1].gameObject.GetComponent<UiItem>().Transfer(_backgrounds[1].transform.position+new Vector3(0,0,1)*5*randomSeed,false);
				PlaceBackground(index, 2, new Vector3(0, 0, 20-randomSeed*5),  0.4f);
				_backgrounds[2].gameObject.GetComponent<UiItem>().Transfer(_backgrounds[2].transform.position+new Vector3(0,0,1)*5*randomSeed,false);
				PlaceBackground(index, 3, new Vector3(0, 0, 30-randomSeed*7.5f), 0f);
				_backgrounds[3].gameObject.GetComponent<UiItem>().Transfer(_backgrounds[3].transform.position+new Vector3(0,0,1)*7.5f*randomSeed,false);
				if (direction.x > direction.y)
				{
					if (direction.x > -direction.y)
					{
						_backgrounds[0].gameObject.GetComponent<UiItem>().Transfer(
							Coordinate.instance.Space2Screen(_backgrounds[0].transform.position) - Vector3.right*2, true, false, 2,
							0.7f);
					}
					else
					{
						_backgrounds[0].gameObject.GetComponent<UiItem>().Transfer(
							Coordinate.instance.Space2Screen(_backgrounds[0].transform.position) - Vector3.down*2, true, false, 2,
							0.7f);
					}
				}
				else
				{
					if (direction.x > -direction.y)
					{
						_backgrounds[0].gameObject.GetComponent<UiItem>().Transfer(
							Coordinate.instance.Space2Screen(_backgrounds[0].transform.position) - Vector3.up*2, true, false, 2, 0.7f);
					}
					else
					{
						_backgrounds[0].gameObject.GetComponent<UiItem>().Transfer(
							Coordinate.instance.Space2Screen(_backgrounds[0].transform.position) - Vector3.left*2, true, false, 2,
							0.7f);
					}
				}
				_backgroundMask.SwitchBackground(direction);			
			}

		}
		
		
		// Called by UiMask script, after the old transition is done.
		public void AfterBackground()
		{
			_backgrounds[0].transform.SetPositionAndRotation(_backgrounds[3].transform.position,_backgrounds[3].transform.rotation);
			_backgrounds[0].transform.localScale = _backgrounds[3].transform.localScale;
			_backgrounds[3].GetPropertyBlock(_block);
			_backgrounds[0].SetPropertyBlock(_block);
			_backgroundMask.Reset();
			Destroy(_backgrounds[1].gameObject);
			Destroy(_backgrounds[2].gameObject);
			Destroy(_backgrounds[3].gameObject);
			switchingBackground = false;
		}
		#endregion
		
	}
}
