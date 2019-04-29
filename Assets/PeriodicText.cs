using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PeriodicText : MonoBehaviour
{
    public string[] Texts = new string[]{};
    public float TotalDuration = 2.2f;
    public float BetweenMax = 10f;
    public float BetweenMin = 6f;
    public float FadeTime = 0.6f;
    public float FadeLift = 0.03f;
    public bool FadeUp;

    public TextMeshPro Text;

    public void Start()
    {
        Text.alpha = 0f;
        StartCoroutine(Wait());
    }

    public IEnumerator DisplayText(string text)
    {
        Text.alpha = 1f;
        Text.text = text;
        yield return new WaitForSeconds(TotalDuration - FadeTime);
        yield return Fade();
    }

    public IEnumerator Fade()
    {
        float Elapsed = 0f;
        float RiseVelocity = FadeLift / FadeTime;
        Vector3 Velocity = Vector3.zero;
        var Rect = Text.GetComponent<RectTransform>();
        Vector3 StartPosition = Rect.position;
        while (Elapsed < FadeTime)
        {
            yield return 0;
            Elapsed += Time.deltaTime;
            Vector3 FadeDirection = FadeUp ? Vector3.up : Vector3.down;
            Velocity += FadeDirection * RiseVelocity * Time.deltaTime * 2f;
            Velocity += Vector3.right * RiseVelocity * Time.deltaTime * 2f * Random.Range(-1f, 1f);
            Rect.position += Velocity;
            Text.alpha = 1f - (Elapsed / FadeTime);
        }
        Rect.position = StartPosition;
        Text.alpha = 0f;
        yield return Wait();
    }

    public IEnumerator Wait()
    {
        float WaitTime = Random.Range(BetweenMin, BetweenMax);
        yield return new WaitForSeconds(WaitTime);
        yield return DisplayText(Texts.ChooseRandom());
    }
}
