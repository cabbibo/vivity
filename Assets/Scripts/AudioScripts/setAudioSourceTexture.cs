using UnityEngine;
using System.Collections;

public class setAudioSourceTexture : MonoBehaviour {

  public GameObject sourceObj;
  // Use this for initialization
  void Start () {
  
  }
  
  // Update is called once per frame
  void Update () {

    Texture2D audioTexture = sourceObj.GetComponent<audioSourceTexture>().AudioTexture;
    Renderer r = GetComponent<MeshRenderer>();
    r.material.SetTexture("_MainTex", audioTexture);
  }
}
