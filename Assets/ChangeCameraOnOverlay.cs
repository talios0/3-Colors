using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeCameraOnOverlay : MonoBehaviour
{
    public ParticleSystem particleSystem;
    public GameObject UIElement;

    public Camera newCamera;

    [Range(0, 1)] // 0 is left side of screen, 1 is right
    public float horizontalPosition;
    public float range;

    void LateUpdate() {
        if (particleSystem.isPlaying) {
            Vector3 screenPoint = Camera.main.WorldToViewportPoint(UIElement.transform.position);
            if (screenPoint.x > horizontalPosition - range && screenPoint.x < horizontalPosition + range) {
                //Camera.main.enabled = false;
                //newCamera.enabled = true;
                //this.enabled = false;
                StateReciever.SetState(States.ACTIVE);
                SceneChanger.ChangeScene("Level1");
            }
        }
    }

}
