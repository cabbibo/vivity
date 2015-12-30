using UnityEngine;
using System.Collections;

public class MoveByController : MonoBehaviour {


  public Transform ogTransform;
  public bool moving;

  private bool inside;


  Collider colInside;

	void OnEnable(){
    EventManager.OnTriggerDown += OnTriggerDown;
    EventManager.OnTriggerUp += OnTriggerUp;
    EventManager.StayTrigger += StayTrigger;
    inside = false;
    moving = false;
    ogTransform = transform.parent;
  }

	// Update is called once per frame
	void Update () {

	
	}

  void OnTriggerDown(GameObject o){
    if( inside == true ){
      transform.SetParent(o.transform);
      moving = true;
    }
  }

  void OnTriggerUp(GameObject o){
    transform.SetParent(ogTransform);
    moving = false;
  }


  void StayTrigger(GameObject o){
//    print("ff");
  }


  void onCollisionEnter(){
    print( "check" );
  }

  void onTriggerEnter(){
    print( "check" );
  }

  void OnTriggerEnter(Collider Other){

    if( Other.tag == "Hand"){ 
      colInside = Other;
      inside = true; 
    }
  }

  void OnTriggerExit(Collider Other){
    if( Other.tag == "Hand" && Other == colInside){ 
      colInside = null;
      inside = false; 
    }
  }

}
