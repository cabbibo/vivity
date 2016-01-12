using UnityEngine;
using System.Collections;

public class LookForFood : MonoBehaviour {

  public float sensingRadius;

  private GameObject[] foodPieces;

  private Rigidbody rb;

  // TODO:
  // only look for closest piece of food

	// Use this for initialization
	void Start () {

    rb = GetComponent<Rigidbody>();
	
	}
	
	// Update is called once per frame
	void Update () {

    foodPieces = GameObject.FindGameObjectsWithTag("Food");

    for( int i = 0; i < foodPieces.Length; i++ ){

      Vector3 dif = foodPieces[i].transform.position - transform.position;
      float l = dif.magnitude;
      if( l < sensingRadius ){
        dif.Normalize();
        rb.AddForce(dif);
      }



    }
	
	}
  
  void OnCollisionEnter(Collision col){
    if( col.gameObject.tag == "Food" ){
      Destroy(col.gameObject);
    }

  }
}
