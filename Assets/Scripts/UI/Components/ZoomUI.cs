using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ZoomUI : MonoBehaviour, IDragHandler {
    [SerializeField] Canvas uiCanvas;
    [SerializeField] float zoomSpeed = 1f;
    [SerializeField] float minZoomRate = 1f;
    [SerializeField] float maxZoomRate = 10f;

    RectTransform targetContent;
    CanvasScaler canvasScaler;

    float CurrentZoomScale => targetContent.localScale.x;

    bool ShouldScaleDragMove =>
        canvasScaler != null &&
        canvasScaler.IsActive() &&
        canvasScaler.uiScaleMode == CanvasScaler.ScaleMode.ScaleWithScreenSize;

    void Awake() {
        targetContent = GetComponent<RectTransform>();
        canvasScaler = uiCanvas.GetComponent<CanvasScaler>();
    }

    void Update() {
        var scroll = Input.mouseScrollDelta.y;
        ScrollToZoomMap(Input.mousePosition, scroll);
    }

    /// <summary>
    /// Adjust to keep the mouse's position at the same location on the content even after zooming.
    /// </summary>
    /// <param name="mousePosition">Current mouse position.</param>
    /// <param name="scroll">Mouse scroll delta.</param>
    public void ScrollToZoomMap(Vector2 mousePosition, float scroll) {
        GetLocalPointInRectangle(mousePosition, out var beforeZoomLocalPosition);

        var afterZoomScale = CurrentZoomScale + scroll * zoomSpeed;
        afterZoomScale = Mathf.Clamp(afterZoomScale, minZoomRate, maxZoomRate);
        DoZoom(afterZoomScale);

        GetLocalPointInRectangle(mousePosition, out var afterZoomLocalPosition);

        var positionDiff = afterZoomLocalPosition - beforeZoomLocalPosition;
        var scaledPositionDiff = positionDiff * afterZoomScale;
        var newAnchoredPosition = targetContent.anchoredPosition + scaledPositionDiff;

        targetContent.anchoredPosition = newAnchoredPosition;
    }

    /// <summary>
    /// Move the window according to the amount of drag.
    /// Automatically called by IDragHandler.
    /// </summary>
    /// <param name="eventData"></param>
    public void OnDrag(PointerEventData eventData) {
        var dragMoveDelta = eventData.delta;

        if (ShouldScaleDragMove) {
            var dragMoveScale = canvasScaler.referenceResolution.x / Screen.width;
            dragMoveDelta *= dragMoveScale;
        }

        targetContent.anchoredPosition += dragMoveDelta;
    }

    void DoZoom(float zoomScale) {
        targetContent.localScale = Vector3.one * zoomScale;
    }

    void GetLocalPointInRectangle(Vector2 mousePosition, out Vector2 localPosition) {
        var targetCamera = uiCanvas.renderMode switch {
            RenderMode.ScreenSpaceCamera => uiCanvas.worldCamera,
            RenderMode.ScreenSpaceOverlay => null,
            _ => throw new System.NotSupportedException(),
        };

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            targetContent, mousePosition, targetCamera, out localPosition);
    }
}
