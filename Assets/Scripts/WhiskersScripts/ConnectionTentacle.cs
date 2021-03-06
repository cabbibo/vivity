﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ConnectionTentacle : MonoBehaviour {

  public GameObject Tip;
  public GameObject Base;
  public int numPoints;
  public float pointSize;
  public float baseSize;
  public float tipSize;

  public GameObject[] Points;

	// Use this for initialization
	void Start () {

    Points = new GameObject[numPoints+2];
    Points[0] = Base;
    
    Vector3 dif = Tip.transform.position - Base.transform.position;
    float length = dif.magnitude;
    length -= baseSize;
    length -= tipSize;

    dif.Normalize();

    Rigidbody rb;
    SpringJoint sj;
    PlayRandomTouch pt;
    List<AudioClip> Clips = new List<AudioClip>();

    Clips.Add( Resources.Load("Audio/hydra/ArmStroke1") as AudioClip );
    Clips.Add( Resources.Load("Audio/hydra/ArmStroke2") as AudioClip );
    Clips.Add( Resources.Load("Audio/hydra/ArmStroke3") as AudioClip );
    for( int i  = 0; i <numPoints; i++ ){

      GameObject capsule = GameObject.CreatePrimitive(PrimitiveType.Capsule);
      capsule.transform.localScale = new Vector3( pointSize , length / numPoints , pointSize );
      capsule.transform.position = Base.transform.position + baseSize * dif + length * dif * ( ((float)i + 0.5f) / (float)numPoints );
      
      Vector3 a = Vector3.forward;
      a = Quaternion.AxisAngle(Vector3.forward , 90) * dif;
      
      capsule.transform.rotation = Quaternion.FromToRotation(Vector3.up, dif);
      capsule.GetComponent<Renderer>().enabled = false;

      pt =  capsule.AddComponent<PlayRandomTouch>();
      pt.pitch = 1.0f * Mathf.Floor( 3.0f * (float)i / (float)numPoints);
      pt.time = Random.Range( 0 , 10 );
      pt.Clips = Clips;
      pt.volume = 0.6f;

      rb = capsule.AddComponent<Rigidbody>();
      rb.drag = 1;
      rb.angularDrag = .5f;
    
      sj = capsule.AddComponent<SpringJoint>();
      sj.spring = 100;
      sj.damper = 1;
      sj.anchor = new Vector3( 0 , -.5f , 0);

      if( i == 0){
        sj.connectedBody = Base.GetComponent<Rigidbody>();
      }else{
        sj.connectedBody = Points[i].GetComponent<Rigidbody>();
      }

      Points[i+1] = capsule;

    }

    sj = Tip.AddComponent<SpringJoint>();

    sj.connectedBody = Points[numPoints].GetComponent<Rigidbody>();
    sj.spring = 100;
    sj.damper = 3;
    sj.anchor = new Vector3( 0 , -.5f , 0);

    Points[numPoints+1] = Tip;

    addTube();

	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

  void addTube(){

    GameObject cam = GameObject.Find("Camera (eye)");
    Tube tube;
    
    if( cam != null ){
      tube = cam.AddComponent<Tube>();
    }else{
//      print("ss");
      //print(Camera.main.gameObject)
      tube = Camera.main.gameObject.AddComponent<Tube>();
    }

    tube.Points = Points;
    //tube.mainObject = gameObject;
    tube.shader = Shader.Find("Custom/Tube");
    tube.computeShader = (ComputeShader)Resources.Load("Tube");
    tube.handL = GameObject.Find("handL");
    tube.handR = GameObject.Find("handR");
    tube.pointSize = pointSize;
    tube.tipSize = tipSize;
    tube.baseSize = baseSize;
    

  }
}
