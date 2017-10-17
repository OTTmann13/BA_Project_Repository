using System.Collections;
using UnityEngine;

public class TrainController : MonoBehaviour {

	public float movementSpeed = 10;
	public bool triggered = false;

    private float maxVolumeTime = 4.0f;
    private float volumeAlpha;
    private float currentVolumeTime;
    private bool raiseVolume = false;
    private AudioSource audioSource;

	// Use this for initialization
	void Start () {
        audioSource = GetComponent<AudioSource>();
        audioSource.volume = 0;
	}
	
	// Update is called once per frame
	void Update () {
		if (triggered) {
			transform.Translate (Vector3.forward * movementSpeed * Time.deltaTime);
		}

        if (raiseVolume)
        {
            currentVolumeTime += Time.deltaTime;

            audioSource.volume += Time.deltaTime / (maxVolumeTime * 2);

            if (currentVolumeTime > maxVolumeTime)
            {
                triggered = true;
                raiseVolume = false;
            }
        }

	}
    
    //Trigger the train to move forward
	public void Trigger() {
        //StartCoroutine(TrainRoutine());
        //TrainRoutine();
        audioSource.Play();
        raiseVolume = true;

	}

    private void TrainRoutine() {

        //currentVolumeTime += Time.deltaTime;

        Debug.Log(Time.deltaTime / maxVolumeTime);

        audioSource.volume = 0;
        audioSource.Play();

        for (float f = 0; f < maxVolumeTime; f += Time.deltaTime) {
            audioSource.volume += Time.deltaTime / maxVolumeTime;
            Debug.Log(audioSource.volume);
        }

        //yield return new WaitForSeconds(2);

        triggered = true;
    }
}
