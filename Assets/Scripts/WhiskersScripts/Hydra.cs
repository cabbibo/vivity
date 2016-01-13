using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Hydra : MonoBehaviour {
  

  public Vector3 startHeight = new Vector3( 0 , 1.25f , 0 );
    
  public float baseScale = 0.5f;
  public float centerScale = 0.3f;
  public float tipScale = 0.15f;

  public int   armPoints = 5;
  public float armLength = 0.6f;
  public float armWidth  = 0.05f;

  public int   stalkPoints = 8;
  public float stalkLength = 1.25f;
  public float stalkWidth  = 0.1f;
  
  private GameObject Center;
  private GameObject Base;
  //private GameObject Tips;
  private List<GameObject> Tips;
	// Use this for initialization

  private Vector3[] directions;


  private PlayTouch pt;


	void OnEnable () {

    Tips = new List<GameObject>();

    directions = new Vector3[] {
      new Vector3(  1 , 0 ,  0 ),
      new Vector3( -1 , 0 ,  0 ),
      new Vector3(  0 , 0 ,  1 ),
      new Vector3(  0 , 0 , -1 ),
      new Vector3(  0 , 1 ,  0 )
    };
    Rigidbody rb;

    Base = GameObject.CreatePrimitive(PrimitiveType.Cube);
    Base.transform.localScale = Base.transform.localScale *  baseScale;
    Base.GetComponent<Renderer>().enabled = false;
    Base.transform.position = Base.transform.position - new Vector3( 0 ,0.25f,0);
    rb = Base.AddComponent<Rigidbody>();
    
    rb.isKinematic = true;


    MakeBase mb = Base.AddComponent<MakeBase>();
    mb.CameraRig = GameObject.Find("[CameraRig]");
    
    
    Center = GameObject.CreatePrimitive(PrimitiveType.Cube);
    Center.transform.localScale = Center.transform.localScale * centerScale;
    Center.transform.position = Vector3.up * stalkLength;
    Center.AddComponent<AddWhiskerBox>();
    rb = Center.AddComponent<Rigidbody>();
    rb.drag = .1f;
    rb.angularDrag = 0.5f;
    //rb.isKinematic = false;

    pt =  Center.AddComponent<PlayTouch>();
    pt.pitch = 1;
    //pt.time = 1;

    pt.clip = Resources.Load("Audio/hydra/BaseHit") as AudioClip;


    
    //Base.transform.position = ;

    ConnectionTentacle ct = Center.AddComponent<ConnectionTentacle>();
    ct.Tip = Center;
    ct.Base = Base;
    ct.numPoints = stalkPoints;
    ct.pointSize = stalkWidth;
    ct.baseSize  = baseScale /2.0f;
    ct.tipSize   = centerScale/2.0f;


    LookForFood lff;

    for( int i = 0; i < directions.Length; i++ ){

      GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
      cube.transform.localScale = cube.transform.localScale * 0.15f;
      cube.transform.position = directions[i] * armLength + Center.transform.position; 
      cube.AddComponent<AddWhiskerBox>();
      rb = cube.AddComponent<Rigidbody>();
      rb.drag = 1;
      rb.angularDrag = 0.5f;

      ct = cube.AddComponent<ConnectionTentacle>();
      ct.Tip = cube;
      ct.Base = Center;
      ct.numPoints = armPoints;
      ct.pointSize = armWidth;
      ct.baseSize  = centerScale /2.0f;
      ct.tipSize   = tipScale/2.0f;

      lff = cube.AddComponent<LookForFood>();
      lff.sensingRadius = 1.0f;

      pt =  cube.AddComponent<PlayTouch>();
      pt.pitch = 1.0f;
      //pt.time = (float)i/directions.Length;
      string name = "Audio/hydra/TipHit" + (i+1);
      print( name );

      pt.clip = Resources.Load(name) as AudioClip;

      Tips.Add( cube );



    }
	
	}

	// Update is called once per frame
	void Update () {

    for( int i = 0; i < directions.Length; i++){

      Vector3 targetPos = Center.transform.rotation * (directions[i] * armLength) + Center.transform.position;
      targetPos -= Tips[i].transform.position;
      targetPos.Normalize();
      targetPos *= .1f;
      Rigidbody rb = Tips[i].GetComponent<Rigidbody>();
      rb.AddForce( targetPos );
    }
	
	}
}
