using UnityEngine;
using System.Collections;


public class MakeBase : MonoBehaviour{

  public GameObject CameraRig;
  public SteamVR_PlayArea PlayArea;

  public float playRadius;
  public GameObject Base;

  void Start(){

    PlayArea = CameraRig.GetComponent<SteamVR_PlayArea>();

    Vector3 v = PlayArea.vertices[0];
    Base = GameObject.CreatePrimitive(PrimitiveType.Cube);
    Base.transform.localScale = new Vector3( Mathf.Abs( v.x )  * 2.0f ,1.0f ,  Mathf.Abs( v.z ) * 2.0f);
    Base.transform.position = new Vector3( 0f , -0.4f , 0f );
    Shader s = Shader.Find("Custom/RaytraceFloor");
    Material m = new Material( s );

    //Base.GetComponent<Renderer>().material = m;
    //Base.GetComponent<CapsuleCollider>().enabled = false;
    Base.AddComponent<BoxCollider>();
    Base.GetComponent<Renderer>().material.color = new Color(0,0,0,1);
    Base.AddComponent<AddWhiskersSea>();

    // playRadius = Mathf.min( Mathf.abs( PlayArea.vertices[0].x ) , 

  }

  void Update(){

  }


}