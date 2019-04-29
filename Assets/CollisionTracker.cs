using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum CollisionType
{
    Error,
    Overlap,
    NoContact,
    OnTop,
    OnBottom,
    OnLeft,
    OnRight,
}

public enum ColliderType
{
    Error,
    Wall,
    SpeedPit,
    Spike,
    None,
    Arrow,
    Bow,
    Character,
    Teleporter,
    PlayerArrow,
    Monster,
    Boss,
}

public class CollisionInfo
{
    public static readonly float Epsilon = 0.01f;
    public static readonly float MaxIntrusion = 0.8f;
    public Vector2 Center;
    public Collider2D Collider;
    public ColliderType Type;
    public CollisionType Adjacency;

    protected static CollisionType ClosestWayOut(Vector2 Delta)
    {
        float absX = Mathf.Abs(Delta.x);
        float absY = Mathf.Abs(Delta.y);
        if (absX < MaxIntrusion && absY < MaxIntrusion)
        {
            return CollisionType.Overlap;
        }
        if (absX < absY)
        {   // Try and push out the y direction
            if (Delta.y > 0)
            {
                return CollisionType.OnTop;
            }
            return CollisionType.OnBottom;
        }
        if (Delta.x > 0)
        {
            return CollisionType.OnRight;
        }
        return CollisionType.OnLeft;
    }

    // Returns collisionType of the form Source is CollisionType of this.  If this returns OnTop, Source is on top of this.
    public CollisionType InteractionWith(Vector2 Source)
    {
        Vector2 Delta = Center - Source;
        
        if (Mathf.Abs(Delta.x) - Epsilon > 1f || Mathf.Abs(Delta.y) - Epsilon > 1f) // We're not close
        {
            return CollisionType.NoContact;
        }
        if (Mathf.Abs(Delta.x) + Epsilon < 1f) // We're overlapping on x
        {
            if (Mathf.Abs(Delta.y) + Epsilon < 1f) // We're overlapping on y
            {
                //return CollisionType.Overlap;
                return ClosestWayOut(Delta);
            }
            if (Delta.y > 0) // We must be Mathf.Abs(Delta.y) ~= 1
            {
                // Further refine this for "wheeling arms" animations?
                return CollisionType.OnTop;
            }
            return CollisionType.OnBottom;
        }
        if (Mathf.Abs(Delta.y) + Epsilon < 1f) // We're diagonally adjacent
        {
            return CollisionType.NoContact;
        }
        if (Delta.x > 0) // We must be Mathf.Abs(Delta.x) ~= 1
        {
            return CollisionType.OnRight;
        }
        return CollisionType.OnLeft;
    }
}

public class CollisionTracker : MonoBehaviour
{
    protected List<CollisionInfo> Collisions = new List<CollisionInfo>();
    public Rigidbody2D RB;

    public static readonly Dictionary<string, ColliderType> CollisionTypes = new Dictionary<string, ColliderType>()
    {
        {"Wall", ColliderType.Wall},
        {"SpeedPit", ColliderType.SpeedPit},
        {"Spike", ColliderType.Spike},
        {"Untagged", ColliderType.None},
        {"Arrow", ColliderType.Arrow},
        {"Bow", ColliderType.Bow},
        {"Character", ColliderType.Character},
        {"Teleporter", ColliderType.Teleporter},
        {"PlayerArrow", ColliderType.PlayerArrow},
        {"Monster", ColliderType.Monster},
        {"Boss", ColliderType.Boss},
    };
    public IEnumerable<CollisionInfo> CurrentCollisions
    {
        get {
            foreach (var Collision in Collisions)
            {
                Collision.Adjacency = Collision.InteractionWith(RB.position);
            }
            return Collisions;
        }
    }

    public void Awake()
    {
        RB = GetComponent<Rigidbody2D>();
    }

    public void OnCollisionEnter2D(Collision2D col)
    {
        Collisions.Add(new CollisionInfo() {
            Center = col.rigidbody.position,
            Collider = col.collider,
            Type = CollisionTypes[col.gameObject.tag]
        });
    }

    public void OnCollisionExit2D(Collision2D col)
    {
        Collisions.RemoveAll(x => x.Collider == col.collider);
    }
}
