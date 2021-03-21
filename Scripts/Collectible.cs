using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collectible : MonoBehaviour 
{
    [SerializeField] private GameObject GameManager;
    [SerializeField] private NightHawk hawk;
    // Use this for initialization
    void Start () { 
		
	}

	void Update () {
        transform.Rotate(0, 100.0f * Time.deltaTime, 0);
	}

    private void OnTriggerEnter(Collider collider)
    {
        if (collider.gameObject.tag == "NightHawk")
        {
            hawk.PlayEnergyCapture();
            GameManager.GetComponent<GameManager>().AddObstaclePoints(10);
            Destroy(gameObject);
        }
    }
}
