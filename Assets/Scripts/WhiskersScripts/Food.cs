using UnityEngine;
using System.Collections;

public class Food : MonoBehaviour {

  public AudioSource foodLoop;
  public AudioSource eatFood;
  public AudioLowPassFilter filter;

  public float closeness;
  public float sensingRadius;

  private Vector3 ogScale;
  public float death;

  public void DestroyMe(){

    print("YA");

    GameObject tmp = new GameObject();



    


    eatFood = tmp.AddComponent<AudioSource>();
    eatFood.loop = true;
    eatFood.spatialize = true;

    eatFood.clip = Resources.Load("Audio/hydra/EatFood") as AudioClip;
    eatFood.volume = 2.0f;
//    eatFood.forceToMono = true;


    eatFood.Play();

    Destroy(gameObject);
    Destroy( tmp , 2.0f );

  }
	// Use this for initialization
	void Start () {

        ogScale = new Vector3();
    ogScale = transform.localScale;
    print( ogScale );
    death = 1.0f;


    foodLoop = gameObject.AddComponent<AudioSource>();
    foodLoop.loop = true;
    foodLoop.spatialize = true;
//    foodLoop.forceToMono = true;

    foodLoop.clip = Resources.Load("Audio/hydra/FoodLoop") as AudioClip;
    foodLoop.volume = 1.0f;
    foodLoop.Play();

    filter = gameObject.AddComponent<AudioLowPassFilter>();
    filter.cutoffFrequency = 500;





    //eatFood.Play();

	}
	
	// Update is called once per frame
	void Update () {
    //print( closeness);

    float dif = sensingRadius - closeness;
    float ogDif = dif;

    if( dif < 0 ){
     dif = 10; 

     death -= .004f;
     transform.localScale = ogScale * death;// new Vector3( v , v , v );
     foodLoop.volume = death;

     if( death < 0 ){
        closeness = 0;
        Destroy( gameObject );
     }

    }else{ 
      death = 1;
      
      dif = 10 + 3000 * dif; 
     
    }

    filter.cutoffFrequency = dif;

    closeness = 10000000;


	
	}
}
