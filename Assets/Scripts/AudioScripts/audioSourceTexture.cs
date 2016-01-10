using UnityEngine;
using System.Collections;

public class audioSourceTexture : MonoBehaviour {

  public Texture2D AudioTexture;
  public AudioSource src;

  public Color[] colors = new Color[256];


  float[] spectrum = new float[2048];


 // private AudioListener audio;
  // Use this for initialization
  void Start () {



    AudioTexture = new Texture2D(256, 1, TextureFormat.ARGB32, false);
    AudioTexture.filterMode = FilterMode.Trilinear;

    Color[] cols = AudioTexture.GetPixels( 0 );
    
    src = GetComponent<AudioSource>();
  
  }
  
  // Update is called once per frame
  void Update () {


    src.GetSpectrumData(spectrum, 0, FFTWindow.Triangle);
    
    Color[] cols = AudioTexture.GetPixels( 0 );
    Color c = new Color(0,0,0,0);

    for( int i = 0; i < 256; i++){
     // c.r = 
      //print( cols[i] );

      c.r = spectrum[ i * 4 + 0 ];
      c.g = spectrum[ i * 4 + 1 ];
      c.b = spectrum[ i * 4 + 2 ];
      c.a = spectrum[ i * 4 + 3 ];

      cols[i] = c;//Color.Lerp( cols[i] , c , .9f );

    }

    AudioTexture.SetPixels( cols , 0 );
    AudioTexture.Apply();
    //print( spectrum[0] );

    //texture.Apply();
  
  }
}
