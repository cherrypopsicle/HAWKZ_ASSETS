using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Scoreboard : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI shootPoints;
    [SerializeField] private TextMeshProUGUI obstaclePoints;
    [SerializeField] private TextMeshProUGUI time;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void UpdateShootPoints(int shootPoints)
    {
        this.shootPoints.text = shootPoints.ToString();
    }

    public void UpdateObstaclePoints(int obstaclePoints)
    {
        this.obstaclePoints.text = obstaclePoints.ToString();
    }

    public void UpdateTime(string time)
    {
        this.time.text = time;
    }
}
