using UnityEngine;
using System.Collections;

public class AddWhiskersSea : MonoBehaviour {

  // Use this for initialization
  void Start () {


//    print( Camera.main );

    GameObject cam = GameObject.Find("Camera (eye)");
    WhiskersSea wb;
    
    if( cam != null ){
      wb = cam.AddComponent<WhiskersSea>();
    }else{
//      print("ss");
      //print(Camera.main.gameObject)
      wb = Camera.main.gameObject.AddComponent<WhiskersSea>();
    }
    print("YOOOOO");

//    print( wb );
    wb.mainObject = gameObject;
    wb.stalkShader = Shader.Find("Custom/seaWhiskers");
    wb.tipShader   = Shader.Find("Custom/seaTips");
     wb.baseShader = Shader.Find("Custom/seaFloor");
    wb.computeShader = (ComputeShader)Resources.Load("WhiskersSea");
    wb.handL = GameObject.Find("handL");
    wb.handR = GameObject.Find("handR");
    wb.Select3D = GameObject.Find("Select3D");
    wb.audioObj = GameObject.Find("Camera (head)");
  }
  
  // Update is called once per frame
  void Update () {
  
  }
}
