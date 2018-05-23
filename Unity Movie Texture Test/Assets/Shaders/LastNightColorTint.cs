///////////////////////////////////////////
//  CameraFilterPack - by VETASOFT 2016 ///
///////////////////////////////////////////
using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
[AddComponentMenu ("LastNightColorTint")]
public class LastNightColorTint : MonoBehaviour {
	#region Variables
	public Shader SCShader;
	private Material SCMaterial;

    [Range(0f, 1f)]
    public float LightIntensity = 1f;  

    public Color ChangeColorRGB;


  #endregion
	
	#region Properties
	Material material
	{
		get
		{
			if(SCMaterial == null)
			{
				SCMaterial = new Material(SCShader);
				SCMaterial.hideFlags = HideFlags.HideAndDontSave;	
			}
			return SCMaterial;
		}
	}
	#endregion
	void Start () 
	{
     	SCShader = Shader.Find("LastNightColorTint");

		if(!SystemInfo.supportsImageEffects)
		{
			enabled = false;
			return;
		}
	}
	
	void OnRenderImage (RenderTexture sourceTexture, RenderTexture destTexture)
	{
		if(SCShader != null)
		{
            material.SetFloat("_LightIntensity", LightIntensity);
            material.SetColor("_ColorRGB", ChangeColorRGB);
          	material.SetVector("_ScreenResolution",new Vector4(sourceTexture.width,sourceTexture.height,0.0f,0.0f));
            GetComponent<Camera>().depthTextureMode = DepthTextureMode.Depth;
        
			Graphics.Blit(sourceTexture, destTexture, material);

		}
		else
		{
			Graphics.Blit(sourceTexture, destTexture);	
		}
		
	}

	void Update () 
	{
		#if UNITY_EDITOR
		if (Application.isPlaying!=true)
		{
			SCShader = Shader.Find("LastNightColorTint");
		}
		#endif
	}
	
	void OnDisable ()
	{
		if(SCMaterial)
		{
			DestroyImmediate(SCMaterial);	
		}
	}
	
	
}



