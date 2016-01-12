using UnityEngine;
using System.Collections;

public class AddRays : MonoBehaviour {

	// Use this for initialization
	void Start () {
    for( int i = 0; i < 20; i++ ){
      GameObject go = GameObject.CreatePrimitive(PrimitiveType.Plane);
      go.transform.localScale = new Vector3( .05f , 1 , 1 );
      go.transform.position = transform.position;
      Vector3 lookVec = new Vector3( 0 , -1 , 0 );
      lookVec.x += Random.Range( -1, 1);
      lookVec.z += Random.Range( -1, 1);
      lookVec.Normalize();
      go.transform.rotation = Quaternion.LookRotation( lookVec);
    }
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
