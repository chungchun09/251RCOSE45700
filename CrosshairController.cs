using UnityEngine;
using UnityEngine.UI;

public class CrosshairController : MonoBehaviour {
    public RectTransform crosshairUI;  // UI Image 오브젝트
    public Canvas canvas;

    void Start() {
        Cursor.visible = false;
    }


    void Update() {
        Vector2 mousePos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvas.transform as RectTransform,
            Input.mousePosition,
            canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : canvas.worldCamera,
            out mousePos
        );

        crosshairUI.anchoredPosition = mousePos;
    }
}
