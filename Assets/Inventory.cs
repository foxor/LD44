using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public static event Action OnTeleported = () => {};

    public GameObject Bow;
    public GameObject Teleporter;
    public GameObject ArrowPrefab;
    public AudioClip Teleport;

    protected Vector3 LastInput;
    protected int SelectedIndex = -1;
    protected float TeleportCooldown = -1f;

    public GameObject Selected {
        get {
            return SelectedIndex == 0 ? Bow : Teleporter;
        }
    }

    protected bool[] Unlocks = new bool[]{ false, false };

    protected void UnlockIndex(int index)
    {
        Unlocks[index] = true;
        SelectedIndex = index;
    }

    public void Unlock(ColliderType Type)
    {
        switch (Type)
        {
            case ColliderType.Bow:
                UnlockIndex(0);
                break;
            case ColliderType.Teleporter:
                UnlockIndex(1);
                break;
        }
    }

    protected void Swap(){
        if (Unlocks[0] && Unlocks[1])
        {
            SelectedIndex = (SelectedIndex + 1) % 2;
        }
    }
    public void Update() {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            Swap();
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (SelectedIndex == 0)
            {
                // Fire bow
                var Arrow = GameObject.Instantiate<GameObject>(ArrowPrefab);
                Arrow.tag = "PlayerArrow";
                Arrow.transform.position = transform.position;
                Arrow.transform.localScale = Selected.transform.localScale;
                Arrow.transform.localRotation = Selected.transform.localRotation;
                var ArrowComponent = Arrow.GetComponent<ArrowController>();
                ArrowComponent.Velocity = LastInput.normalized * ArrowComponent.Velocity.magnitude;
            }
            else
            {
                // Teleport
                transform.position += LastInput.normalized * 2f;
                OnTeleported();
                SFXSource.PlaySound(Teleport, transform.position);
            }
        }

        Bow.SetActive(SelectedIndex == 0 && Unlocks[0]);
        Teleporter.SetActive(SelectedIndex == 1 && Unlocks[1]);

        Vector3 InputAxis = new Vector3(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        if (InputAxis.sqrMagnitude > Mathf.Epsilon)
        {
            LastInput = InputAxis;
        }
        else
        {
            InputAxis = LastInput;
        }
        Vector3 ScaleDirection = InputAxis.x > 0f ? Vector3.right : Vector3.left;
        var Rotation = Quaternion.FromToRotation(ScaleDirection, InputAxis);
        Selected.transform.localScale = new Vector3(InputAxis.x > 0f ? -1f : 1f, 1f, 1f);
        Selected.transform.localRotation = Rotation;
    }
}
