using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack : MonoBehaviour
{
    [Header("Fire Settings")]
    public float reloadTime;
    public float maxForce;
    public float maxDistance;
    public float explosionRadius;

    private AttackTracker attackTracker;
    private float reloadTracker;

    [Header("Attack")]
    public GameObject burst;
    public BurstProperties[] burstTypes;

    // EXTRA
    private Camera mainCamera;

    void Start() {
        mainCamera = Camera.main;
        attackTracker = AttackTracker.IDLE;
    }

    void Update() {
        if (StateReciever.GetState() == States.INACTIVE) return;
        if (attackTracker == AttackTracker.IDLE && Input.GetAxisRaw("Attack") != 0) Shoot();
    }

    void FixedUpdate() {
        if (StateReciever.GetState() == States.INACTIVE) return;
        if (attackTracker == AttackTracker.RELOAD) {
            if (reloadTracker > reloadTime)
            {
                reloadTracker = 0;
                attackTracker = AttackTracker.IDLE;
            }
            else reloadTracker++;
        }
    }

    private void Shoot() {
        if (attackTracker != AttackTracker.IDLE) return;
        // Get mouse position
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = 0;
        // Convert to world position
        Vector3 worldMouse = mainCamera.ScreenToWorldPoint(mousePos);

        // Create Burst Attack
        GameObject firedShot = Instantiate(burst);
        int burstColor = (int)ColorSelector.GetColor();
        firedShot.GetComponent<Explode>().burstProperties = burstTypes[burstColor];
        firedShot.transform.position = transform.position;

        firedShot.tag = ColorSelector.GetColor().ToString();

        // Color changes
        Color color = new Color();
        Color highlightColor = new Color();
        ColorUtility.TryParseHtmlString(ColorScheme.primaryColors[burstColor], out color);
        ColorUtility.TryParseHtmlString(ColorScheme.highlightColors[burstColor], out highlightColor);
        firedShot.GetComponent<SpriteRenderer>().color = color;
        firedShot.transform.GetComponent<TrailRenderer>().startColor = highlightColor;
        firedShot.transform.GetComponent<TrailRenderer>().endColor = highlightColor;

        PhysicsMaterial2D physMat = new PhysicsMaterial2D();
        physMat.bounciness = burstTypes[burstColor].bounciness;
        physMat.friction = burstTypes[burstColor].friction;
        firedShot.GetComponent<Rigidbody2D>().sharedMaterial = physMat;
        firedShot.GetComponent<Rigidbody2D>().drag = burstTypes[burstColor].drag;

        firedShot.transform.localScale = new Vector2(burstTypes[burstColor].size,burstTypes[burstColor].size);

        // Force Calculations + Launch
        float angle = Mathf.Atan2(worldMouse.y - transform.position.y, worldMouse.x - transform.position.x);
        //float distance = Mathf.Sqrt(Mathf.Pow(worldMouse.x - transform.position.x, 2) + Mathf.Pow(worldMouse.y - transform.position.y, 2));
        firedShot.GetComponent<Rigidbody2D>().AddForce(new Vector2(maxForce * Mathf.Cos(angle) * Mathf.Clamp(Mathf.Abs(worldMouse.x - transform.position.x)/maxDistance, 0, 1), maxForce * Mathf.Sin(angle) * Mathf.Clamp(Mathf.Abs(worldMouse.y - transform.position.y)/maxDistance,0,1)));


        
        // Resets player to Reload atttack state
        attackTracker = AttackTracker.RELOAD;
    }

}
