﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraTracker : MonoBehaviour
{
    [Range(0, 5)]
    public float maxValue;
    public float changeAmount;

    private float position;

    private void FixedUpdate() {
        position += Input.GetAxisRaw("Horizontal") * changeAmount;
        position = Mathf.Clamp(position, -maxValue, maxValue);
    }

    public float GetPosition() {
        return position;
    }

}
