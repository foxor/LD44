using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public static event System.Action<ColliderType> OnRespawn = (_) => {};

    public int JumpForgivnessFrames;
    public CollisionTracker ObstructionTracker;
    public CollisionTracker DeathTracker;
    public float WallSlideLimit;
    public Inventory Inventory;

    public AudioClip Acid;

    protected RPGStats Stats;
    protected Vector2 Position;
    protected Vector2 Velocity;
    protected Rigidbody2D[] RBs;
    protected Vector2 OriginalPosition;
    protected int FramesSinceGrounded = 100;
    protected bool WallBracedLeft = false;
    protected bool WallBracedRight = false;

    protected bool PressedJump = false;
    void Start()
    {
        RBs = GetComponentsInChildren<Rigidbody2D>();
        OriginalPosition = RBs[0].position;
        Stats = GetComponent<RPGStats>();
        Inventory.OnTeleported += Teleported;
    }

    protected void PushOutOfOverlap(CollisionType Type, ref Vector2 Position, ref Vector2 Velocity)
    {
        bool XAxis = Type == CollisionType.OnRight || Type == CollisionType.OnLeft;
        bool PushPositive = Type == CollisionType.OnLeft || Type == CollisionType.OnBottom;
        var Value = XAxis ? Position.x : Position.y;
        var NewValue = Mathf.Round(Value);
        var Push = NewValue - Value;

        if (Push > 0 != PushPositive)
        {
            return;
        }

        if (XAxis)
        {
            Position.x += Push;
            if ((Velocity.x > 0) != (Push > 0))
            {
                //Velocity.x = 0f;
            }
        }
        else
        {
            Position.y += Push;
            if ((Velocity.y > 0) != (Push > 0))
            {
                Velocity.y = 0f;
            }
        }
    }

    protected static bool CollisionIsObstruction(CollisionInfo Info)
    {
        return Info.Type == ColliderType.Wall;
    }

    protected static bool CollisionCausesDeath(CollisionInfo Info)
    {
        return Info.Type == ColliderType.SpeedPit || Info.Type == ColliderType.Spike || Info.Type == ColliderType.Arrow || Info.Type == ColliderType.Monster || Info.Type == ColliderType.Boss;
    }

    protected static bool CollisionIsOverlap(CollisionInfo Info)
    {
        return Info.Adjacency != CollisionType.NoContact;
    }

    protected static bool CollisionIsPowerup(CollisionInfo Info)
    {
        return Info.Type == ColliderType.Teleporter || Info.Type == ColliderType.Bow;
    }

    protected static bool IsHorizontalOverlap(CollisionInfo Info)
    {
        return Info.Adjacency == CollisionType.OnLeft || Info.Adjacency == CollisionType.OnRight || Info.Adjacency == CollisionType.Overlap;
    }

    protected static bool IsVerticalOverlap(CollisionInfo Info)
    {
        return Info.Adjacency == CollisionType.OnBottom || Info.Adjacency == CollisionType.OnTop || Info.Adjacency == CollisionType.Overlap;
    }

    protected void Teleported()
    {
        Position = transform.position;
        SyncRBToPosition();
    }

    protected void SyncRBToPosition()
    {
        transform.position = Position;
        foreach (var rb in RBs)
        {
            rb.position = Position;
            rb.velocity = Vector2.zero;
        }
    }

    protected void Respawn(ColliderType Killer)
    {
        Position = OriginalPosition;
        SyncRBToPosition();
        Velocity = Vector2.zero;
        Stats.LevelUp(Killer);
        OnRespawn(Killer);
    }

    protected void HandleFalling()
    {
        if (FramesSinceGrounded > 0)
        {
            Velocity.y += Stats.Gravity * Time.fixedDeltaTime;
            if ((WallBracedLeft || WallBracedRight) && Stats.WallClimbing.Unlocked)
            {
                // Since we limit air-steering, you'll fall off a wall if you aren't being held into it
                float braceHoldDirection = WallBracedRight ? 1f : -1f;
                if (Velocity.y > 0f)
                {
                    Velocity.y = 0f;
                }
                Velocity.y = Mathf.Max(Velocity.y, WallSlideLimit);
                if (PressedJump)
                {
                    Velocity.y = Stats.JumpSpeed.Value;
                    Velocity.x = -braceHoldDirection * Stats.JumpSpeed.Value;
                    Position.x -= braceHoldDirection * 0.1f;
                    SyncRBToPosition();
                    // This thinks we're jumping the correct direction, we just get push backward by something in the next tick
                }
                else
                {
                    Velocity.x = braceHoldDirection * Stats.MoveSpeed.Value;
                }
            }
        }
        if (FramesSinceGrounded < JumpForgivnessFrames)
        {
            if (PressedJump)
            {
                Velocity.y = Stats.JumpSpeed.Value;
                FramesSinceGrounded = JumpForgivnessFrames + 1;
            }
            Velocity.x = Input.GetAxis("Horizontal") * Stats.MoveSpeed.Value;
        }
        PressedJump = false;
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            PressedJump = true;
        }
    }

    public void FixedUpdate()
    {
        HandleFalling();

        // Apply input to the model
        Position = ObstructionTracker.RB.position + Velocity * Time.fixedDeltaTime;
        SyncRBToPosition();

        // Collect feedback
        var Collisions = ObstructionTracker.CurrentCollisions;
        var Overlaps = Collisions.Where(CollisionIsOverlap);
        // Horizontal first, then vertical, so you can't wedge yourself in a crack in a wall
        var Obstructions = Overlaps.Where(CollisionIsObstruction).Where(IsHorizontalOverlap);
        
        WallBracedLeft = false;
        WallBracedRight = false;
        foreach (var Obstruction in Obstructions)
        {
            if (Obstruction.Adjacency == CollisionType.Overlap)
            {
                Respawn(ColliderType.Wall);
                return;
            }
            PushOutOfOverlap(Obstruction.Adjacency, ref Position, ref Velocity);
            if (Obstruction.Adjacency == CollisionType.OnLeft)
            {
                WallBracedLeft = true;
            }
            else
            {
                WallBracedRight = true;
            }
        }

        // Get new collisions, in case we got shoved out of a wall, which could change a OnBottom to a NoContact
        SyncRBToPosition();
        Collisions = ObstructionTracker.CurrentCollisions;
        Obstructions = Collisions.Where(CollisionIsObstruction).Where(IsVerticalOverlap);
        if (Obstructions.Any(x => x.Adjacency == CollisionType.OnBottom))
        {
            FramesSinceGrounded = 0;
        }
        else
        {
            FramesSinceGrounded ++;
        }
        
        foreach (var Obstruction in Obstructions)
        {
            PushOutOfOverlap(Obstruction.Adjacency, ref Position, ref Velocity);
        }

        transform.position = Position;
        SyncRBToPosition();
        
        Collisions = DeathTracker.CurrentCollisions;
        var Killers = Collisions.Where(CollisionCausesDeath);
        var Powerups = Collisions.Where(CollisionIsPowerup);

        if (Killers.Any())
        {
            Respawn(Killers.First().Type);
            if (Killers.First().Type == ColliderType.SpeedPit)
            {
                SFXSource.PlaySound(Acid, transform.position);
            }
            return;
        }

        if (Powerups.Any())
        {
            var Powerup = Powerups.First();
            Inventory.Unlock(Powerup.Type);
            GameObject.Destroy(Powerup.Collider.gameObject);
        }
    }
}
