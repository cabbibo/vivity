using UnityEngine;
using System.Collections;

public class Interface: MonoBehaviour {

  GameObject Handle;


  // Use this for initialization
  void Start () {

    foreach (Transform child in transform){
      if( child.tag == "Handle" ){ Handle = child.gameObject; }
    }
  
  }
  
  // Update is called once per frame
  void Update () {
    transform.position = Handle.transform.position;
    transform.rotation = Handle.transform.rotation;
  }
}
