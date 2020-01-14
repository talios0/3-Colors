using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    private int bounces;


    private Rigidbody2D rb;
    private List<Vector3> verticies;

    private Collision2D previousCollision;
    private GameObject LineSprite;
    private bool airborne;

    // COLOR
    private Color unityColor;

    private void Start() {
        rb = GetComponent<Rigidbody2D>();
        DecalManager = GameObject.Find("Decals");
        previousCollision = null;

        unityColor = new Color();
        ColorUtility.TryParseHtmlString(ColorScheme.primaryColors[(int)burstProperties.color], out unityColor);
    }
    private void OnCollisionEnter2D(Collision2D other) {
        if (burstProperties.sizeOverVelocity) {
            if (Physics2D.Raycast(transform.position, Vector2.down).transform != other.transform) {
                Burst(other);
                RemoveFromScene();
                return;
            }
            if (previousCollision != null && other.gameObject.GetInstanceID() == previousCollision.gameObject.GetInstanceID()) { OnCollisionStay2D(other);return; }
            previousCollision = other;
            if (bounces >= burstProperties.bounces) {
                RemoveFromScene();
            } else {
                NewLine(other);
            }
            bounces++;
            return;
        }
        Burst(other);
        if (bounces >= burstProperties.bounces) {
            RemoveFromScene();
        }
        bounces++;
    }

    private void OnCollisionStay2D(Collision2D other) {
        if (!burstProperties.sizeOverVelocity) return;
        float newSize = Mathf.Sqrt(Mathf.Pow(rb.velocity.x,2) + Mathf.Pow(rb.velocity.y,2));
        if (newSize < 0.5f) { RemoveFromScene(); return; }
        ContinueLine(other);
    }

    private void Burst(Collision2D col) {
        GameObject splat = Instantiate(splatter);
        splat.transform.position = new Vector2(transform.position.x, transform.position.y);
        splat.transform.eulerAngles = new Vector3(0,0,Random.Range(0,360));
        splat.GetComponent<SpriteRenderer>().color = unityColor;

        splat.transform.parent = DecalManager.transform;
    }

    private RaycastHit2D FindRelevantObject(RaycastHit2D[] hits) {
        foreach (RaycastHit2D hit in hits) {
            if (Physics2D.GetIgnoreLayerCollision(LayerMask.NameToLayer("Player Attack"), hit.transform.gameObject.layer)) continue;
            return hit; // Assuming first valid object is closest
        }

        return default(RaycastHit2D);

    }

    private void NewLine(Collision2D other) {

        verticies = new List<Vector3>();
        LineSprite = new GameObject();
        LineSprite.AddComponent<SpriteRenderer>();
        LineSprite.GetComponent<SpriteRenderer>().sprite = baseSprite;
        LineSprite.GetComponent<SpriteRenderer>().color = unityColor;
        LineSprite.GetComponent<SpriteRenderer>().sortingOrder = 6;
        LineSprite.GetComponent<SpriteRenderer>().maskInteraction = SpriteMaskInteraction.VisibleInsideMask;
        LineSprite.AddComponent<BoxCollider2D>();
        LineSprite.GetComponent<BoxCollider2D>().isTrigger = true;
        LineSprite.GetComponent<BoxCollider2D>().size = new Vector2(1, 0.5f);
        LineSprite.GetComponent<BoxCollider2D>().offset = new Vector2(0,-0.25f);
        LineSprite.transform.parent = DecalManager.transform;
        LineSprite.layer = LayerMask.NameToLayer("Effect");

        SetSlopeAngle();
    }

    private void ContinueLine(Collision2D other) {
        if (rb.velocity.magnitude < 0.25f) { Destroy(gameObject); return; }
        
        verticies.Add(new Vector3(transform.position.x, transform.position.y - transform.localScale.y/2, -0.25f));
        if (verticies.Count == 1) return;
        
        LineSprite.transform.localScale = new Vector3(verticies[0].x - verticies[verticies.Count-1].x, 1, 1);
        LineSprite.transform.position = new Vector3((verticies[verticies.Count - 1].x + verticies[0].x)/2, verticies[0].y, 0);
    }

    private void RemoveFromScene() {
        Destroy(gameObject);
    }

    private void Update() {
        float maxSpeed = 30;

        if (Mathf.Abs(GetSlopeAngle()) > Mathf.Abs(LineSprite.transform.eulerAngles.z + 1) && Mathf.Abs(GetSlopeAngle()) < Mathf.Abs(LineSprite.transform.eulerAngles.z - 1)) {
            NewLine();
        } 

        if (burstProperties.sizeOverVelocity) {
            float newSize = Mathf.Sqrt(Mathf.Pow(rb.velocity.x,2) + Mathf.Pow(rb.velocity.y,2)) / maxSpeed;
            transform.localScale = new Vector3(burstProperties.size*newSize, burstProperties.size*newSize);
        }

        CheckAirborne();
    }

    private void CheckAirborne() {
        if (Physics2D.Raycast(transform.position, Vector2.down, 0.5f) == default(RaycastHit2D)) {
            airborne = true;
        }
    }

    private float GetSlopeAngle() {
        RaycastHit2D ray_down = Physics2D.Raycast(transform.position,Vector2.down, 1f);
        RaycastHit2D ray_dir = Physics2D.Raycast(transform.position, Vector2.left, 1f);

        if (rb.velocity.x > 0 && rb.velocity.y > 0 || rb.velocity.x < 0 && rb.velocity.y < 0) {
            ray_dir = Physics2D.Raycast(transform.position, Vector2.left, 1f);
        } else {
            ray_dir = Physics2D.Raycast(transform.position, Vector2.right, 1f);
        }
        
        if (ray_dir == default(RaycastHit2D)) return 0f;

        return Mathf.Atan2(ray_dir.distance, ray_down.distance) * Mathf.Rad2Deg;
    }

    private void SetSlopeAngle() {
        LineSprite.transform.eulerAngles = new Vector3(0,0, GetSlopeAngle());
    }

}
