using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions.Must;
using UnityEngine.UIElements;

public class Explode : MonoBehaviour
{
    public GameObject DecalManager;
    public BurstProperties burstProperties;
    public int color;

    [Header("Burst Size")]
    public GameObject splatter;
    public int size;
    public int points;
    public int smoothing;
    public Sprite baseSprite;

    public float maxAngle;

    private int bounces;

    private Rigidbody2D rb;
    private List<Vector3> verticies;

    private Collision2D previousCollision;
    private GameObject LineParent;
    private GameObject LineSprite;
    private bool airborne = true;

    // COLOR
    private Color unityColor;


    // OTHER
    private bool createLine;

    // Ray Trackers
    private RaycastHit2D[] previousRays;

    // Line Trackers
    private float slopeAngle;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        DecalManager = GameObject.Find("Decals");
        previousCollision = null;

        unityColor = new Color();
        ColorUtility.TryParseHtmlString(ColorScheme.primaryColors[(int)burstProperties.color], out unityColor);
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (burstProperties.createLine || other.transform.gameObject.layer != LayerMask.NameToLayer("Default")) return;

        Burst();
        if (bounces >= burstProperties.bounces) RemoveFromScene();

        bounces++;
    }

    private void TouchingGround()
    {
        if (!burstProperties.createLine) return;
        RaycastHit2D[] rays = new RaycastHit2D[3]; // left, down, right

        Vector3 bottomCenter = new Vector3(transform.position.x, transform.position.y - transform.localScale.y / 2, transform.position.z);

        rays[0] = Physics2D.Raycast(bottomCenter, Vector2.left, burstProperties.distanceFromGround + transform.localScale.x / 2, burstProperties.interactable);
        rays[1] = Physics2D.Raycast(bottomCenter, Vector2.down, burstProperties.distanceFromGround, burstProperties.interactable);
        rays[2] = Physics2D.Raycast(bottomCenter, Vector2.right, burstProperties.distanceFromGround + transform.localScale.x / 2, burstProperties.interactable);

        RaycastHit2D leaveRay = Physics2D.Raycast(bottomCenter, Vector2.down, burstProperties.distanceToLeaveGround, burstProperties.interactable);

        if (rays[1] == default)
        {
            airborne = true;
            if (LineSprite != null && leaveRay == default) EndLine();
        }
        else
        {
            if (airborne) bounces++;
            if (bounces > burstProperties.bounces) { Burst(); RemoveFromScene(); return; }
            airborne = false;
        }

        float newSlopeAngle = Transformations.GetSlopeAngle(transform, rays[0], rays[1], rays[2]);

        // Create Line
        if ((previousRays != null && rays[1] != default && previousRays[1] == default && LineSprite == null) || (!airborne && LineSprite != null && Mathf.Abs(newSlopeAngle - slopeAngle) > burstProperties.angleRange))
        {
            NewLine();
        }

        else if (LineSprite != null) ContinueLine();

        previousRays = rays;
        slopeAngle = newSlopeAngle;
    }


    private void OnTriggerExit2D(Collider2D other)
    {
        RaycastHit2D ray_down = Physics2D.Raycast(transform.position, Vector2.down, 0.5f, 1 << LayerMask.NameToLayer("Default"));
        if (ray_down != default(RaycastHit2D))
            airborne = false;
        else airborne = true;
    }


    private void Burst()
    {
        GameObject splat = Instantiate(splatter);
        splat.transform.position = new Vector2(transform.position.x, transform.position.y);
        splat.transform.eulerAngles = new Vector3(0, 0, Random.Range(0, 360));
        splat.GetComponent<SpriteRenderer>().color = unityColor;

        splat.transform.parent = DecalManager.transform;
        splat.GetComponent<EffectProperty>().properties = burstProperties;
    }

    private RaycastHit2D FindRelevantObject(RaycastHit2D[] hits)
    {
        foreach (RaycastHit2D hit in hits)
        {
            if (Physics2D.GetIgnoreLayerCollision(LayerMask.NameToLayer("Player Attack"), hit.transform.gameObject.layer)) continue;
            return hit; // Assuming first valid object is closest
        }

        return default(RaycastHit2D);

    }

    private void NewLine()
    {
        if (Physics2D.Raycast(transform.position, Vector2.down, 1f, 1 << LayerMask.NameToLayer("Effect"))) LineSprite = null;

        // Variable Initialization
        verticies = new List<Vector3>();
        LineParent = new GameObject();
        LineSprite = new GameObject();

        // Direction
        int direction = 1;
        if (rb.velocity.x < 0) direction = -1;

        // Position
        LineParent.transform.position = new Vector2(transform.position.x - (direction * transform.localScale.x / 2), transform.position.y - transform.localScale.y / 2);
        LineSprite.transform.parent = LineParent.transform;

        // Sprite Renderer
        SpriteRenderer sr = LineSprite.AddComponent<SpriteRenderer>();
        sr.sprite = burstProperties.line;
        if (direction == -1) LineSprite.transform.localScale = new Vector3(-1, 1, 1);
        sr.color = unityColor;
        sr.sortingOrder = 6;
        sr.drawMode = SpriteDrawMode.Tiled;
        sr.maskInteraction = SpriteMaskInteraction.VisibleInsideMask;

        // Collider
        LineSprite.AddComponent<BoxCollider2D>();
        LineSprite.GetComponent<BoxCollider2D>().isTrigger = true;
        LineSprite.GetComponent<BoxCollider2D>().size = new Vector2(0.5f, 0.5f);
        LineSprite.GetComponent<BoxCollider2D>().offset = new Vector2(0.25f, -0.25f);

        // Parent Details
        LineParent.transform.parent = DecalManager.transform;
        LineSprite.layer = LayerMask.NameToLayer("Effect");

        // Effect
        LineSprite.AddComponent<EffectProperty>();
        LineSprite.GetComponent<EffectProperty>().properties = burstProperties;

        // Position and Scale
        LineSprite.transform.localPosition = new Vector2(0, 0);
        LineSprite.transform.localScale = new Vector3(direction, 1, 1);
        sr.size = new Vector2(transform.localScale.x / 2, sr.size.y);

        // Set angle and start line
        SetSlopeAngle();
    }

    private void EndLine()
    {
        verticies = null;
        LineSprite = null;
        LineParent = null;
    }

    private void ContinueLine()
    {
        if (LineParent == null) return;
        if (rb.velocity.magnitude < 0.25f) { Destroy(gameObject); return; }

        verticies.Add(new Vector3(transform.position.x, transform.position.y - transform.localScale.y / 2, -0.25f));
        if (verticies.Count == 1) return;

        LineSprite.GetComponent<SpriteRenderer>().size = new Vector2(Mathf.Abs(verticies[0].x - verticies[verticies.Count - 1].x), LineSprite.GetComponent<SpriteRenderer>().size.y);
        //LineParent.transform.position = new Vector3((verticies[verticies.Count - 1].x + verticies[0].x)/2, (verticies[verticies.Count - 1].y + verticies[0].y)/2, 0);
    }

    private void RemoveFromScene()
    {
        Destroy(gameObject);
    }

    private void Update()
    {
        TouchingGround();
    }

    private void CheckAirborne()
    {
        if (Physics2D.Raycast(transform.position, Vector2.down, 0.5f) == default(RaycastHit2D))
        {
            airborne = true;
        }
    }

    private void SetSlopeAngle()
    {
        LineParent.transform.eulerAngles = new Vector3(0, 0, Transformations.GetSlopeAngle(transform, rb, new Vector3(0, -transform.localScale.y / 2, 0)));
    }

}