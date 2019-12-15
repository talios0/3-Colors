using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ColorSelector : MonoBehaviour
{

    public GameObject[] colors;
    public Image[] highlightColors;
    private static Colors selectedColor;

    // Start is called before the first frame update
    void Start()
    {
        selectedColor = Colors.ORANGE;
    }

    // Update is called once per frame
    void Update()
    {
        UpdatePanel();
        UpdateHighlight();
    }

    private void UpdateHighlight() {
        highlightColors[(int) selectedColor].enabled = true;
        int skip = (int) selectedColor;
        for (int i = 0; i < highlightColors.Length; i++) {
            if (i == skip) continue;
            highlightColors[i].enabled = false;
        }
    }

    private void UpdatePanel() {
        int index = (int) selectedColor;
        if (Input.GetAxisRaw("Change Color") == -1) {
            index = index-1 < 0 ? colors.Length - 1 : index - 1;
        }
        else if (Input.GetAxisRaw("Change Color") == 1) {
            index = index + 1 > colors.Length - 1 ? 0 : index + 1;
        }

        if (Input.GetAxisRaw("Select Orange") != 0) index = 0;
        else if (Input.GetAxisRaw("Select Blue") != 0) index = 1;
        else if (Input.GetAxisRaw("Select Green") != 0) index = 2;

        if (index == (int)selectedColor) return;
        selectedColor = (Colors) index;

    }

    public static Colors GetColor() {
        return selectedColor;
    }
}
