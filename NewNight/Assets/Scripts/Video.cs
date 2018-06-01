using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class Video : MonoBehaviour
{

	private RawImage _rawImage;

	private VideoPlayer _videoPlayer;

	private bool flag = true;

	// Use this for initialization
	void Start ()
	{
		_rawImage = GetComponent<RawImage>();
		_videoPlayer = GetComponent<VideoPlayer>();
		
		_videoPlayer.Prepare();
		
	}
	
	// Update is called once per frame
	void Update () {
		if (_videoPlayer.isPrepared && !_videoPlayer.isPlaying)
		{
			if (flag)
			{
				flag = false;
				_rawImage.texture = _videoPlayer.texture;
			}
			_videoPlayer.Play();
			_videoPlayer.Prepare();
		}
		
	
	}
}
