using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeteorZone : MonoBehaviour
{
    public GameObject[] meteors;
    public Vector2 size;
    public float timeActive;
    private float time;

    private void FixedUpdate() {
        
    }

    private void OnDrawGizmos() {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position, size);
    }
}
