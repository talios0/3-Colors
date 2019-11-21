using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hover : MonoBehaviour
{
    public float minDistance;
    public float maxDistance;
    public float maxForce;
    public float gapForceMultiplier;
    public float gapDistanceMultiplier;
    public float gapMinDistanceMultiplier;
    public ParticleSystem hoverEffect;

    public float gapHoverTime;
    private float time;
    private GapHoverState gapHover;
    private bool gapDetected;

    private Rigidbody2D rb;

    private float relativeGroundY;
    private float lastRelativeGroundY;
    private RaycastHit2D lastRayHit;


    [Header("Random Force")]
    // RANDOM FORCE
    public float randomForce;
    public Vector2 randomForceWaitRange;
    private int randomForceWaitTime;
    private int randomForceTime;
    


    private Movement movement;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        movement = GetComponent<Movement>();
    }

    void FixedUpdate()
    {
        GetGroundY();
        AddHoverForce();
        if (!gapDetected && gapHover == GapHoverState.ENDED) gapHover = GapHoverState.NONE;
        RandomForce();
        if (gapHover == GapHoverState.HOVER || movement.GetJump() == JumpState.INITIATED) {
            hoverEffect.Play();
        } else {
            hoverEffect.Stop();
        }
    }

    private void GetGroundY()
    {
        RaycastHit2D rayHit = Physics2D.Raycast(transform.position, Vector2.down);
        float groundY;
        if (rayHit == new RaycastHit2D())
        {
            gapDetected = true;
            if (gapHover != GapHoverState.ENDED) gapHover = GapHoverState.HOVER;
            groundY = lastRayHit.transform.position.y + lastRayHit.transform.localScale.y / 2;
            lastRelativeGroundY = transform.position.y - transform.localScale.y / 2 - groundY;
            return;
        }
        gapDetected = false;
        gapHover = GapHoverState.NONE;
        time = 0;
        groundY = rayHit.transform.position.y + rayHit.transform.localScale.y / 2;
        relativeGroundY = transform.position.y - transform.localScale.y / 2 - groundY;
        lastRelativeGroundY = relativeGroundY;
        if (rayHit.transform.gameObject.layer != LayerMask.NameToLayer("Dynamic")) lastRayHit = rayHit;
    }


    private void AddHoverForce()
    {
        float yPosition = relativeGroundY;
        float hoverForce = maxForce;
        float highDistance = maxDistance;
        float lowDistance = minDistance;
        if (gapHover == GapHoverState.HOVER)
        {
            if (gapHoverTime <= time) {
                time = 0;
                gapHover = GapHoverState.ENDED;
                return;
            }
            yPosition = lastRelativeGroundY;
            time++;
            highDistance*=gapDistanceMultiplier;
            hoverForce *= gapForceMultiplier;
            lowDistance *= gapMinDistanceMultiplier;
        }
        if (gapHover == GapHoverState.ENDED) return;
        if (movement.GetInput().y != 0 && movement.GetJump() == JumpState.INITIATED) return;
        float force = (highDistance - yPosition) / (highDistance - lowDistance);
        if (force < 0) return;
        force *= hoverForce;
        rb.AddForce(Vector2.up * force);
    }

    private void RandomForce() {
        if (randomForceWaitTime > randomForceTime) {
            randomForceWaitTime = Mathf.RoundToInt(Random.Range(randomForceWaitRange.x, randomForceWaitRange.y));
            randomForceTime++;
            return;
        }
        randomForceTime = 0;
        randomForceWaitTime = Mathf.RoundToInt(Random.Range(randomForceWaitRange.x, randomForceWaitRange.y));
        int direction = Mathf.RoundToInt(Random.Range(0f, 0.5f) - 1);
        rb.AddForce(Vector2.up * direction *randomForce, ForceMode2D.Impulse);
    }

    public GapHoverState GetGapStatus() {
        return gapHover;
    }
}
