using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    public float speed;
    public float distanceToStartJump;
    public int maxJumpTime; // # of Frames jump can be held for
    public float jumpForce;

    private int jumpTime; // # of frames since jump has started
    private bool initiateJump = false;

    private Vector3 input;
    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
        GetInput();
        Move();
        Jump();
    }

    public Vector3 GetInput()
    {
        input.x = Input.GetAxisRaw("Horizontal");
        input.y = Input.GetAxis("Jump");
        return input;
    }

    void Move()
    {
        if (input.x != 0) rb.AddForce(input.x * speed * Vector2.right);
        else if (rb.velocity.x != 0)
        {
            bool xDirection = rb.velocity.x > 0;
            if (xDirection) rb.AddForce(-speed / 5 * Vector2.right);
            else rb.AddForce(speed / 5 * Vector2.right);
        }
    }



    void InitiateJump()
    {
        if (input.y == 0) return;
        // RAYCAST CHECK
        RaycastHit2D rayHit = Physics2D.Raycast(new Vector3(transform.position.x, transform.position.y + transform.localScale.y / 2, transform.position.z), Vector2.down);
        if ((transform.position.y + transform.localScale.y / 2) - (rayHit.transform.position.y + rayHit.transform.localScale.y / 2) > distanceToStartJump) return;
        initiateJump = true;
    }

    void Jump()
    {
        if (!initiateJump)
        {
            InitiateJump();
            return;
        }
        else if (jumpTime < maxJumpTime) {
            if (input.y != 0) rb.AddForce(jumpForce * Vector2.up);
            jumpTime++;
        }

        if (jumpTime >= maxJumpTime) {
            initiateJump = false;
            jumpTime = 0;
        }
    }

    public bool GetJump() {
        return initiateJump;
    }
}
