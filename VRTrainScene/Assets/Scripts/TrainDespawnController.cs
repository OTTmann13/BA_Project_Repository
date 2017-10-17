using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrainDespawnController : MonoBehaviour {

    private GameObject train;

    private GameObject fadeCube;
    private FadeController fadeController;

    public void Start()
    {
        train = GameObject.Find("Train");

        fadeCube = GameObject.Find("FadeCube");
        fadeController = fadeCube.GetComponent<FadeController>();
    }

    private void OnTriggerEnter(Collider other) {
        Debug.Log(other.name);
        if (other.name == train.name) fadeController.SetFadeActive();
    }
}
