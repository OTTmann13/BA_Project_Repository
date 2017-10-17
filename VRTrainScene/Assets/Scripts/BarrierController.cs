using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarrierController : MonoBehaviour {

    private AudioSource audioSource;

	// Use this for initialization
	void Start () {
        audioSource = GetComponent<AudioSource>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public IEnumerator PlaySoundForSec(float seconds) {
        audioSource.Play();
        yield return new WaitForSeconds(seconds);
        audioSource.Stop();
    }
}
