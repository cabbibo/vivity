using UnityEngine;
using System.Collections;

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

    for( int i  = 0; i <numPoints; i++ ){

      GameObject capsule = GameObject.CreatePrimitive(PrimitiveType.Capsule);
      capsule.transform.localScale = new Vector3( pointSize , length / numPoints , pointSize );
      capsule.transform.position = Base.transform.position + baseSize * dif + length * dif * ( ((float)i + 0.5f) / (float)numPoints );
      Vector3 a = Vector3.forward;
      a = Quaternion.AxisAngle(Vector3.forward , 90)  * dif;
      capsule.transform.rotation = Quaternion.FromToRotation(Vector3.up, dif);
      capsule.GetComponent<Renderer>().enabled = false;

      rb = capsule.AddComponent<Rigidbody>();
      rb.drag = 3;
      rb.angularDrag = 1;
    
      sj = capsule.AddComponent<SpringJoint>();
      sj.spring = 100;
      sj.damper = 3;
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
    

  }
}
