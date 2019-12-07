using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransitionStart : MonoBehaviour
{
    public ParticleSystem particle;
    public Camera transitionCamera;

    private void OnTriggerEnter2D(Collider2D other) {
        StateReciever.SetState(States.INACTIVE);
        transitionCamera.GetComponent<CameraSplice>().enabled = true;
        particle.Play();
        gameObject.SetActive(false);
    }

}
