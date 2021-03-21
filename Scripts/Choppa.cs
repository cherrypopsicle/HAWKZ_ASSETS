using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;


public class Choppa : MonoBehaviourPunCallbacks
{
    // Serialized Fields
    // GameObjects
    [SerializeField] private GameObject Barrel, Bullet, ChoppaHolster, OriginalParent, LeftHand, RightHand, arrayOfPrefabs, Muzzle;

    // Boolean
    [SerializeField] public bool grabbed = false;
    [SerializeField] public bool shootingRight;
    [SerializeField] public bool shootingLeft;

    // Enum
    [SerializeField] public enum handGrab { LeftHand, RightHand, None };
    [SerializeField] public handGrab grab = handGrab.None;

    // TESTING VARIABLES
    [SerializeField] private bool grabbedTest = false;
    [SerializeField] private bool shootingTest = false;


    // Floats 
    [SerializeField] private float BulletForce;
    [SerializeField] private float perShotDelay = 0.3f;
    [SerializeField] private float timeToMove = 1.0f;
    [SerializeField] private float timeToDestroy = 10.0f;

    // private fields
    private Rigidbody rb;
    private float time = 0.0f;
    private float grabTime = 0.0f;
    private float timestamp = 0.0f;
    private ParticleSystem muzzleSystem;
    private AudioSource fire;

    // Start is called before the first frame update
    void Start()
    {
        arrayOfPrefabs = GameObject.FindGameObjectWithTag("ArrayOfPrefabs");
        rb = GetComponent<Rigidbody>();
        Barrel = GameObject.FindGameObjectWithTag("Barrel");
        Bullet = arrayOfPrefabs.GetComponent<ArrayOfPrefabs>().arrayOfPrefabs[0];
        ChoppaHolster = GameObject.FindGameObjectWithTag("Holster");
        LeftHand = GameObject.FindGameObjectWithTag("LeftControllerAnchor");
        RightHand = GameObject.FindGameObjectWithTag("RightControllerAnchor");
        Muzzle = GameObject.FindGameObjectWithTag("Muzzle");
        OriginalParent = transform.parent.gameObject;
        fire = GetComponent<AudioSource>();
        muzzleSystem = Muzzle.GetComponent<ParticleSystem>();
        var main = muzzleSystem.main;
        main.simulationSpeed = 2.0f;
    }

    // Update is called once per frame
    void FixedUpdate()
    {

    }
    private void Update()
    {
        if (shootingRight == true || shootingLeft == true || (shootingTest))
        {
            Shoot();
            Muzzle.SetActive(true);
        }

        else
        {
            Muzzle.SetActive(false);
        }

        if (transform.GetComponent<OVRGrabbable>().isGrabbed)
        {
            photonView.RequestOwnership();
            GameObject grabbedBy = transform.GetComponent<OVRGrabbable>().grabbedBy.gameObject;
            grabbed = true;
            rb.isKinematic = false;
            transform.parent = grabbedBy.transform;
            transform.position = grabbedBy.transform.position;
            if (grabbedBy.name == "CustomHandLeft")
            {
                GrabChoppa(LeftHand);
                grab = handGrab.LeftHand;
            }

            else if (grabbedBy.name == "CustomHandRight")
            {
                GrabChoppa(RightHand);
                grab = handGrab.RightHand;
            }
        }
        else
        {
            grabbed = false;
            rb.isKinematic = true;
            transform.parent = OriginalParent.transform;
            //FloatToHolster();
            grab = handGrab.None;
        }
    }

    void Shoot()
    {
        if (Time.time > timestamp)
        {
            fire.Play();
            timestamp = Time.time + perShotDelay;
            var temporaryBulletHandler = Instantiate(Bullet, Barrel.transform.position, Barrel.transform.rotation) as GameObject;
            if (temporaryBulletHandler != null)
            {
                var temporaryRigidBody = temporaryBulletHandler.GetComponent<Rigidbody>();
                temporaryRigidBody.AddForce(Barrel.transform.forward * BulletForce);
                Destroy(temporaryBulletHandler, timeToDestroy);
            }
        }
    }

    void FloatToHolster()
    {
        time += Time.deltaTime;
        float lerpTime = time / timeToMove;
        transform.position = Vector3.Lerp(transform.position, ChoppaHolster.transform.position, lerpTime);
        transform.rotation = Quaternion.Slerp(transform.rotation, ChoppaHolster.transform.rotation, lerpTime);
        if (time >= timeToMove)
        {
            time = 0.0f;
        }
    }

    // TODO: FIX LATER
    void GrabChoppa(GameObject hand)
    {
        float timeGrab = 1.0f;
        grabTime += Time.deltaTime;
        float lerpTime = grabTime / timeGrab;
        transform.rotation = Quaternion.Slerp(transform.rotation, hand.transform.rotation, lerpTime);
        if (grabTime >= timeGrab)
        {
            grabTime = 0.0f;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log(collision.gameObject.name);
    }
}

