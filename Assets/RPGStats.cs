using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public abstract class Stat
{
    public static Action<string> OnLevelUp = (_) => {};
    public string Name;

    public abstract string LevelUp();
}

[System.Serializable]
public class LinearStat : Stat
{
    [SerializeField]
    protected float Min;

    [SerializeField]
    protected float Max;

    [SerializeField]
    protected int Levels;

    public int CurrentLevel {get; protected set;}

    public override string LevelUp()
    {
        if (CurrentLevel < Levels)
        {
            OnLevelUp(Name);
            CurrentLevel ++;
        }
        if (CurrentLevel < Levels)
        {
            return $"+1 {Name}";
        }
        return $"{Name} maxed";
    }

    public float Value
    {
        get {
            return Min + (Max - Min) * (CurrentLevel * 1f / Levels);
        }
    }
}

[System.Serializable]
public class UnlockStat : Stat
{
    [SerializeField]
    protected int Levels;

    public int CurrentLevel {get; protected set;}

    public override string LevelUp()
    {
        if (CurrentLevel < Levels)
        {
            CurrentLevel ++;
            OnLevelUp(Name);
        }
        if (CurrentLevel < Levels)
        {
            return $"{CurrentLevel}/{Levels} {Name}";
        }
        return $"{Name} unlocked";
    }

    public bool Unlocked {
        get {
            return CurrentLevel == Levels;
        }
    }
}

public class RPGStats : MonoBehaviour
{
    public LinearStat JumpSpeed;
    public LinearStat MoveSpeed;
    public UnlockStat WallClimbing;
    // TODO: Air steering when a boss kills you?
    public float DefaultMoveAcceleration = 0.3f;
    public float DefaultGravity = 0.1f;

    public GameObject FlyupPrefab;

    public float Gravity
    {
        get {
            return DefaultGravity;
        }
    }

    protected Stat StatFromColliderType(ColliderType Killer)
    {
        switch (Killer)
        {
            case ColliderType.SpeedPit:
                return MoveSpeed;
            case ColliderType.Spike:
                return JumpSpeed;
            case ColliderType.Arrow:
                return WallClimbing;
            case ColliderType.Wall:
            case ColliderType.Monster:
                return null;
            default:
                throw new System.Exception($"No stat for Collider Type: {Killer.ToString()}");
        }
    }

    public void LevelUp(ColliderType Killer)
    {
        var stat = StatFromColliderType(Killer);

        if (stat != null)
        {
            string flyupText = stat.LevelUp();

            var Flyup = GameObject.Instantiate(FlyupPrefab) as GameObject;
            Flyup.transform.position = transform.position;
            Flyup.GetComponent<Flyup>().Fly(flyupText);
        }
    }
}
