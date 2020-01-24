using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerEffectManager : MonoBehaviour
{

    private Colors interacting;
public GameObject player;


    void FixedUpdate() {
        RaycastHit2D effect_down = Physics2D.Raycast(player.transform.position,Vector2.down, 2f, 1 << LayerMask.NameToLayer("Effect"));
        RaycastHit2D effect_left = Physics2D.Raycast(player.transform.position, Vector2.left, 2f, 1 << LayerMask.NameToLayer("Effect"));
        RaycastHit2D effect_right = Physics2D.Raycast(player.transform.position, Vector2.right, 2f, 1 << LayerMask.NameToLayer("Effect"));
        RaycastHit2D effect_up = Physics2D.Raycast(player.transform.position, Vector2.up, 2f, 1 << LayerMask.NameToLayer("Effect"));

//        Debug.Log(effect_down.collider.name);

        if (effect_down != default(RaycastHit2D)) {
            if (effect_down.transform.GetComponent<EffectProperty>().properties.color == Colors.ORANGE && interacting != Colors.ORANGE) {
                    player.GetComponent<Movement>().speed = player.GetComponent<Movement>().speed * 5;
                    interacting = Colors.ORANGE;
            } 
        }
        if (effect_down.transform.GetComponent<EffectProperty>().properties.color != Colors.ORANGE && interacting == Colors.ORANGE) {
            player.GetComponent<Movement>().speed = player.GetComponent<Movement>().speed / 1.25f; interacting = Colors.NONE;
        } 
    }

}
