using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DecalManager : MonoBehaviour
{
    public int maxDecals;

    void FixedUpdate() {
        int childCount = transform.childCount;
        while (childCount > maxDecals) {
            Destroy(transform.GetChild(0).gameObject);
            childCount--;
        }
    }


}
