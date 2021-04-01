using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewSteeringWheel : MonoBehaviour
{
    // From Mr.Pineapple studio. Thank you!
    [SerializeField] private float maxDistance = 0.2f;
    // LeftHand
    [SerializeField] private GameObject leftHand;
    [SerializeField] private bool leftHandOnWheel = false;
    [SerializeField] private Transform leftHandOriginalParent;

    // RightHand
    [SerializeField] private GameObject rightHand;
    [SerializeField] private bool rightHandOnWheel = false;
    [SerializeField] private Transform rightHandOriginalParent;

    // Positions the hand can snap on the wheel
    [SerializeField] private Transform[] snapPositions;

    private int numberOfHandsOnWheel = 0;
    [SerializeField] private float currentWheelRotation = 0;
    private float turnDampening = 250;
    [SerializeField] private Transform directionalObject;

    [SerializeField] private Transform originalParent;

    // accessed by NightHawk
    public float wheelRotation;

    void Start()
    {
        originalParent = transform.parent;
        leftHandOriginalParent = leftHand.transform.parent;
        rightHandOriginalParent = rightHand.transform.parent;
    }

    // Update is called once per frame
    void Update()
    {
        CheckDistanceToHands(leftHand, rightHand);
        ReleaseHandsFromWheel();

        ConvertHandRotationToSteeringWheelRotation();

        currentWheelRotation = -transform.rotation.eulerAngles.z;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("RightHand"))
        {
            var rightTrigger = OVRInput.Get(OVRInput.Button.PrimaryHandTrigger, OVRInput.Controller.RTouch);
            if (rightHandOnWheel == false && rightTrigger)
            {
                //PlaceHandOnWheel(ref rightHand, ref rightHandOriginalParent, ref rightHandOnWheel);
            }
        }

        else if (other.CompareTag("LeftHand"))
        {
            var leftTrigger = OVRInput.Get(OVRInput.Button.PrimaryHandTrigger, OVRInput.Controller.LTouch);
            if (leftHandOnWheel == false && leftTrigger)
            {
                //PlaceHandOnWheel(ref leftHand, ref leftHandOriginalParent, ref leftHandOnWheel);
            }
        }
    }

    void CheckDistanceToHands(GameObject leftHand, GameObject rightHand)
    {
        if (Vector3.Distance(leftHandOriginalParent.transform.position, transform.position) <= maxDistance)
        {
            leftHandOnWheel = true;
        }

        else
        {
            leftHandOnWheel = false;
        }

        if (Vector3.Distance(rightHandOriginalParent.transform.position, transform.position) <= maxDistance)
        {
            rightHandOnWheel = true;
        }

        else
        {
            rightHandOnWheel = false;
        }
    }

    private void ConvertHandRotationToSteeringWheelRotation()
    {
        var leftTrigger = OVRInput.Get(OVRInput.Button.PrimaryHandTrigger, OVRInput.Controller.LTouch);
        var rightTrigger = OVRInput.Get(OVRInput.Button.PrimaryHandTrigger, OVRInput.Controller.RTouch);
        
        if ((rightHandOnWheel && rightTrigger) && !leftHandOnWheel)
        {
            rightHand.transform.parent = transform.parent;
            Quaternion newRot = Quaternion.Euler(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y, -(rightHandOriginalParent.transform.rotation.eulerAngles.z));
            directionalObject.rotation = newRot;
            transform.parent = directionalObject;
            wheelRotation = directionalObject.localEulerAngles.z;
        }
        else if ((leftHandOnWheel && leftTrigger) && !rightHandOnWheel)
        {
            leftHand.transform.parent = transform.parent;
            Quaternion newRot = Quaternion.Euler(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y, -(leftHandOriginalParent.transform.rotation.eulerAngles.z));
            directionalObject.rotation = newRot;
            transform.parent = directionalObject;
            wheelRotation = directionalObject.localEulerAngles.z;
        }

        //else if ((rightHandOnWheel && rightTrigger) && (leftTrigger && leftHandOnWheel))
        //{
        //    leftHandOriginalParent = leftHand.transform.parent;
        //    leftHand.transform.parent = transform.parent;
        //    rightHandOriginalParent = rightHand.transform.parent;
        //    rightHand.transform.parent = transform.parent;
        //    Quaternion newRotLeft = Quaternion.Euler(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y, leftHandOriginalParent.transform.rotation.eulerAngles.z);
        //    Quaternion newRotRight = Quaternion.Euler(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y, rightHandOriginalParent.transform.rotation.eulerAngles.z);
        //    Quaternion finalRot = Quaternion.Slerp(newRotLeft, newRotRight, 0.25f);
        //    directionalObject.rotation = finalRot;
        //    transform.parent = directionalObject;
        //    wheelRotation = directionalObject.localEulerAngles.z;
        //}
    }

    private void ReleaseHandsFromWheel()
    {
        if (!rightHandOnWheel)
        {
            rightHand.transform.parent = rightHandOriginalParent;
            rightHand.transform.position = rightHandOriginalParent.position;
            rightHand.transform.localEulerAngles = new Vector3(0, 0, -90);
            numberOfHandsOnWheel--;
            wheelRotation = directionalObject.localEulerAngles.z;
        }

        if (!leftHandOnWheel)
        {
            leftHand.transform.parent = leftHandOriginalParent;
            leftHand.transform.position = leftHandOriginalParent.position;
            leftHand.transform.localEulerAngles = new Vector3(0, 0, 90);
            numberOfHandsOnWheel--;
            wheelRotation = directionalObject.localEulerAngles.z;
        }

        if (!leftHandOnWheel && !rightHandOnWheel)
        {
            leftHand.transform.parent = leftHandOriginalParent;
            leftHand.transform.position = leftHandOriginalParent.position;
            leftHand.transform.localEulerAngles = new Vector3(0, 0, 90); 
            rightHand.transform.parent = rightHandOriginalParent;
            rightHand.transform.position = rightHandOriginalParent.position;
            rightHand.transform.localEulerAngles = new Vector3(0, 0, -90);
            transform.parent = originalParent;
            transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y, 0.0f);
            wheelRotation = 0.0f;
        }
    }
}
