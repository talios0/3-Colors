using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack : MonoBehaviour
{
    [Header("Fire Settings")]
    public float reloadTime;
    public float maxForce;
    public float explosionRadius;

    private AttackTracker attackTracker;
    private float reloadTracker;

    [Header("Attack")]
    public GameObject burst;

    // EXTRA
    private Camera mainCamera;

    void Start() {
        mainCamera = Camera.main;
    }

    void Update() {
        if (StateReciever.GetState() == States.INACTIVE) return;
        if (Input.GetAxisRaw("Attack") != 0) Shoot();
    }

    void FixedUpdate() {
        if (StateReciever.GetState() == States.INACTIVE) return;
    }

    private void Shoot() {
        if (attackTracker != AttackTracker.IDLE) return;
        // Get mouse position
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = 0;
        // Convert to world position
        Vector3 worldMouse = mainCamera.ScreenToWorldPoint(mousePos);
        // Angle between center of player and mouse position
        float angle = Mathf.Atan2(worldMouse.y - transform.position.y, worldMouse.x - transform.position.x);

        // Create Burst Attack
        GameObject firedShot = Instantiate(burst);
        firedShot.transform.position = transform.position;
        Color color = new Color();
        Color highlightColor = new Color();
        ColorUtility.TryParseHtmlString(ColorScheme.primaryColors[(int)ColorSelector.GetColor()], out color);
        ColorUtility.TryParseHtmlString(ColorScheme.highlightColors[(int)ColorSelector.GetColor()], out highlightColor);
        firedShot.GetComponent<SpriteRenderer>().color = color;
        firedShot.transform.GetComponent<TrailRenderer>().startColor = highlightColor;
        firedShot.transform.GetComponent<TrailRenderer>().endColor = highlightColor;
        firedShot.GetComponent<Rigidbody2D>().AddForce(new Vector2(maxForce * Mathf.Cos(angle), maxForce * Mathf.Sin(angle)));
    }

}
