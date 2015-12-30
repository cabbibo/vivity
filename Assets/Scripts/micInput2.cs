using UnityEngine;
using System.Collections;

[RequireComponent(typeof(AudioSource))]
public class micInput2 : MonoBehaviour
{
    void Start()
    {
        GetComponent<AudioSource>().Stop();

        GetComponent<AudioSource>().loop = true;

        int minFreq, maxFreq;
        Microphone.GetDeviceCaps(null, out minFreq, out maxFreq);

        GetComponent<AudioSource>().clip = Microphone.Start(null, true, 1, maxFreq > 0 ? maxFreq : 44100);

        while (GetComponent<AudioSource>().clip != null)
        {
            int delay = Microphone.GetPosition(null);
            if (delay > 0)
            {
                GetComponent<AudioSource>().Play();
                Debug.Log("Latency = " + (1000.0f / GetComponent<AudioSource>().clip.frequency * delay) + " msec");
                break;
            }
        }
    }

    void OnApplicationPause(bool paused)
    {
        if (paused)
        {
            GetComponent<AudioSource>().Stop();
            Microphone.End(null);
            Debug.Log("paused");
        }
        else
        {
            Start();
        }
    }
}