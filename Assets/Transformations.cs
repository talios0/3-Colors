using UnityEngine;

public class Transformations
{

    public static float GetSlopeAngle(Transform t, Rigidbody2D rb)
    {
        return GetSlopeAngle(t, rb, Vector3.zero);
    }

    public static float GetSlopeAngle(Transform t, Rigidbody2D rb, Vector3 offset)
    {
        RaycastHit2D[] ray_down = Physics2D.RaycastAll(t.position + offset, Vector2.down, 2f, 1 << LayerMask.NameToLayer("Default"));
        RaycastHit2D[] ray_dir;

        int modifier_vertical = 1;
        int modifier_horizontal = 1;

        if (rb.velocity.x > 0 && rb.velocity.y < 0 || rb.velocity.x < 0 && rb.velocity.y > 0)
        {
            ray_dir = Physics2D.RaycastAll(t.position + offset, Vector2.left, 0.5f, 1 << LayerMask.NameToLayer("Default"));
            modifier_horizontal = -1;
        }
        else
        {
            ray_dir = Physics2D.RaycastAll(t.position + offset, Vector2.right, 0.5f, 1 << LayerMask.NameToLayer("Default"));
        }

        if (ray_dir.Length == 0 || ray_down.Length == 0) return 0f;
        RaycastHit2D usableRay_horizontal = new RaycastHit2D();
        RaycastHit2D usableRay_vertical = new RaycastHit2D();
        if (ray_dir.Length != 0) usableRay_horizontal = ray_dir[0];
        if (ray_down.Length != 0) usableRay_vertical = ray_down[0];

        if (usableRay_vertical == default) return 0;
        else if (usableRay_horizontal == default) return Mathf.PI / 2;

        if (usableRay_horizontal == default(RaycastHit2D) || usableRay_vertical == default(RaycastHit2D)) return 0f;

        return Mathf.Round(Mathf.Atan2(usableRay_vertical.distance, usableRay_horizontal.distance) * Mathf.Rad2Deg) * modifier_horizontal;
    }

    public static float GetSlopeAngle(Transform t, RaycastHit2D leftRay, RaycastHit2D downRay, RaycastHit2D rightRay)
    {
        if (leftRay == default)
        {
            if (rightRay == default)
            {
                if (downRay != default) return 0;
            }
            else
            {
                if (downRay != default) return Mathf.Round(Mathf.Atan2(downRay.distance, rightRay.distance) * Mathf.Rad2Deg);
            }
        }
        else
        {
            if (rightRay == default)
            {
                if (downRay != default) return Mathf.Round(Mathf.Atan2(downRay.distance, leftRay.distance) * Mathf.Rad2Deg) * -1;
            }
            else
            {
                if (leftRay.distance < rightRay.distance) return Mathf.Round(Mathf.Atan2(downRay.distance, leftRay.distance) * Mathf.Rad2Deg) * -1;
                else return Mathf.Round(Mathf.Atan2(downRay.distance, rightRay.distance) * Mathf.Rad2Deg);
            }
        }

        return 0;
    }

    public static float To360(float angle)
    {
        if (angle >= 0 && angle <= 360) return angle;
        if (angle > 360) { while (angle > 360) { angle -= 360; } return angle; }
        return angle + 360;
    }
}