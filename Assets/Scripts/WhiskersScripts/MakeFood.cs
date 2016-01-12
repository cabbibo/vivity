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


    audio = go.AddComponent<AudioSource>();
    audio.loop = true;
    audio.spatialize = true;

    audio.clip = Resources.Load("Audio/feedback noise") as AudioClip;
    audio.volume = 0.1f;
    audio.Play();

  }

	// Update is called once per frame
	void Update () {
	
	}
}
