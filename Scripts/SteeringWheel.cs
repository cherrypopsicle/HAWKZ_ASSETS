using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SteeringWheel : MonoBehaviour
{
    [SerializeField] private bool leftHandPlaced = false;
    [SerializeField] private bool rightHandPlaced = false;
    [SerializeField] private GameObject leftHand;
    [SerializeField] private GameObject rightHand;
    [SerializeField] private float maxDistance;

    // accessed by NightHawk
    public float wheelRotation;
    // Start is called before the first frame update
    void Start()
    {
        wheelRotation = transform.localEulerAngles.z;
        //IgnoreCollisions(leftHand, rightHand);
    }

    // Update is called once per frame
    void Update()
    {
        CheckDistanceToHands(leftHand, rightHand);
        var rightTrigger = OVRInput.Get(OVRInput.Button.PrimaryHandTrigger, OVRInput.Controller.RTouch);
        var leftTrigger = OVRInput.Get(OVRInput.Button.PrimaryHandTrigger, OVRInput.Controller.LTouch);
        if (leftHandPlaced == true && leftTrigger)
        {
            Vector3 dest = new Vector3(transform.localEulerAngles.x, transform.localEulerAngles.y, -(leftHand.transform.rotation.eulerAngles.z));
            transform.localEulerAngles = Vector3.Lerp(transform.localEulerAngles, dest, 100.0f);
            wheelRotation = transform.localEulerAngles.z;

        }
        else if (rightHandPlaced == true && rightTrigger)
        {
            Vector3 dest = new Vector3(transform.localEulerAngles.x, transform.localEulerAngles.y, -(rightHand.transform.rotation.eulerAngles.z));
            transform.localEulerAngles = Vector3.Lerp(transform.localEulerAngles, dest, 100.0f);
            wheelRotation = transform.localEulerAngles.z;
        }

        else if (rightHandPlaced == true && leftHandPlaced == true && (leftTrigger && rightTrigger))
        {
            var directionOne = new Vector3(transform.localEulerAngles.x, transform.localEulerAngles.y, -(rightHand.transform.rotation.eulerAngles.z));
            var directionTwo = new Vector3(transform.localEulerAngles.x, transform.localEulerAngles.y, -(leftHand.transform.rotation.eulerAngles.z));
            var midway = directionOne.normalized + directionTwo.normalized;
            midway = midway.normalized;
            transform.localEulerAngles = Vector3.Lerp(transform.localEulerAngles, midway, 2.0f);
            wheelRotation = transform.localEulerAngles.z;
        }

        else
        {
            transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, transform.localEulerAngles.y, 0.0f);
            wheelRotation = transform.localEulerAngles.z;
        }
    }

    void CheckDistanceToHands(GameObject leftHand, GameObject rightHand)
    {
        if (Vector3.Distance(leftHand.transform.position, transform.position) <= maxDistance)
        {
            leftHandPlaced = true;
        }

        else
        {
            leftHandPlaced = false;
        }

        if (Vector3.Distance(rightHand.transform.position, transform.position) <= maxDistance)
        {
            rightHandPlaced = true;
        }

        else
        {
            rightHandPlaced = false;
        }
    }
}
