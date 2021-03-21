using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shootable : MonoBehaviour
{
    [SerializeField] private GameObject GameManager;
    [SerializeField] private NightHawk hawk;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    void Update()
    {
        transform.Rotate(0, 100.0f * Time.deltaTime, 0);
    }

    private void OnTriggerEnter(Collider collider)
    {
        Debug.Log(collider.gameObject.tag);
        if (collider.gameObject.tag == "Bullet")
        {
            hawk.PlayShot();
            GameManager.GetComponent<GameManager>().AddShootPoints(10);
            Destroy(gameObject);
        }
    }
}
