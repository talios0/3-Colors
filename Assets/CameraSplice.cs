using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CameraSplice : MonoBehaviour
{
    public float speed;
    public Vector2 force;
    public float forceMultiplierOverTime;
    public float maxTime;
    public float runTimes;
    private float runInstance;


    public Camera sourceCamera, destinationCamera;
    public GameObject mask, image, UI;
    private RenderTexture sourceTexture, destinationTexture;
    private Material material;

    private float timeRunning;


    private Vector3 startPos;

    void Start() {
        destinationTexture = new RenderTexture(destinationCamera.pixelWidth, destinationCamera.pixelHeight, (int) destinationCamera.depth, UnityEngine.Experimental.Rendering.DefaultFormat.HDR);

        destinationCamera.targetTexture = destinationTexture;
        material = new Material(Shader.Find("Unlit/Texture"));
        material.SetTexture("_MainTex", destinationTexture);

        startPos = mask.GetComponent<RectTransform>().position;
    }

    private void FixedUpdate() {
        if (runInstance > runTimes) {
            mask.SetActive(false);
            image.SetActive(false);
            StateReciever.SetState(States.ACTIVE);
            this.enabled = false;
            return;
        }
        timeRunning++;
        if (timeRunning >= maxTime) {
            runInstance++;
            mask.GetComponent<RectTransform>().position = startPos;
            mask.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
            timeRunning = 0;
        }
        mask.GetComponent<Rigidbody2D>().AddForce(force * Mathf.Clamp(timeRunning/30, 0, 1) * forceMultiplierOverTime, ForceMode2D.Force);
    }

    private void LateUpdate() {
        image.transform.position = Camera.main.transform.position;
        image.GetComponent<RectTransform>().position = new Vector3(image.GetComponent<RectTransform>().position.x, image.GetComponent<RectTransform>().position.y, 0);
    }
}
