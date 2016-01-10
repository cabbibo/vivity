﻿using UnityEngine;
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

    Base = GameObject.CreatePrimitive(PrimitiveType.Sphere);
    Base.transform.localScale = Base.transform.localScale *  baseScale;
    rb = Base.AddComponent<Rigidbody>();
    rb.isKinematic = true;

    Center = GameObject.CreatePrimitive(PrimitiveType.Cube);
    Center.transform.localScale = Center.transform.localScale * centerScale;
    Center.transform.position = Vector3.up * stalkLength;
    Center.AddComponent<AddWhiskerBox>();
    rb = Center.AddComponent<Rigidbody>();
    rb.drag = 10;
    rb.angularDrag = 10;
    //rb.isKinematic = false;

    
    //Base.transform.position = ;

    ConnectionTentacle ct = Center.AddComponent<ConnectionTentacle>();
    ct.Tip = Center;
    ct.Base = Base;
    ct.numPoints = stalkPoints;
    ct.pointSize = stalkWidth;
    ct.baseSize  = baseScale /2.0f;
    ct.tipSize   = centerScale/2.0f;


    for( int i = 0; i < directions.Length; i++ ){

      GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
      cube.transform.localScale = cube.transform.localScale * 0.15f;
      cube.transform.position = directions[i] * armLength + Center.transform.position; 
      cube.AddComponent<AddWhiskerBox>();
      rb = cube.AddComponent<Rigidbody>();
      rb.drag = 10;
      rb.angularDrag = 10;

      ct = cube.AddComponent<ConnectionTentacle>();
      ct.Tip = cube;
      ct.Base = Center;
      ct.numPoints = armPoints;
      ct.pointSize = armWidth;
      ct.baseSize  = centerScale /2.0f;
      ct.tipSize   = tipScale/2.0f;



    }
	
	}

	// Update is called once per frame
	void Update () {
	
	}
}