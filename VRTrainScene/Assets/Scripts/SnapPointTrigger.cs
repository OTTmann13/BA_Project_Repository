/*using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnapPointTrigger : MonoBehaviour {

    private WheelController parent;

	// Use this for initialization
	void Start () {
        parent = transform.parent.GetComponent<WheelController>();
	}

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("triggerEnter");
        transform.gameObject.SetActive(true);
    }

    private void OnTriggerExit(Collider other)
    {
        transform.gameObject.SetActive(false);
    }
}
*/