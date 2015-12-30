using UnityEngine;
using System.Collections;

public class Select3D : MonoBehaviour {

  GameObject xAxis;
  GameObject yAxis;
  GameObject zAxis;

  GameObject Marker;
  GameObject Grabber;

  GameObject Background;

  public Vector3 Value;

  private Vector3 localPos;
  private Vector3 fPos;

  // Use this for initialization
  void Start () {

    foreach (Transform child in transform){
      if( child.tag == "Marker" ){ Marker = child.gameObject; }
      if( child.tag == "Axis" ){
        if( child.name == "xAxis" ){ xAxis = child.gameObject; }
        if( child.name == "yAxis" ){ yAxis = child.gameObject; }
        if( child.name == "zAxis" ){ zAxis = child.gameObject; }
      }
      if( child.tag == "Grabber" ){ Grabber = child.gameObject; }
    }
  
  }
  
  // Update is called once per frame
  void Update () {


    localPos = transform.worldToLocalMatrix.MultiplyPoint( Grabber.transform.position );

    float x = Mathf.Clamp( localPos.x , -0.5f , 0.5f);
    float y = Mathf.Clamp( localPos.y , -0.5f , 0.5f);
    float z = Mathf.Clamp( localPos.z , -0.5f , 0.5f);

    xAxis.transform.localPosition = new Vector3( x , y , 0);
    yAxis.transform.localPosition = new Vector3( x , 0 , z);
    zAxis.transform.localPosition = new Vector3( 0 , y , z);

    fPos = new Vector3( x , y , z );

    Marker.transform.localPosition = fPos;

    Value = (fPos + new Vector3(0.5f,0.5f,0.5f));
    Color col = new Color( Value.x , Value.y , Value.z , 1 );

    Marker.GetComponent<Renderer>().material.SetColor("_Color", col );
    xAxis.GetComponent<Renderer>().material.SetColor("_Color", new Color( Value.x , 0 , 0 ,1 ) );
    yAxis.GetComponent<Renderer>().material.SetColor("_Color", new Color( 0 , Value.y , 0 ,1) );
    zAxis.GetComponent<Renderer>().material.SetColor("_Color", new Color( 0 , 0 , Value.z , 1) );


    if(Grabber.GetComponent<MoveByController>().moving == false ){
      Grabber.transform.position = Marker.transform.position;
    }
  
  }
}
