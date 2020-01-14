using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/BurstProperty", order = 1)]
public class BurstProperties : ScriptableObject
{
    public Colors color;
    public int bounces;
    public float size;
    public bool splatOnAllCollisions;
    public bool splatBounceAndSize;
    public float minSplatSize;
    public float maxSplatSize;
    public float bounciness;
    public float friction;
    public float drag;

    public bool sizeOverVelocity;

}
    