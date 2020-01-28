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
    private JumpState jumpState;

    private RaycastHit2D lastRayhit;

    private Vector3 input;
    private Rigidbody2D rb;

    private ColorSelector colorSelector;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        //colorSelector = GameObject.Find("Color Selector").GetComponent<ColorSelector>();
        lastRayhit = Physics2D.Raycast(new Vector3(transform.position.x, transform.position.y + transform.localScale.y / 2, transform.position.z), Vector2.down);
    }

    void FixedUpdate()
    {
        if (StateReciever.GetState() == States.INACTIVE) return;
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
        if (input.x == -1)  {
            GetComponent<SpriteRenderer>().flipX = true;
        }
        if (input.x == 1) {
            GetComponent<SpriteRenderer>().flipX = false;
        }
        RaycastHit2D ground = GetGround();
        Vector2 dir = Vector2.right;
        if (ground != default(RaycastHit2D)) {
            float angle = Transformations.To360(Transformations.GetSlopeAngle(transform, rb, new Vector3(0, -0.6f, 0))) * Mathf.Deg2Rad;
            dir = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
        }

        if (input.x != 0) rb.AddForce(input.x * speed * dir);
        else if (rb.velocity.x != 0)
        {
            bool xDirection = rb.velocity.x > 0;
            if (xDirection) rb.AddForce(-speed / 4 * dir);
            else rb.AddForce(speed / 4 * dir);
        }
    }

    void InitiateJump()
    {
        // RAYCAST CHECK
        RaycastHit2D rayHit = GetGround();
        if (rayHit == new RaycastHit2D())
        {
            if (rayHit == lastRayhit) return;
            if (GetComponent<Hover>().GetGapStatus() == GapHoverState.ENDED) return;
            rayHit = lastRayhit;
            if (initiateJump) lastRayhit = new RaycastHit2D();
        }
        else {
            lastRayhit = rayHit;
            if (jumpState == JumpState.INITIATED) return;
            jumpState= JumpState.NONE;
        }

        if (input.y == 0 || jumpState == JumpState.ENDED) return;
        if (rayHit.distance > distanceToStartJump) return;

        jumpState = JumpState.INITIATED;
    }

    void Jump()
    {
        InitiateJump();
        if (jumpTime < maxJumpTime && jumpState == JumpState.INITIATED) {
            if (input.y != 0) rb.AddForce(jumpForce * Vector2.up);
            jumpTime++;
        }

        if (jumpTime >= maxJumpTime) {
            jumpState = JumpState.ENDED;
            jumpTime = 0;
        }
    }

    public JumpState GetJump() {
        return jumpState;
    }

    public RaycastHit2D GetGround() {
        RaycastHit2D[] rayHits = Physics2D.RaycastAll(new Vector3(transform.position.x, transform.position.y + transform.localScale.y / 2, transform.position.z), Vector2.down, 5f, 1 << LayerMask.NameToLayer("Default"));
        RaycastHit2D ground = new RaycastHit2D();
        foreach (RaycastHit2D obj in rayHits) {
            if (Physics2D.GetIgnoreLayerCollision(gameObject.layer, obj.transform.gameObject.layer)) continue;
            if (ground == default(RaycastHit2D)) {
                ground = obj;
                continue;
            }
            if (ground.distance < obj.transform.position.y + obj.transform.localScale.y / 2) ground = obj;
        }

        return ground;
    }
}