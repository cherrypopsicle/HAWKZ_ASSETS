using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class MainSphereCollider : MonoBehaviourPunCallbacks
{
    Rigidbody rb;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "BouncyWall")
        {
            rb.angularVelocity = Vector3.zero;
            var forceDirection = collision.gameObject.transform.right;
            rb.AddForce(forceDirection * 1000.0f, ForceMode.Impulse);
        }
    }
}
