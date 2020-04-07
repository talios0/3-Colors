using UnityEngine;

public class BurstDebug : MonoBehaviour
{
    public BurstProperties burstProperties;

    private void OnDrawGizmos()
    {
        if (!burstProperties.showRays) return;
        Vector2 bottomCenter = new Vector2(transform.position.x, transform.position.y - transform.localScale.y / 2);

        Gizmos.color = Color.red;
        Gizmos.DrawLine(bottomCenter, bottomCenter + Vector2.left * (burstProperties.distanceFromGround + transform.localScale.x/2));
        Gizmos.DrawLine(bottomCenter, bottomCenter + Vector2.down * burstProperties.distanceFromGround);
        Gizmos.DrawLine(bottomCenter, bottomCenter + Vector2.right * (burstProperties.distanceFromGround + transform.localScale.x/2));
    }
}