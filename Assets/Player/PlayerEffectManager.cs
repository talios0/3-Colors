using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerEffectManager : MonoBehaviour
{

    
public GameObject player;


    void FixedUpdate() {
        RaycastHit2D effect_down = Physics2D.Raycast(player.transform.position,Vector2.down, 2f, LayerMask.NameToLayer("Effect"));
        RaycastHit2D effect_left = Physics2D.Raycast(player.transform.position, Vector2.left, 2f, LayerMask.NameToLayer("Effect"));
        RaycastHit2D effect_right = Physics2D.Raycast(player.transform.position, Vector2.right, 2f, LayerMask.NameToLayer("Effect"));
        //RaycastHit2D effect_up = Physics2D.Raycast(pla)
    }

}
