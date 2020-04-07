using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/BurstProperty", order = 1)]
public class BurstProperties : ScriptableObject
{
    [Header("Basic Properties")]
    public Colors color;
    public int bounces;
    public float size;
    public bool splatOnAllCollisions;
    public bool splatBounceAndSize;
    public float minSplatSize;
    public float maxSplatSize;

    [Header("Physics Properties")]
    public float bounciness;
    public float friction;
    public float drag;

    [Header("Misc")]
    public bool sizeOverVelocity;

    [Header("Splatter")]
    public Sprite splatter;
    public Sprite line;

    [Header("Splatter Properties")]
    public bool createLine;

    [Header("Slope Detection")]
    public float angleRange;

    [Header("Ground Detection")]
    public float distanceFromGround;
    public float distanceToLeaveGround;
    public LayerMask interactable;

    [Header("Debug")]
    public bool showRays;

}
    