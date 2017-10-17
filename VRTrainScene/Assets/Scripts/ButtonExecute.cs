using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class ButtonExecute : MonoBehaviour {
    public float timeToSelect = 2.0f;
    private float countdown;
    private GameObject currentButton;
    public GameObject curser;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        
        Transform camera = Camera.main.transform;
        Ray ray = new Ray(camera.transform.position, camera.rotation * Vector3.forward);
        RaycastHit hit;
        GameObject hitButton = null;
        PointerEventData data = new PointerEventData(EventSystem.current);
        curser.transform.position = ray.GetPoint(25.0f);
        if (Physics.Raycast (ray, out hit))
        {
            if(hit.transform.gameObject.tag == "Button")
            {
                hitButton = hit.transform.parent.gameObject;
            }
        }
        if (currentButton != hitButton) {
            if (currentButton != null) {
                ExecuteEvents.Execute<IPointerExitHandler>(currentButton, data, ExecuteEvents.pointerExitHandler);
            }
            currentButton = hitButton;
            if(currentButton != null)
            {
                ExecuteEvents.Execute<IPointerEnterHandler>(currentButton, data, ExecuteEvents.pointerEnterHandler);
                countdown = timeToSelect;
            }            
        }
        if (currentButton != null)
        {
            countdown -= Time.deltaTime;
            if (countdown <= 0.0f)
            {
                ExecuteEvents.Execute<IPointerClickHandler>(currentButton, data, ExecuteEvents.pointerClickHandler);
                countdown = timeToSelect;
            }
        }
    }

    //Move to GameController
    public void LoadLevel(string name) {   
        SceneManager.LoadScene(name);
    }
}
