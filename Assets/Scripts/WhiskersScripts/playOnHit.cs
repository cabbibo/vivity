using UnityEngine;
using System.Collections;

public class playOnHit : MonoBehaviour {

  AudioSource aS;
	// Use this for initialization
	void Start () {
    aS = GetComponent<AudioSource>();
    aS.pitch = .8f * Random.Range( 1 , 3 );
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
  void OnCollisionEnter(){
    aS.Play();
  }
}
