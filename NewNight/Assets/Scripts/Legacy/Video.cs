﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class Video : MonoBehaviour
{
	//private RawImage _rawImage;
	private VideoPlayer _videoPlayer;
	//private bool flag = true;

	void Start ()
	{
		//_rawImage = GetComponent<RawImage>();
		_videoPlayer = GetComponent<VideoPlayer>();
		_videoPlayer.Prepare();
	}

	void Update()
	{
			if (_videoPlayer.isPrepared && !_videoPlayer.isPlaying)
			{
				_videoPlayer.Play();
				_videoPlayer.Prepare();
			}
	}	
}
