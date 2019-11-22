using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StarRotate : MonoBehaviour
{
       private void FixedUpdate() {
           transform.Rotate(0, 0f, BackgroundManager.starRotateSpeed, Space.Self);
       }
}
