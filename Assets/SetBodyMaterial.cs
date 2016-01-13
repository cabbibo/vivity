using UnityEngine;
using System.Collections;

public class SetBodyMaterial : MonoBehaviour {

  public Shader shader;
  // Use this for initialization
  private Material mat;
  void Start () {

    mat = new Material(Shader.Find("Custom/RaytraceFloor"));

    foreach (Transform child in transform){
//      print("YA1");
      if( child.gameObject.GetComponent<Renderer>() != null ){
        child.gameObject.GetComponent<Renderer>().material = mat;
      }
    // do whatever you want with child transform object here
    }

  }
  
  // Update is called once per frame
  void Update () {


  
  }
}
