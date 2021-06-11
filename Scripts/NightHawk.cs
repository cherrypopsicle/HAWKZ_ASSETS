using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
//using RetroAesthetics;

public class NightHawk : MonoBehaviourPunCallbacks
{
    // glitch
    private bool glitching = false;
    private float glitchTime = 0.0f;
    private float rotateTime = 0.0f;
    private GameObject SteeringWheel, Sphere, ChoppaHolster, Choppa;

    [Header("Floats")]
    public float acceleration, brakeDeceleration, reverse, rotateAngle, steerLocation;
    [SerializeField] private float steerAngle, maxSteerAngle, speed, maxSpeed;

    [Header("Booleans")]
    public bool accelerateHawk = true;
    public bool brakeHawk = false;
    public bool rotateHawk = true;
    public bool reverseHawk = false;
    public bool steerHawk = false;

    [Header("Audiosources")]
    // AudioSources
    private AudioSource dropAudio, energyAudio, brakeAudio, revAudio, idleAudio, shotAudio;

    [Header("Slip values")]
    [SerializeField] private float[] floorSlipValues;
    [SerializeField] private float[] slipperySlipValues;
    [SerializeField] private float[] ruggedSlipValues;
    [SerializeField] private float[] stickToSurfaceSlipValues;
    [SerializeField] private Vector3 offsetPosition;

    // Steering 
    [SerializeField]
    private WheelCollider rearLeft, rearRight, frontLeft, frontRight;
    [SerializeField]
    private Transform rearLeftT, rearRightT, frontLeftT, frontRightT;

    private bool spawnChoppa = false;

    Transform sphereTransform;
    Transform normalTransform;
    Rigidbody sphereRb;

    // Dead code
    //[SerializeField] private LayerMask layer;
    //[SerializeField] private GameObject NightHawkNormal;
    //public GameObject Force;
    //public GameObject SidewaySlip;
    //public GameObject ForwardSlip;
    //public GameObject Velocity;

    void Start()
    {
        Sphere = transform.parent.gameObject;
        sphereTransform = Sphere.transform;
        SteeringWheel = GameObject.FindGameObjectWithTag("SteeringWheel");

        // Audio Source gameObjects
        idleAudio = GameObject.FindGameObjectWithTag("Idle").GetComponent<AudioSource>();
        revAudio = GameObject.FindGameObjectWithTag("Rev").GetComponent<AudioSource>();
        brakeAudio = GameObject.FindGameObjectWithTag("Brake").GetComponent<AudioSource>();
        dropAudio = GameObject.FindGameObjectWithTag("Drop").GetComponent<AudioSource>();
        energyAudio = GameObject.FindGameObjectWithTag("EnergyCapture").GetComponent<AudioSource>();
        shotAudio = GameObject.FindGameObjectWithTag("Shot").GetComponent<AudioSource>();
        ChoppaHolster = GameObject.FindGameObjectWithTag("Holster");
    }
    private void Update()
    {

    }

    private void FixedUpdate()
    {
        if (PhotonNetwork.IsMasterClient && !spawnChoppa)
        {
            Choppa = PhotonNetwork.InstantiateRoomObject("Choppa", ChoppaHolster.transform.position, ChoppaHolster.transform.rotation) as GameObject;
            spawnChoppa = true;
        }
        //FindNormal();
        //HawkTransform();
        //transform.position = sphereTransform.position - offsetPosition;
        //UpdateWheelPoses();
        //CalculateSteeringWheelRotation();

        // NEW SPHERICAL COLLIDER MECHANICS
        if (accelerateHawk == true)
        {
            AccelerateHawk();
        }
        else if (reverseHawk == true)
        {
            ReverseHawk();
        }
        else
        {
            DeccelerateHawk();
        }

        // WILL USE LATER
        //if (brakeHawk == true)
        //{
        //    BrakeHawk();
        //}

        // OLD WHEEL COLLIDER MECHANICS
        //if (steerHawk == true)
        //{
        //    SteerHawk();
        //}
        //else
        //{
        //    BackToRegular();
        //}

        //if (rotateHawk == true)
        //{
        //    RotateHawk();
        //}

        //if (glitching == true)
        //{
        //    Glitch();
        //}
    }



    void CalculateSteeringWheelRotation()
    {
        float zAngle = SteeringWheel.GetComponent<NewSteeringWheel>().wheelRotation;
        zAngle = (zAngle > 180) ? zAngle - 360 : zAngle;
        float finalAngle = Mathf.Floor(zAngle);
        var steer = -(-finalAngle / 180.0f);
        if (steer >= .02f || steer <= -.02f)
        {
            sphereTransform.RotateAround(sphereTransform.position, sphereTransform.up, finalAngle * Time.deltaTime);
            HawkTransform();
            //var steer = leftAxis;
            //steerLocation = steer;
            //steerHawk = true;
        }

        else
        {
            sphereRb.angularVelocity = Vector3.Lerp(sphereRb.angularVelocity, Vector3.zero, 2.0f);
        }

    }

    void HawkTransform()
    {
        var destination = new Vector3(0, sphereTransform.localEulerAngles.y, 0);
        transform.localEulerAngles = Vector3.Lerp(transform.localEulerAngles, destination, 5.0f);
    }

    // We remove the brake torques on all wheels then proceed to add motorTorque gradually
    void AccelerateHawk()
    {
        frontRight.brakeTorque = 0;
        frontLeft.brakeTorque = 0;
        rearLeft.brakeTorque = 0;
        rearRight.brakeTorque = 0;
        frontRight.motorTorque = speed;
        frontLeft.motorTorque = speed;
        rearLeft.motorTorque = speed;
        rearRight.motorTorque = speed;
        speed = speed + acceleration;
        speed = Mathf.Clamp(speed, 0.0f, maxSpeed);
        //sphereRb.AddForce(transform.forward * speed, ForceMode.Acceleration);
        //idleAudio.Stop();
        //if (!revAudio.isPlaying)
        //{
        //    revAudio.Play();
        //}
    }

    void ReverseHawk()
    {
        frontRight.brakeTorque = 0;
        frontLeft.brakeTorque = 0;
        rearLeft.brakeTorque = 0;
        rearRight.brakeTorque = 0;
        frontRight.motorTorque = speed;
        frontLeft.motorTorque = speed;
        rearLeft.motorTorque = speed;
        rearRight.motorTorque = speed;
        speed = speed - reverse;
        speed = Mathf.Clamp(speed, -maxSpeed, 0.0f);
        //idleAudio.Stop();
        //if (!revAudio.isPlaying)
        //{
        //    revAudio.Play();
        //}
    }

    // When the player lets go of the acceleration trigger, we add brake torque to all wheels to gradually slow down the ATV.
    // Since we don't remove the motorTorque, the car gradually slows down
    void DeccelerateHawk()
    {
        speed = speed - 1.0f;
        speed = Mathf.Clamp(speed, 0.0f, maxSpeed);
        frontRight.brakeTorque = 200;
        frontLeft.brakeTorque = 200;
        rearLeft.brakeTorque = 200;
        rearRight.brakeTorque = 200;
        //revAudio.Stop();
        //if (!idleAudio.isPlaying)
        //{
        //    idleAudio.Play();
        //}
    }
    // Add BrakeTorque to wheels when braking, but also deactivate the motor force. This way the car stops instantly.
    void BrakeHawk()
    {
        frontRight.brakeTorque = 2000;
        frontLeft.brakeTorque = 2000;
        rearLeft.brakeTorque = 2000;
        rearRight.brakeTorque = 2000;
        frontRight.motorTorque = 0;
        frontLeft.motorTorque = 0;
        rearLeft.motorTorque = 0;
        rearRight.motorTorque = 0;
        //if (!brakeAudio.isPlaying)
        //{
        //    brakeAudio.Play();
        //}
    }
    //void FindNormal()
    //{
    //    RaycastHit hit;
    //    Physics.Raycast(transform.position + (transform.up * .1f), Vector3.down, out hit, 2.0f, layer);

    //    normalTransform.up = Vector3.Lerp(NightHawkNormal.transform.up, hit.normal, 8.0f);
    //    normalTransform.transform.Rotate(0, transform.eulerAngles.y, 0);
    //}

    public void PlayEnergyCapture()
    {
        energyAudio.Play();
    }

    public void PlayShot()
    {
        shotAudio.Play();
    }
}


// DEAD CODE
//void MapPosition(Transform target, Transform rigTransform)
//{
//    target.position = rigTransform.position;
//    target.rotation = rigTransform.rotation;
//}

//void SteerHawk()
//{
//    steerAngle = steerLocation * maxSteerAngle;
//    frontLeft.steerAngle = steerAngle;
//    frontRight.steerAngle = steerAngle;
//}

//void BackToRegular()
//{
//    steerLocation = 0.0f;
//    frontLeft.steerAngle = 0.0f;
//    frontRight.steerAngle = 0.0f;
//}

//// Used for debugging purposes
//void RotateHawk()
//{
//    transform.RotateAround(transform.position, transform.up, Time.deltaTime * rotateAngle * 90f);
//}

//void UpdateWheelPoses()
//{
//    UpdateWheelPose(frontLeft, frontLeftT);
//    UpdateWheelPose(frontRight, frontRightT);
//    UpdateWheelPose(rearLeft, rearLeftT);
//    UpdateWheelPose(rearRight, rearRightT);
//}

//// Here, we attach the WheelCollider and WheelTransform. We first get the current position and quat from the wheel transform,
//// then we pass those attributes to be modified (in order to get current wheelCollider pos and quat), then we pass that back to
//// the wheelTransform. This in turn gives us the updated position and quaternion
//void UpdateWheelPose(WheelCollider wheelCollider, Transform wheelTransform)
//{
//    Vector3 _pos = wheelTransform.position;
//    Quaternion _quat = wheelTransform.rotation;

//    wheelCollider.GetWorldPose(out _pos, out _quat);
//    wheelTransform.position = _pos;
//    wheelTransform.rotation = _quat;

//    // DELETE LATER
//    //float zAngle = SteeringWheel.transform.localEulerAngles.z;
//    //zAngle = (zAngle > 180) ? zAngle - 360 : zAngle;
//    //float finalSteeringAngle = Mathf.Floor(zAngle);
//    //var steer = -finalSteeringAngle / 180.0f;
//    //Debug.Log(steer);
//    // TODO: BRING THIS CODE ON THE RIGHT BACK WHEN IT'S TIME. FWR: " + FrontLeftT.rotation.eulerAngles.z;
//    //WheelRotationUI.GetComponent<TextMeshProUGUI>().text = "SWR: " + finalSteeringAngle + " \nWSA: " + steerAngle + "\n Steer: " + steer; 
//    // We check if the wheel has touched the ground. If it did, we want to check what the type of terrain is. Depending on what the terrain is
//    // we apply different methods that modify the forward and sideway frictions accordingly.
//    WheelHit hit;
//    if (wheelCollider.GetGroundHit(out hit))
//    {
//        var force = hit.force;
//        if (force >= 6000.0f)
//        {
//            //controller.dropped = true;
//            //// TO-DO: Play only when it's a hard fall
//            //if (!Drop.isPlaying) Drop.Play();
//            GlitchOn();
//        }

//        var forwardSlip = hit.forwardSlip;
//        var sidewaySlip = hit.sidewaysSlip;

//        if (forwardSlip > 0.9f || sidewaySlip > 0.9f)
//        {
//            brakeAudio.Play();
//        }

//        //Force.GetComponent<Text>().text = "Force: " + force;
//        //ForwardSlip.GetComponent<Text>().text = "ForwardSlip: " + forwardSlip;
//        //SidewaySlip.GetComponent<Text>().text = "SidewaySlip: " + sidewaySlip;
//        //Velocity.GetComponent<Text>().text = "Velocity: " + velocity;

//        var terrain = hit.collider.gameObject.tag;
//        switch (terrain)
//        {
//            case "Floor":
//                FloorSlip(wheelCollider);
//                break;
//            case "SlipperyTerrain":
//                SlipperyTerrainSlip(wheelCollider);
//                break;
//            case "RuggedTerrain":
//                RuggedTerrain(wheelCollider);
//                break;
//            case "StickyTerrain":
//                StickyTerrain(wheelCollider);
//                break;
//        }
//    }
//    else
//    {
//        // do nothing
//    }
//}

//void GlitchOn()
//{
//    glitching = true;
//}

//// TODO: Import the RetroAesthetics namespace should you need to use it in the future. 
//void Glitch()
//{
//    this.glitchTime += Time.deltaTime;
//    if (this.glitchTime <= 0.5f)
//    {
//        // TODO: USE WITH POST PROCESSING
//        //Camera.GetComponent<RetroCameraEffect>().randomGlitches = RetroCameraEffect.GlitchDirections.Horizontal;
//        //Camera.GetComponent<RetroCameraEffect>().glitchIntensity = 2.5f;
//        //Camera.GetComponent<RetroCameraEffect>().glitchFrequency = 50;
//        //Camera.GetComponent<RetroCameraEffect>().bottomHeight = 0.5f;
//        //Camera.GetComponent<RetroCameraEffect>().displacementAmplitude = 5;
//        //Camera.GetComponent<RetroCameraEffect>().displacementFrequency = 150;
//        //Camera.GetComponent<RetroCameraEffect>().displacementSpeed = 5;
//        //Camera.GetComponent<RetroCameraEffect>().chromaticAberration = 50;
//    }
//    else
//    {
//        this.glitchTime = 0.0f;
//        this.glitching = false;
//        // TODO: USE WITH POST PROCESSING
//        //Camera.GetComponent<RetroCameraEffect>().randomGlitches = RetroCameraEffect.GlitchDirections.None;
//        //Camera.GetComponent<RetroCameraEffect>().glitchIntensity = 0.0f;
//        //Camera.GetComponent<RetroCameraEffect>().glitchFrequency = 0;
//        //Camera.GetComponent<RetroCameraEffect>().bottomHeight = 0.116f;
//        //Camera.GetComponent<RetroCameraEffect>().displacementAmplitude = 0;
//        //Camera.GetComponent<RetroCameraEffect>().displacementFrequency = 0;
//        //Camera.GetComponent<RetroCameraEffect>().displacementSpeed = 0;
//        //Camera.GetComponent<RetroCameraEffect>().chromaticAberration = 13.7f;
//    }

//}

//void SlipperyTerrainSlip(WheelCollider _collider)
//{
//    // Modifying main terrain to make it slippery forward and sideways
//    // 50 2 20 1 1
//    WheelFrictionCurve forwardFriction = _collider.forwardFriction;
//    forwardFriction.extremumSlip = slipperySlipValues[0];
//    forwardFriction.extremumValue = slipperySlipValues[1];
//    forwardFriction.asymptoteSlip = slipperySlipValues[2];
//    forwardFriction.asymptoteValue = slipperySlipValues[3];
//    forwardFriction.stiffness = slipperySlipValues[4];
//    _collider.forwardFriction = forwardFriction;

//    WheelFrictionCurve sidewaysFriction = _collider.sidewaysFriction;
//    sidewaysFriction.extremumSlip = slipperySlipValues[0];
//    sidewaysFriction.extremumValue = slipperySlipValues[1];
//    sidewaysFriction.asymptoteSlip = slipperySlipValues[2];
//    sidewaysFriction.asymptoteValue = slipperySlipValues[3];
//    sidewaysFriction.stiffness = slipperySlipValues[4];
//    _collider.sidewaysFriction = sidewaysFriction;
//}

//void FloorSlip(WheelCollider _collider)
//{
//    WheelFrictionCurve forwardFriction = _collider.forwardFriction;
//    forwardFriction.extremumSlip = floorSlipValues[0];
//    forwardFriction.extremumValue = floorSlipValues[1];
//    forwardFriction.asymptoteSlip = floorSlipValues[2];
//    forwardFriction.asymptoteValue = floorSlipValues[3];
//    forwardFriction.stiffness = floorSlipValues[4];
//    _collider.forwardFriction = forwardFriction;

//    WheelFrictionCurve sidewaysFriction = _collider.sidewaysFriction;
//    sidewaysFriction.extremumSlip = floorSlipValues[0];
//    sidewaysFriction.extremumValue = floorSlipValues[1];
//    sidewaysFriction.asymptoteSlip = floorSlipValues[2];
//    sidewaysFriction.asymptoteValue = floorSlipValues[3];
//    sidewaysFriction.stiffness = floorSlipValues[4];
//    _collider.sidewaysFriction = sidewaysFriction;
//}

//void RuggedTerrain(WheelCollider _collider)
//{
//    // Modifying main terrain to make it move forward and sideways with little slip
//    WheelFrictionCurve forwardFriction = _collider.forwardFriction;
//    forwardFriction.extremumSlip = 2.0f;
//    forwardFriction.extremumValue = 5.0f;
//    forwardFriction.asymptoteSlip = 5.0f;
//    forwardFriction.asymptoteValue = 2.0f;
//    forwardFriction.stiffness = 10.0f;
//    _collider.forwardFriction = forwardFriction;

//    WheelFrictionCurve sidewaysFriction = _collider.sidewaysFriction;
//    sidewaysFriction.extremumSlip = 2.0f;
//    sidewaysFriction.extremumValue = 5.0f;
//    sidewaysFriction.asymptoteSlip = 5.0f;
//    sidewaysFriction.asymptoteValue = 2.0f;
//    sidewaysFriction.stiffness = 10.0f;
//    _collider.sidewaysFriction = sidewaysFriction;
//}

//void StickyTerrain(WheelCollider _collider)
//{
//    // Modifying main terrain to make it move forward and sideways with little slip
//    WheelFrictionCurve forwardFriction = _collider.forwardFriction;
//    forwardFriction.extremumSlip = 0.1f;
//    forwardFriction.extremumValue = 5.0f;
//    forwardFriction.asymptoteSlip = 0.3f;
//    forwardFriction.asymptoteValue = 4.0f;
//    forwardFriction.stiffness = 1.0f;
//    _collider.forwardFriction = forwardFriction;

//    WheelFrictionCurve sidewaysFriction = _collider.sidewaysFriction;
//    sidewaysFriction.extremumSlip = 0.1f;
//    sidewaysFriction.extremumValue = 5.0f;
//    sidewaysFriction.asymptoteSlip = 0.3f;
//    sidewaysFriction.asymptoteValue = 4.0f;
//    sidewaysFriction.stiffness = 1.0f;
//    _collider.sidewaysFriction = sidewaysFriction;
//}