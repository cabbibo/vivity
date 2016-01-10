using UnityEngine;
using System.Collections;

public class SetColorFromValue : MonoBehaviour {

  public GameObject Select3D;
  Camera camera;

	// Use this for initialization
	void Start () {
    camera = GetComponent<Camera>();
    camera.clearFlags = CameraClearFlags.SolidColor;

	}
	
	// Update is called once per frame
	void Update () {

    Vector3 val = Select3D.GetComponent<Select3D>().Value;
    camera.backgroundColor = new Color( val.x , val.y , val.z );
	
	}
}
