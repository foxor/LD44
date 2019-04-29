using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraNudge : MonoBehaviour
{
    public float NudgeBorder = 0.3f;

    public Transform character;

    protected Camera myCamera;

    public void Start()
    {
        myCamera = GetComponent<Camera>();
        PlayerController.OnRespawn += Recenter;
    }

    protected void Recenter(ColliderType Killer)
    {
        transform.position = new Vector3(0f, 0f, transform.position.z);
    }

    public void LateUpdate()
    {
        Vector3 halfClip = new Vector3(myCamera.orthographicSize * myCamera.aspect, myCamera.orthographicSize, myCamera.farClipPlane);
        Vector3 bottomLeft = transform.position - halfClip;
        Vector3 characterDelta = character.transform.position - bottomLeft;
        Vector3 clipSpace = characterDelta.ComponentwiseDivide(halfClip) - Vector3.one;
        System.Func<float, float, float> clipToCorrection = (float clipSpaceElement, float halfClipElement) => {
            var offCenterMagnitude = Mathf.Abs(clipSpaceElement);
            var nudgeMagnitude = offCenterMagnitude - NudgeBorder;
            if (nudgeMagnitude < 0f)
            {
                return 0f;
            }
            return nudgeMagnitude * halfClipElement * (clipSpaceElement / offCenterMagnitude);
        };
        Vector3 correction = new Vector3(clipToCorrection(clipSpace.x, halfClip.x), clipToCorrection(clipSpace.y, halfClip.y));
        transform.position += correction;
    }
}
