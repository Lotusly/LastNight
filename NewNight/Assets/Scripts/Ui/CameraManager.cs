using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Rendering.PostProcessing;

namespace Ui
{
	public class CameraManager : MonoBehaviour
	{
		[SerializeField] private UiCamera _main;

		[SerializeField] private UiCamera _background;
		private static int _baseLayer = 9;
		private Dictionary<string, UiCamera> _cameraList;
		private UiCamera[] _layerSlots;
		private PostProcessVolume[] _volumes;
		void Awake()
		{
			Supportive.PostEffects.instance.CheckResources();
			_cameraList = new Dictionary<string, UiCamera>();
			_cameraList.Add("Main", _main);
			_cameraList.Add("Background", _background);
			_layerSlots = new UiCamera[6] ;
			_volumes = GetComponentsInChildren<PostProcessVolume>();
		}
	
		


		private int FindAccessLayer()
		{
			for (int i = 0; i < 6; i++)
			{
				if (_layerSlots[i]==null) return i;
			}

			return -1;
		}

		private int FindCameraIndex(UiCamera cam)
		{
			for (int i = 0; i < 6; i++)
			{
				if (_layerSlots[i] == cam)
					return i;
			}

			return -1;
		}

		public bool NewCamera(string name, int depth)
		{
			if (_cameraList.ContainsKey(name)) return false;
			int layerFound = FindAccessLayer();
			if (layerFound < 0) return false;
			
			
			GameObject newObject = new GameObject(name);
			Camera newCamera = newObject.AddComponent<Camera>();
			UiCamera newUiCamera = newObject.AddComponent<UiCamera>();
			newObject.AddComponent<PhysicsRaycaster>();
			newObject.AddComponent<PostProcessLayer>().volumeLayer=1<<(_baseLayer+layerFound);
			newCamera.depth = depth;
			newCamera.clearFlags = CameraClearFlags.Depth;
			newCamera.cullingMask = 1 << (_baseLayer + layerFound);
			_layerSlots[layerFound] = newUiCamera;
			_cameraList.Add(name,newUiCamera);
			newUiCamera.Volume = _volumes[2 + layerFound];
			return true;
		}

		public bool DestroyCamera(string name)
		{
			if (!_cameraList.ContainsKey(name)) return false;
			UiCamera uiCamera = _cameraList[name];
			int i = FindCameraIndex(uiCamera);
			if (i < 0) return false;
			
			_layerSlots[i] = null;
			_cameraList.Remove(name);
			Destroy(uiCamera.gameObject);
			return true;
		}

		public UiCamera GetCamera(string name)
		{
			if (!_cameraList.ContainsKey(name)) return null;
			return _cameraList[name];
		}
		
		
	
	}
}
