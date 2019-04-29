using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowSpawner : MonoBehaviour
{
    public GameObject Prefab;
    public float Interval;

    public void Start()
    {
        StartCoroutine(Spawn());
    }

    protected IEnumerator Spawn()
    {
        var delay = new WaitForSeconds(Interval);
        while (true)
        {
            yield return delay;
            GameObject.Instantiate<GameObject>(Prefab);
            Prefab.transform.position = transform.position;
        }
    }
}
