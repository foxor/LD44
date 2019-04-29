using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SFXSource
{
    public static void PlaySound(AudioClip clip, Vector3 position)
    {
        var go = new GameObject("SFX");
        go.transform.position = position;
        var source = go.AddComponent<AudioSource>();
        source.rolloffMode = AudioRolloffMode.Linear;
        source.spatialBlend = 1f;
        source.minDistance = 0f;
        source.maxDistance = 8f;
        source.PlayOneShot(clip);
        GameObject.Destroy(go, clip.length);
    }
}
