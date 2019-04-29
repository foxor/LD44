using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicRunner : MonoBehaviour
{
    public AudioSource MusicSource;
    public AudioClip Music;
    public float LengthToClip;
    public void Start() {
        GameObject.DontDestroyOnLoad(gameObject);
        StartCoroutine(Play());
    }

    protected IEnumerator Play() {
        MusicSource.clip = Music;
        var wait = new WaitForSeconds(Music.length - LengthToClip);
        while (true) {
            MusicSource.Play();
            yield return wait;
            MusicSource.Stop();
        }
    }
}
