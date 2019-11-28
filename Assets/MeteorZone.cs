using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeteorZone : MonoBehaviour
{
    public GameObject[] meteors;
    public Vector2 size;
    public float timeActive;
    private float time;

    public GameObject player;
    public GameObject particleSpitter;

    void Update() {
        if (player.transform.position.x + player.transform.localScale.x/2 > transform.position.x - size.x/2 && player.transform.position.x - player.transform.localScale.x/2 < transform.position.x + size.x/2) {
            if (!particleSpitter.GetComponent<ParticleSystem>().isPlaying) particleSpitter.GetComponent<ParticleSystem>().Play();
        } else {
            if (particleSpitter.GetComponent<ParticleSystem>().isPlaying) particleSpitter.GetComponent<ParticleSystem>().Stop();
        }
    }

    private void OnDrawGizmos() {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position, size);
    }
}
