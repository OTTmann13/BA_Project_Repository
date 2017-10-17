using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerTrain : MonoBehaviour {

	public GameObject train_placeholder;
	public TrainController moveTrain;

	// Use this for initialization
	void Start () {
		train_placeholder = GameObject.Find ("train_placeholder");
		moveTrain = train_placeholder.GetComponent<TrainController>();
		if(train_placeholder == null) {
			Debug.Log ("moveTrain is null");
		}
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	private void OnTriggerEnter (Collider other) {
        moveTrain.Trigger();
	}
		
}
