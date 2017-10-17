using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerController : MonoBehaviour {

    private GameObject trainObject;
    private TrainController trainController;
    private GameObject barrierObject1;
    private GameObject barrierObject2;
    private BarrierController barrierController;
    private FadeController fadeController;
    private GameObject fadeCube;

    private bool carInDanger = false;

    private string trainName = "Train";
    private string carName = "Car";
    private string barrierName1 = "Barrier1";
    private string barrierName2 = "Barrier2";

    private float barrierCloseDelay = 5.0f;
    private float trainStartDelay = 10.0f;

    private Animator animBarrier1;
    private Animator animBarrier2;
    private string barrierCloseTriggerName = "isClosing";

    void Start () {
        trainObject = GameObject.Find(trainName);
        trainController = trainObject.GetComponent<TrainController>();

        //Get the animators of the barriers
        barrierObject1 = GameObject.Find(barrierName1);
        animBarrier1 = barrierObject1.GetComponent<Animator>();
        barrierController = barrierObject1.GetComponentInParent<BarrierController>();

        barrierObject2 = GameObject.Find(barrierName2);
        animBarrier2 = barrierObject2.GetComponent<Animator>();

        fadeCube = GameObject.Find("FadeCube");
        fadeController = fadeCube.GetComponent<FadeController>();

        //anim.SetTrigger("isClosing");
    }

    private void Update() {


    }

    //Routine to trigger the car stall, close barriers and start train
    IEnumerator StallCarRoutine()
    {
        yield return new WaitForSeconds(barrierCloseDelay);

        StartCoroutine(barrierController.PlaySoundForSec(6.5f));

        animBarrier1.SetTrigger(barrierCloseTriggerName);
        animBarrier2.SetTrigger(barrierCloseTriggerName);

        

        yield return new WaitForSeconds(trainStartDelay);
        //Trigger the train to start movement
        trainController.Trigger();
    }

    public void OnTriggerEnter(Collider enteredObject) {

        //Car hitting the trigger
        if (enteredObject.name == carName) {
            GameObject carObject = enteredObject.gameObject;
            CarController carController = carObject.GetComponent<CarController>();

            //Set the state of the car to danger
            carController.SetCarState(CarController.CarStates.InDanger);

            StartCoroutine(StallCarRoutine());

            carInDanger = true;

            //Make the car grabable
            carObject.layer = 8;
        }

        //Train hitting the trigger
        if(enteredObject.name == trainName) { 
            if(carInDanger) {
                //Start the fade out when the train hits the trigger
                fadeController.SetFadeActive();
            } else {
                
            }
        }
    }

    public void OnTriggerExit(Collider exitObject) {
        //Car exiting the trigger
        if(exitObject.name == carName) {
            carInDanger = false;

            //Maybe stop dramatic music
            //Start Win Countdown or something
        }
    }
}
