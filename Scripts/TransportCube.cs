using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransportCube : MonoBehaviour {

    [SerializeField] private Transform startLocation;
    [SerializeField] private Transform endLocation;
    [SerializeField] private bool enteredCube = true;
    [SerializeField] private bool pulledLever = true;
    [SerializeField] private bool atStart;
    [SerializeField] private bool calledTransport = false;
    [SerializeField] private float timeToMove = 30.0f;
    [SerializeField] private GameObject player;
    [SerializeField] private GameObject transportCollider;
    [SerializeField] private GameObject transportObject;
    [SerializeField] private GameObject localScale;
    private Rigidbody playerRigidbody;
    private float distanceLength;
    private float startTime = 0.0f;

    void Awake()
    {
        // Check whether or not cube is at the beginning or not
        //if (transform.position == startLocation.position) atStart = true;
        //else { atStart = false; transform.position = endLocation.position; }
    }

    void Start()
    {
        playerRigidbody = player.GetComponent<Rigidbody>();
        // Calculate length of journey
        if (atStart) distanceLength = Vector3.Distance(startLocation.position, endLocation.position);
        else distanceLength = Vector3.Distance(endLocation.position, startLocation.position);
    }
    
    void Update()
    {
        if (enteredCube && pulledLever && !atStart) Move(endLocation, startLocation, atStart);
        else if (enteredCube && pulledLever && atStart) Move(startLocation, endLocation, atStart);

        if (calledTransport && !atStart) CallTransport(endLocation, startLocation, atStart);
        else if (calledTransport && atStart) CallTransport(startLocation, endLocation, atStart);

    }

    void Move(Transform start, Transform end, bool isAtStart)
    {
        startTime += Time.deltaTime;
        float lerpTime = startTime / timeToMove;
        transportObject.transform.position = Vector3.Lerp(start.transform.position, end.transform.position, lerpTime);
        transportObject.transform.rotation = Quaternion.Slerp(transportObject.transform.rotation, end.transform.rotation, lerpTime);
        player.transform.SetParent(transportObject.transform, true);
        playerRigidbody.constraints = RigidbodyConstraints.FreezeAll;
        if (lerpTime >= 1.0f && isAtStart == false)
        {
            pulledLever = false;
            player.transform.SetParent(null);
            playerRigidbody.constraints = RigidbodyConstraints.None;
            atStart = true;
            startTime = 0.0f;
        }
        else if (lerpTime >= 1.0f && isAtStart == true)
        {
            pulledLever = false;
            player.transform.SetParent(null);
            playerRigidbody.constraints = RigidbodyConstraints.None;
            atStart = false;
            startTime = 0.0f;
        }
    }

    void CallTransport(Transform start, Transform end, bool isAtStart)
    {
        startTime += Time.deltaTime;
        float lerpTime = startTime / timeToMove;
        transportObject.transform.position = Vector3.Lerp(start.transform.position, end.transform.position, lerpTime);
        transportObject.transform.rotation = Quaternion.Slerp(transportObject.transform.rotation, end.transform.rotation, lerpTime);
        if (lerpTime >= 1.0f && isAtStart == false)
        {
            calledTransport = false;
            atStart = true;
            startTime = 0.0f;
        }
        else if (lerpTime >= 1.0f && isAtStart == true)
        {
            calledTransport = false;    
            atStart = false;
            startTime = 0.0f;
        }
    }

    public void ButtonDown()
    {
        //this.pulledLever = true;
    }

    public void ButtonUp()
    {
        if (pulledLever == false)
        {
            Debug.Log("Pulled lever");
            pulledLever = true;
            Debug.Log(pulledLever);
        }
    }

    public void CallTeleport()
    {
        if (calledTransport == false) calledTransport = true;
    }

    void OnTriggerEnter(Collider collider)
    {
        if (collider.gameObject.tag == "Motorcycle") {
            Debug.Log("Entered collider");
            enteredCube = true;
        }
    }

    void OnTriggerExit(Collider collider)
    {
        if (collider.gameObject.tag == "Motorcycle")
        {
            Debug.Log("Exited collider");
            enteredCube = false;
        }
    }
}
