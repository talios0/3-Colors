using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerEffectManager : MonoBehaviour {
    public GameObject player;

    [Header("Effect Properties")]
    public float walkSpeedMultiplier;
    public float hoverHeightModifier;
    private bool[] interactingOrange = new bool[2];
    private bool[] interactingGreen = new bool[2];
    private bool[] interactingBlue = new bool[2];

    void FixedUpdate() {
        //CheckEffect(Vector2.up);
        CheckEffect(Vector2.down);
        //CheckEffect(Vector2.left);
        //CheckEffect(Vector2.right);
    }

    private void CheckEffect(Vector2 direction) {
        RaycastHit2D[] rays = Physics2D.RaycastAll(player.transform.position, direction, 2f, 1 << LayerMask.NameToLayer("Effect"));

        interactingOrange[1] = interactingOrange[0];
        interactingBlue[1] = interactingBlue[0];
        interactingGreen[1] = interactingGreen[0];

        interactingOrange[0] = false;
        interactingBlue[0] = false;
        interactingGreen[0] = false;

        foreach (RaycastHit2D r in rays) {
            if (r.transform.GetComponent<EffectProperty>().properties.color == Colors.ORANGE && !interactingOrange[0]) {
                // ORANGE EFFECT
                if (!interactingOrange[1]) {
                    player.GetComponent<Movement>().speed *= walkSpeedMultiplier;
                }
                // ---
                interactingOrange[0] = true;
            }
            else if (r.transform.GetComponent<EffectProperty>().properties.color == Colors.BLUE && !interactingBlue[0]) {
                // BLUE EFFECT
                if (!interactingBlue[1]) {
                    //player.GetComponent<Hover>().
                }
                // ---
                interactingBlue[0] = true;
            }
            else if (r.transform.GetComponent<EffectProperty>().properties.color == Colors.GREEN && !interactingGreen[0]) {
                // GREEN EFFECT
                if (!interactingGreen[1]) {
                    player.GetComponent<Hover>().minDistance = 2;
                    player.GetComponent<Hover>().maxDistance = 3;
                    player.GetComponent<Hover>().gapForceMultiplier = 5;
                    player.GetComponent<Movement>().distanceToStartJump = 3f;
                }
                // ---
                interactingGreen[0] = true;
            }
        }

        if (!interactingOrange[0] && interactingOrange[1]) {
            player.GetComponent<Movement>().speed /= walkSpeedMultiplier;
        }
        if (!interactingBlue[0] && interactingBlue[1]) {
           
        }
        if (!interactingGreen[0] && interactingGreen[1]) {
            player.GetComponent<Hover>().minDistance = 0.85f;
            player.GetComponent<Hover>().maxDistance = 1.15f;
            player.GetComponent<Hover>().gapForceMultiplier = 1.28f;
            player.GetComponent<Movement>().distanceToStartJump = 1.5f;
        }
    }

}
