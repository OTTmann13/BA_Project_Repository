using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarController : MonoBehaviour {

	//Wheels
	public List<AxleInfo> axleInfos;
	public float maxMotorTorque;
	public float maxSteeringAngle;
	public float maxBrakeTorque;
    public bool autoDrive = false;
	public GameObject[] visualWheels = new GameObject[4];

    public enum CarStates
    {
        Drive,
        InDanger,
        Hold
    }

    CarStates carCurrentState = CarStates.Drive;

    private Quaternion wheelStartRotationFL;
    private Quaternion wheelStartRotationFR;
    private Quaternion wheelStartRotationBL;
    private Quaternion wheelStartRotationBR;

    public float motor;
    public float brake;
    private float steering;

    //Steering Wheel
    public GameObject steeringWheel;
	private Quaternion steeringWheelRotation;
	private float visualSteeringWheelFactor = 3;

    //Autodrive
    public GameObject[] waypoints;
    GameObject currentWaypoint;
    private int wayPointIndex = 0;
    private float autoDriveMotorTorque = 100.0f;

    //Audio clips
    public AudioClip driveClip;
    public AudioClip stallClip;
    public AudioClip motorStartClip;
    private AudioSource audioSource;

    public void Start() {
		steeringWheelRotation = steeringWheel.transform.localRotation;

        wheelStartRotationFL = visualWheels[0].transform.rotation;
        wheelStartRotationFR = visualWheels[1].transform.rotation;
        wheelStartRotationBL = visualWheels[2].transform.rotation;
        wheelStartRotationBR = visualWheels[3].transform.rotation;

        audioSource = GetComponent<AudioSource>();

        SetCarState(CarStates.Drive);
    }

	//Fixed Update is used for rigidbody components and is called every fixed framerate frame
	public void FixedUpdate() {
        if (!autoDrive)
        {
            //Get Vertical Axis (Keyboard keys W and S or up and down arrow) Value between 1 and -1
            motor = maxMotorTorque * Mathf.Clamp(Input.GetAxis("Vertical"), 0, 1);
            brake = maxBrakeTorque * Mathf.Clamp((Input.GetAxis("Vertical") * -1), 0, 1);
            //Get Horizontal Axis (Keyboard keys A and D or left and right arrow) Value between 1 and -1
            steering = maxSteeringAngle * Input.GetAxis("Horizontal");
        }

		ApplyVisualsToSteeringWheel (steering);
		//Set the steering, motor and breake torque for each responsible wheel 
		foreach (AxleInfo axleInfo in axleInfos) {
			if (axleInfo.steering) {
				axleInfo.leftWheel.steerAngle = steering;
				axleInfo.rightWheel.steerAngle = steering;
			}
			if (axleInfo.motor) {
				axleInfo.leftWheel.motorTorque = motor;
				axleInfo.rightWheel.motorTorque = motor;
			}
			if (axleInfo.breaking) {
				axleInfo.leftWheel.brakeTorque = brake;
				axleInfo.rightWheel.brakeTorque = brake;
			}
			if (axleInfo.front) {
				ApplyLocalPositionToVisuals (axleInfo.leftWheel, visualWheels[0], wheelStartRotationFL);
				ApplyLocalPositionToVisuals (axleInfo.rightWheel, visualWheels[1], wheelStartRotationFR);
			}
			if (axleInfo.back) {
				ApplyLocalPositionToVisuals (axleInfo.leftWheel, visualWheels[2], wheelStartRotationBL);
				ApplyLocalPositionToVisuals (axleInfo.rightWheel, visualWheels[3], wheelStartRotationBR);
			}
		}
        if (autoDrive) AutoDrive();

        //HandleSounds();
	}

    void AutoDrive()
    {
        if (carCurrentState == CarStates.Drive)
        {
            currentWaypoint = waypoints[wayPointIndex];

            //Set waypoint at the height of the car to get a precise angle on the x axis
            currentWaypoint.transform.position = new Vector3(currentWaypoint.transform.position.x, transform.position.y, currentWaypoint.transform.position.z);

            //Vector heading to the target from the car
            Vector3 heading = currentWaypoint.transform.position - transform.position;

            //Distance to the target in float
            float distance = heading.magnitude;

            //TODO Auf korrektheit Prüfen
            Vector3 referenceForward = transform.forward;
            Vector3 referenceRight = Vector3.Cross(Vector3.up, referenceForward);

            //Angle between forward vector and the vector from car to target (0..180 degree in float)
            float targetDirection = Vector3.Angle(heading, referenceForward);


            float sign = Mathf.Sign(Vector3.Dot(heading, referenceRight));
            float targetAngle = sign * targetDirection;

            steering = Mathf.Clamp(targetAngle, -maxSteeringAngle, maxSteeringAngle);

            /*float step = turnSpeed * Time.deltaTime;
            Vector3 newDir = Vector3.RotateTowards(transform.forward, heading, step, 0.0f);
            Debug.DrawRay(transform.position, (newDir + Vector3.up) * 10, Color.green);

            transform.rotation = Quaternion.LookRotation(newDir);*/

            if (distance < 3.0f && wayPointIndex < waypoints.Length - 1)
            {
                wayPointIndex++;
            }

            //Get the distance to the last waypoint in order to stop the vehicle before it over shots the waypoint
            Vector3 vectorToLastWaypoint = waypoints[waypoints.Length - 1].transform.position - transform.position;
            float distanceToLastWaypoint = vectorToLastWaypoint.magnitude;

            if (distanceToLastWaypoint < 10.0f)
            {
                brake = 1500;
            }
            else
            {
                motor = autoDriveMotorTorque;
            }
        }
    }

	public void ApplyLocalPositionToVisuals(WheelCollider wheelcollider, GameObject visualWheel, Quaternion startRotation) {

		Vector3 position;
		Quaternion rotation;
		//Get so position an rotation of the collider
		wheelcollider.GetWorldPose (out position, out rotation);

		//Set the visual wheel object to the position an rotation of the collider
		visualWheel.transform.position = position;
		visualWheel.transform.rotation = rotation;
	}

	//Apply a visual rotation to the steering wheel according to the wheel steering. 
	public void ApplyVisualsToSteeringWheel(float steering) {
		steeringWheel.transform.localRotation = steeringWheelRotation * Quaternion.AngleAxis((steering * visualSteeringWheelFactor), Vector3.up);
	}

	//Serializable is shown in the inspector
	[System.Serializable]
	//Axle info contains the wheel collider for the axle and which function the wheel is suposed to do
	public class AxleInfo {
		public WheelCollider leftWheel;
		public WheelCollider rightWheel;
		public bool front;
		public bool back;
		public bool motor;
		public bool steering;
		public bool breaking;
	}

    private void HandleSounds() {

        switch(carCurrentState)
        {
            case CarStates.Drive:
                //Play drive sounds
                if (driveClip != null) {
                    if (audioSource.clip != driveClip) {
                        audioSource.clip = driveClip;
                        audioSource.Play();
                    }
                }
                else {
                    Debug.Log("AudioClip: DriveClip missing!");
                }
                break;
            case CarStates.Hold:
                //Play no Sound
                Debug.Log("Stop Plaing sound");
                audioSource.Stop();
                break;
            case CarStates.InDanger:
                //Play stall and start sounds
                if (stallClip || motorStartClip != null) {
                    if (audioSource.clip != stallClip) {
                        audioSource.clip = stallClip;
                        audioSource.Play();
                    }

                    if (audioSource.clip != motorStartClip) {
                        audioSource.clip = motorStartClip;
                        audioSource.PlayDelayed(stallClip.length);
                    }
                }
                break;
        }
    }

    public CarStates GetCarState() {
        return carCurrentState;
    }

    public void SetCarState(CarStates newCarState) {
        carCurrentState = newCarState;
        HandleSounds();
    }
}