using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Flyup : MonoBehaviour
{
    public TextMeshPro Text;

    public float FlyTime;
    public float Distance;
    public void Fly(string Text)
    {
        this.Text.text = Text;
        StartCoroutine(DoFlyup());
    }

    protected IEnumerator DoFlyup()
    {
        float Elapsed = 0f;
        while (Elapsed < FlyTime)
        {
            yield return 0;
            Elapsed += Time.deltaTime;
            transform.position += Vector3.up * Time.deltaTime * Distance / FlyTime;
            Text.alpha = 1f - (Elapsed / FlyTime);
        }
        Destroy(gameObject);
    }
}
