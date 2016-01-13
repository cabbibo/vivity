using UnityEngine;
using System.Collections;

public class PlayTouch : MonoBehaviour {

  private AudioSource audio;
  public AudioClip clip;
  public float pitch;
  public float time;

	// Use this for initialization
	void Start () {

    audio = gameObject.AddComponent<AudioSource>();
 
    audio.clip = clip;
    //audio.playOnAwake = true;
    //audio.Play();
    audio.pitch = pitch;
    audio.spatialize = true;
	  audio.time = time;
    audio.volume = 3;
	}
	
	// Update is called once per frame
	void Update () {
	
	}

  void OnCollisionEnter(Collision col){

    if( col.gameObject.tag == "Hand"){
//      print("ss");
      //audio.volume = col.relativeVelocity.magnitude * 100.0f;
      audio.Play();
    }

  }
}
