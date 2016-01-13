using UnityEngine;
using System.Collections;


public class MakeFood : MonoBehaviour {

  private GameObject go;
  private AudioSource audio;

  void OnEnable(){
    EventManager.OnTriggerDown += OnTriggerDown;
  }

  void OnTriggerDown(GameObject o){
    go = GameObject.CreatePrimitive(PrimitiveType.Cube);
    go.transform.localScale = go.transform.localScale * 0.05f;
    go.tag = "Food";
    go.transform.position = o.transform.position;
    go.transform.rotation = o.transform.rotation;
    go.transform.position = go.transform.position + ( go.transform.rotation * (Vector3.forward * .1f) );
    go.AddComponent<Rigidbody>();

    go.AddComponent<Food>();
    Shader s = Shader.Find("Custom/RaytraceFood");
    Material m = new Material( s );
  
    go.GetComponent<Renderer>().material = m;

   

  }

	// Update is called once per frame
	void Update () {
	
	}
}
