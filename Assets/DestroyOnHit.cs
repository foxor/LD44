using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DestroyOnHit : MonoBehaviour
{
    public static event Action<string> OnDestroy = (_) => {};
    public ColliderType[] DestroyTypes;
    public CollisionTracker Tracker;
    public AudioClip DestroyClip;
    public void Update()
    {
        if (Tracker.CurrentCollisions.Any(x => DestroyTypes.Contains(x.Type)))
        {
            OnDestroy(tag);
            if (DestroyClip != null)
            {
                SFXSource.PlaySound(DestroyClip, transform.position);
            }
            Destroy(gameObject);
        }
    }
}
