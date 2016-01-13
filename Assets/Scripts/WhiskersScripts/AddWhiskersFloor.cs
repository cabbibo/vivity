using UnityEngine;
using System.Collections;

public class AddWhiskersFloor : MonoBehaviour {

  // Use this for initialization
  void Start () {


//    print( Camera.main );

    GameObject cam = GameObject.Find("Camera (eye)");
    WhiskersFloor wb;
    
    if( cam != null ){
      wb = cam.AddComponent<WhiskersFloor>();
    }else{
//      print("ss");
      //print(Camera.main.gameObject)
      wb = Camera.main.gameObject.AddComponent<WhiskersFloor>();
    }

//    print( wb );
    wb.mainObject = gameObject;
    wb.stalkShader = Shader.Find("Custom/Whiskers");
    wb.tipShader = Shader.Find("Custom/Tips");
    wb.baseShader = Shader.Find("Custom/Floor");
    wb.computeShader = (ComputeShader)Resources.Load("WhiskersFloor");
    wb.handL = GameObject.Find("handL");
    wb.handR = GameObject.Find("handR");
    wb.Select3D = GameObject.Find("Select3D");
    wb.audioObj = GameObject.Find("Camera (head)");
  }
  
  // Update is called once per frame
  void Update () {
  
  }
}
