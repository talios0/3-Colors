using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ColorSelector : MonoBehaviour
{

    public GameObject[] colors;
    public Image[] highlightColors;
    private Colors selectedColor;
    private bool colorShiftPanelActive;

    // Start is called before the first frame update
    void Start()
    {
        selectedColor = Colors.NONE;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButton("Change Color") && !colorShiftPanelActive) {
            ActivateColorShiftPanel();
            OpenPanel();
        }
        if (colorShiftPanelActive)  {UpdatePanel(); UpdateHighlight(); }
    }

    private void ActivateColorShiftPanel() {
        colorShiftPanelActive = true;
    }

    private void OpenPanel() {
        switch (selectedColor) {
            case Colors.NONE:
                selectedColor = Colors.ORANGE;
                break;
            case Colors.ORANGE:
                break;
            case Colors.GREEN:
                break;
            case Colors.BLUE:
                break;
        }
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
        if (!Input.GetButtonDown("Horizontal")) return;
        if (Input.GetAxisRaw("Horizontal") == -1) {
            index = index-1 < 0 ? colors.Length - 1 : index - 1;
        }
        else if (Input.GetAxisRaw("Horizontal") == 1) {
            index = index + 1 > colors.Length - 1 ? 0 : index + 1;
        }
        selectedColor = (Colors) index;
    }
}
