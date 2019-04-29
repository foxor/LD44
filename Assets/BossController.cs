using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossController : MonoBehaviour
{
    public GameObject Player;
    public GameObject LaserPrefab;

    public void FireLaser()
    {
        var Laser = GameObject.Instantiate<GameObject>(LaserPrefab);
        Laser.transform.position = transform.position;
        var ArrowComponent = Laser.GetComponent<ArrowController>();
        ArrowComponent.Velocity = (Player.transform.position - transform.position).normalized * ArrowComponent.Velocity.magnitude;
    }
}
