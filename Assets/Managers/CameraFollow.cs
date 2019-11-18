using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{

    public Transform player;
    public CameraTracker tracker;

    public float xOffset;
    public float yOffset;
    public bool followY;

    public float incrementTime;

    [Range(0, 5)]
    public float shiftValue; // Value needed on the x position of the tracker to change the camera direction
    [Range(0,5)]
    public float shiftMin;

    private ShiftDirection shift;
    

    // Start is called before the first frame update
    void Start()
    {
        shift = ShiftDirection.NONE;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Vector3 cameraPos = new Vector3();
        cameraPos.z = transform.position.z;
        cameraPos.y = player.position.y + yOffset;
        if (tracker.GetPosition() >= shiftValue) {
            cameraPos.x = Mathf.Lerp(transform.position.x, player.position.x + xOffset, incrementTime);
            shift = ShiftDirection.RIGHT;
        } else if (tracker.GetPosition() <= -shiftValue) {
            cameraPos.x = Mathf.Lerp(transform.position.x, player.position.x - xOffset, incrementTime);
            shift = ShiftDirection.LEFT;
        }
        else {
            if (shift == ShiftDirection.RIGHT) {
                cameraPos.x = Mathf.Lerp(transform.position.x, player.position.x + xOffset, incrementTime);
            } else if (shift == ShiftDirection.LEFT) {
                cameraPos.x = Mathf.Lerp(transform.position.x, player.position.x - xOffset, incrementTime);
            }
            else {
                cameraPos.x = Mathf.Lerp(transform.position.x, player.position.x, incrementTime);
            }

        }

        if (!followY) cameraPos.y = yOffset;

        transform.position = cameraPos;

        
    }
}
