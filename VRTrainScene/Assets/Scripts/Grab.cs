using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grab : MonoBehaviour {

    public OVRInput.Controller controller;
    public string buttonName;
    public float grabRadius;
    public LayerMask grabMask;
    public LayerMask snapPointMask;
    public LayerMask rotateMask;

    private GameObject grabbedObject;
    private Transform grabbedObjectParent;
    private GameObject rotateObject;
    private bool grabbed;
    private bool grabbedSnapPoint;

    private GameObject car;
    private CarController carContoller;

    //World rotation
    private Vector3 previousPosition;
    private float rotationSpeed = 10;

    private void Start()
    {
        car = GameObject.Find("Car");
        carContoller = car.GetComponent<CarController>();
    }

    void Update()
    {
        if (!grabbedSnapPoint) {
            transform.localPosition = OVRInput.GetLocalControllerPosition(controller);
            transform.localRotation = OVRInput.GetLocalControllerRotation(controller);
        }

        if (!grabbed && Input.GetAxis(buttonName) == 1) { GrabObject(); }
        if (grabbed && Input.GetAxis(buttonName) < 1) { DropObject(); }

        if (rotateObject) { RotateObject(); }


        //turn the wheel when moving the controller 
        /*if (grabbedObject.transform.name == "SnapPointRight")
        {
            handleWheelTurn();
        }*/
    }

    void GrabObject()
    {
        //Remember Position of the Hand
        previousPosition = transform.position;

        grabbed = true;

        RaycastHit[] grabableObjects;
        RaycastHit[] rotateableObjects;
        RaycastHit[] snapPoints;


        //pick up objects
        grabableObjects = Physics.SphereCastAll(transform.position, grabRadius, transform.forward, 0f, grabMask);

        if (grabableObjects.Length > 0)
        {
            int closestHit = 0;

            for (int i = 0; i < grabableObjects.Length; i++)
            {
                if (grabableObjects[i].distance < grabableObjects[closestHit].distance)
                {
                    closestHit = i;
                }
            }

            grabbedObject = grabableObjects[closestHit].transform.gameObject;
            grabbedObject.GetComponent<Rigidbody>().isKinematic = true;
            grabbedObject.transform.position = transform.position;

            //Save the parent of the grabbed gameobject to restore it later
            grabbedObjectParent = grabbedObject.transform.parent;

            grabbedObject.transform.parent = transform;

            if (grabbedObject.name == car.name) carContoller.SetCarState(CarController.CarStates.Hold);
        }

        //Rotate Objects
        rotateableObjects = Physics.SphereCastAll(transform.position, grabRadius, transform.forward, 0f, rotateMask);

        if(rotateableObjects.Length > 0)
        {
            int closestHit = 0;
            for(int i = 0; i < rotateableObjects.Length; i++)
            {
                if (rotateableObjects[i].distance < rotateableObjects[closestHit].distance) {
                    closestHit = i;
                }
            }

            rotateObject = rotateableObjects[closestHit].transform.gameObject;
        }

        //snap points
        snapPoints = Physics.SphereCastAll(transform.position, grabRadius, transform.forward, 0f, snapPointMask);
        if (snapPoints.Length > 0)
        {
            grabbedSnapPoint = true;
            int closestHit = 0;
            for (int i = 0; i < snapPoints.Length; i++)
            {
                if (snapPoints[i].distance < snapPoints[closestHit].distance)
                {
                    closestHit = i;
                }
            }

            grabbedObject = snapPoints[closestHit].transform.gameObject;
            grabbedObject.GetComponent<Rigidbody>().isKinematic = true;
            //grabbedObject.transform.position = transform.position;
            //grabbedObject.transform.parent = transform;
            transform.position = grabbedObject.transform.position;
            transform.rotation = grabbedObject.transform.rotation;

            Debug.Log(grabbedObject.transform.name);
            if (grabbedObject.transform.name == "SnapPointRight")
            {
            handleWheelTurn();
            }
        }

    }

    void RotateObject() {


        //Manage the rotation of the grabbed Obeject
        //TODO Only the world should be rotateble the car should be moveable
        //Create a rotation from the hand movment in x axsis
        float yRotation = (transform.position.x - previousPosition.x) * -rotationSpeed;
        rotateObject.transform.Rotate(Vector3.up * yRotation, Space.Self);

        //Remember the hand position in order to track movment
        previousPosition = transform.position;
    }

    void DropObject()
    {
        grabbed = false;
        if (grabbedObject != null)
        {
            grabbedObject.transform.parent = grabbedObjectParent;

            //Only set grabbed obejcts kinematic to true if its not a snapPoint
            if(!grabbedSnapPoint)
            {
                grabbedObject.GetComponent<Rigidbody>().isKinematic = false;
            }

            grabbedSnapPoint = false;
            grabbedObject.GetComponent<Rigidbody>().velocity = OVRInput.GetLocalControllerVelocity(controller);
            grabbedObject.GetComponent<Rigidbody>().angularVelocity = OVRInput.GetLocalControllerAngularVelocity(controller);
            Debug.Log("Dropped Object: " + grabbedObject.name);
            grabbedObject = null;
            //transform.parent = null;

            //transform.localPosition = OVRInput.GetLocalControllerPosition(controller);
            //transform.localRotation = OVRInput.GetLocalControllerRotation(controller);
        }

        if (rotateObject != null)
        {
            rotateObject = null;
        }
    }

    void handleWheelTurn()
    {
        grabbedObject.transform.parent.rotation = Quaternion.AngleAxis(OVRInput.GetLocalControllerPosition(controller).y * 360, Vector3.forward);
    }
}
