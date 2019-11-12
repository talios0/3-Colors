using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hover : MonoBehaviour
{
    public float minDistance;
    public float maxDistance;
    public float maxForce;

    private Rigidbody2D rb;

    private float relativeGroundY;

    private Movement movement;

    void Start() {
        rb = GetComponent<Rigidbody2D>();
        movement = GetComponent<Movement>();
    }

    void FixedUpdate() {
        GetGroundY();
        AddHoverForce();
    }

    private void GetGroundY() {
        RaycastHit2D rayHit = Physics2D.Raycast(transform.position, Vector2.down);
        float groundY = rayHit.transform.position.y + rayHit.transform.localScale.y/2;
        relativeGroundY = transform.position.y - groundY;
    }

    private void AddHoverForce() {

        if (movement.GetInput().y != 0 && movement.GetJump()) return;
        float force = (maxDistance - relativeGroundY) / (maxDistance - minDistance);
        if (force < 0) return;
        force *= maxForce;
        rb.AddForce(Vector2.up * force);
    }
}
