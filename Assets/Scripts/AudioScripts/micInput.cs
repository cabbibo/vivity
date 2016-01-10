using UnityEngine;  
using System.Collections;  
  
[RequireComponent (typeof (AudioSource))]  
public class micInput : MonoBehaviour {


  public string device;
  public AudioSource src;

//Script MicrophoneInput
  void Start(){

    src = GetComponent<AudioSource>();
    print( Microphone.devices );
    if (device == null) device = Microphone.devices[1];
    print( device ); 

    foreach (string vice in Microphone.devices) {
        Debug.Log("Name: " + vice);
        device = vice;
    }

    device = "Stereo Mix (Realtek High Definition Audio)";

    src.clip = Microphone.Start( device , true, 999, 44100);

    while (!(Microphone.GetPosition(device) > 0)){} 

    src.Play();

  }

  void Update(){
   

  }

}