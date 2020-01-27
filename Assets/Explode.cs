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

    public float maxAngle;

    private int bounces;

    private Rigidbody2D rb;
    private List<Vector3> verticies;

    private Collision2D previousCollision;
    private GameObject LineSprite;
    private bool airborne;

    private float angle;
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
        if (other.transform.gameObject.layer != LayerMask.NameToLayer("Default")) return;

        if (burstProperties.sizeOverVelocity) {
            if (Physics2D.Raycast(transform.position, Vector2.down, 1f, 1 << LayerMask.NameToLayer("Default")).transform != other.transform) {
                Burst(other);
                RemoveFromScene();
                return;
            }
            if (previousCollision != null && other.gameObject.GetInstanceID() == previousCollision.gameObject.GetInstanceID()) { OnCollisionStay2D(other);return; }
            previousCollision = other;
            if (bounces >= burstProperties.bounces) {
                if (Mathf.Abs(GetSlopeAngle() - angle) > maxAngle) {
                    Burst(other);
                    RemoveFromScene();
                    return;
                }
                NewLine(); return; 
            } else {
                if (LineSprite == default(GameObject)) NewLine();
            }
            if (airborne) { bounces++; airborne = false; }
            angle = GetSlopeAngle();
            return;
        }
        Burst(other);
        if (bounces >= burstProperties.bounces) {
            if (burstProperties.sizeOverVelocity) Burst(other);
            RemoveFromScene();
        }

        if (burstProperties.sizeOverVelocity && !airborne) bounces++;
        else if (!burstProperties.sizeOverVelocity) bounces++;
    }

    private void OnTriggerExit2D(Collider2D other) {
        RaycastHit2D ray_down = Physics2D.Raycast(transform.position, Vector2.down, 0.5f, 1 << LayerMask.NameToLayer("Default"));
        if (ray_down != default(RaycastHit2D))
            airborne = false;
        else airborne = true;
    }

    private void OnCollisionStay2D(Collision2D other) {
        if (!burstProperties.sizeOverVelocity) return;
        float newSize = Mathf.Sqrt(Mathf.Pow(rb.velocity.x,2) + Mathf.Pow(rb.velocity.y,2));
        if (newSize < 0.5f) { RemoveFromScene(); return; }
        ContinueLine();
    }
    private void Burst(Collision2D col) {
        GameObject splat = Instantiate(splatter);
        splat.transform.position = new Vector2(transform.position.x, transform.position.y);
        splat.transform.eulerAngles = new Vector3(0,0,Random.Range(0,360));
        splat.GetComponent<SpriteRenderer>().color = unityColor;

        splat.transform.parent = DecalManager.transform;
        splat.GetComponent<EffectProperty>().properties = burstProperties;
    }

    private RaycastHit2D FindRelevantObject(RaycastHit2D[] hits) {
        foreach (RaycastHit2D hit in hits) {
            if (Physics2D.GetIgnoreLayerCollision(LayerMask.NameToLayer("Player Attack"), hit.transform.gameObject.layer)) continue;
            return hit; // Assuming first valid object is closest
        }

        return default(RaycastHit2D);

    }

    private void NewLine() {
        if (Physics2D.Raycast(transform.position, Vector2.down, 1f, 1 << LayerMask.NameToLayer("Effect"))) LineSprite = null;
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
        LineSprite.AddComponent<EffectProperty>();
        LineSprite.GetComponent<EffectProperty>().properties = burstProperties;

        SetSlopeAngle();
        ContinueLine();
    }

    private void ContinueLine() {
        if (LineSprite == null) return;
        if (rb.velocity.magnitude < 0.25f) { Destroy(gameObject); return; }
        
        verticies.Add(new Vector3(transform.position.x, transform.position.y - transform.localScale.y/2, -0.25f));
        if (verticies.Count == 1) return;
        
        LineSprite.transform.localScale = new Vector3(verticies[0].x - verticies[verticies.Count-1].x, 1, 1);
        LineSprite.transform.position = new Vector3((verticies[verticies.Count - 1].x + verticies[0].x)/2, (verticies[verticies.Count - 1].y + verticies[0].y)/2, 0);
    }

    private void RemoveFromScene() {
        Destroy(gameObject);
    }

    private void Update() {
        float maxSpeed = 30;

        if (burstProperties.sizeOverVelocity) {
            if (LineSprite != default(GameObject) && (To360(GetSlopeAngle()) < Mathf.Abs(LineSprite.transform.eulerAngles.z) - 5 || To360(GetSlopeAngle())> Mathf.Abs(LineSprite.transform.eulerAngles.z) + 5)) {
                NewLine();
            } 
            float newSize = Mathf.Sqrt(Mathf.Pow(rb.velocity.x,2) + Mathf.Pow(rb.velocity.y,2)) / maxSpeed;
            //transform.localScale = new Vector3(burstProperties.size*newSize, burstProperties.size*newSize);
        }

        CheckAirborne();
    }

    private void CheckAirborne() {
        if (Physics2D.Raycast(transform.position, Vector2.down, 0.5f) == default(RaycastHit2D)) {
            airborne = true;
        }
    }

    private float GetSlopeAngle() {
        RaycastHit2D[] ray_down = Physics2D.RaycastAll(transform.position,Vector2.down, 1f);
        RaycastHit2D[] ray_dir;

        int modifier_vertical = 1;
        int modifier_horizontal = 1;

        if (rb.velocity.x > 0 && rb.velocity.y < 0 || rb.velocity.x < 0 && rb.velocity.y > 0) {
            ray_dir = Physics2D.RaycastAll(transform.position, Vector2.left, 1f);
            modifier_horizontal = -1;
        } else {
            ray_dir = Physics2D.RaycastAll(transform.position, Vector2.right, 1f);
        }
        
        if (ray_dir.Length == 0 || ray_down.Length == 0) return 0f;
        RaycastHit2D usableRay_horizontal = new RaycastHit2D();
        RaycastHit2D usableRay_vertical = new RaycastHit2D();
        foreach (RaycastHit2D r in ray_dir) {
            if (r.transform.gameObject.layer == LayerMask.NameToLayer("Default")) {usableRay_horizontal = r; break; }
        }
         foreach (RaycastHit2D r in ray_down) {
            if (r.transform.gameObject.layer == LayerMask.NameToLayer("Default")) {usableRay_vertical = r; break; }
        }

        if (usableRay_horizontal == default(RaycastHit2D) || usableRay_vertical == default(RaycastHit2D)) return 0f;

//        Debug.Log(Mathf.Atan2(usableRay_vertical.distance, usableRay_horizontal.distance) * Mathf.Rad2Deg);

        // /Debug.Log(usableRay_horizontal.distance);
        return Mathf.Round(Mathf.Atan2(usableRay_vertical.distance, usableRay_horizontal.distance) * Mathf.Rad2Deg) * modifier_horizontal;
    }

    private void SetSlopeAngle() {
        LineSprite.transform.eulerAngles = new Vector3(0,0, GetSlopeAngle());
    }

    private float To360(float angle) {
        if (angle > 0) return angle;
        return angle + 360;
    }

}
