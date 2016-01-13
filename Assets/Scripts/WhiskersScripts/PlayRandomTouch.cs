using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayRandomTouch : MonoBehaviour {

  private AudioSource audio;
  public List<AudioClip> Clips;
  public float pitch;
  public float time;
  public float volume = 1.0f;

  // Use this for initialization
  void Start () {

    audio = gameObject.AddComponent<AudioSource>();
 
   // audio.clip = clip;
    //audio.playOnAwake = true;
    //audio.Play();
    audio.pitch = pitch;
    audio.spatialize = true;
    audio.time = time;
    audio.volume = volume;
  }
  
  // Update is called once per frame
  void Update () {
  
  }

  void OnCollisionEnter(Collision col){

    if( col.gameObject.tag == "Hand"){
//      print("ss");
      //audio.volume = col.relativeVelocity.magnitude * 100.0f;
      audio.clip = Clips[Random.Range( 0, Clips.Count )];
      audio.Play();
    }

  }
}
