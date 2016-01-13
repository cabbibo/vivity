using UnityEngine;
using System.Collections;

public class AddRays : MonoBehaviour {

	// Use this for initialization
	void Start () {
   

    for( int i = 0; i < 40; i++ ){
      GameObject go = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
      go.transform.localScale = new Vector3( 0.5f , 6 , 0.5f );
      go.GetComponent<Renderer>().material.color = new Color(0,0,0,1);
         Shader s = Shader.Find("Custom/RaytraceFloor");
    Material m = new Material( s );
  
    go.GetComponent<Renderer>().material = m;
      //go.transform.position = transform.position;
      //Vector3 lookVec = new Vector3( 0 , -1 , 0 );
      //lookVec.x += Random.Range( -.1f, .1f);
      //lookVec.z += Random.Range( -.1f, .1f);
      //lookVec.Normalize();
      //go.transform.rotation = Quaternion.LookRotation( lookVec);
      float angle = (float)i/40 * 2 * Mathf.PI;
      go.transform.position = new Vector3( Mathf.Sin(angle ) * 6 , 2.3f , Mathf.Cos( angle ) * 6 );

    }
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
