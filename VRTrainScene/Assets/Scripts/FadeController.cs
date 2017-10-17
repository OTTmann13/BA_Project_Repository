using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeController : MonoBehaviour {

    public Material fadeCube;

    private GameController gameController;
    private GameObject gameControllerObject;

    private float fadeTime = 0.5f;
    private Color currentColor = Color.black;
    private bool fadeActive;
    private float currentFadeTime;
    private float changeSceneDelay = 1.0f;

    // Use this for initialization
    void Start () {
        gameControllerObject = GameObject.Find("GameController");
        gameController = gameControllerObject.GetComponent<GameController>();

        currentColor.a = 0;
        fadeCube.color = currentColor;
    }
	
	// Update is called once per frame
	void Update () {
        if (fadeActive)
        {
            currentFadeTime += Time.deltaTime;
            float alpha = Time.deltaTime / fadeTime;
            currentColor.a += alpha;

            fadeCube.color = currentColor;

            //Change level to Menu when after the screens fades black + delay
            if (currentFadeTime > fadeTime + changeSceneDelay)
            {
                gameController.LoadLevel("Menu");
            }
        }
        else
        {
            //Fade back of the car gets rescued the last second
        }
    }

    public void SetFadeActive() {
        fadeActive = true;
    }
}
