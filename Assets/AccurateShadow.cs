using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AccurateShadow : MonoBehaviour
{
    public GameObject foreground;
    public GameObject background;

    public Vector2 offset;

    private void LateUpdate() {
        // parent world position: transform.position
        // background world position: background.transform.position

        background.transform.position = new Vector2(transform.position.x + offset.x, transform.position.y + offset.y);
    }
}
