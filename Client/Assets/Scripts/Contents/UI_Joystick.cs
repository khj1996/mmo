using UnityEngine;
using UnityEngine.EventSystems;

public class UI_Joystick : MonoBehaviour, IDragHandler, IPointerUpHandler, IPointerDownHandler
{
    public RectTransform joystickBackground;
    public RectTransform joystickHandle;
    public CanvasGroup canvasGroup;
    public float joystickRadius = 100f;
    public Vector2 inputDirection;

    private void Start()
    {
        joystickHandle.anchoredPosition = Vector2.zero;
        inputDirection = Vector2.zero;
        canvasGroup.alpha = 0.3f;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        canvasGroup.alpha = 1;
        joystickHandle.anchoredPosition = Vector2.zero;
        inputDirection = Vector2.zero;
    }

    public void OnDrag(PointerEventData eventData)
    {
        Vector2 direction = eventData.position - joystickBackground.anchoredPosition;
        inputDirection = direction.normalized;
        joystickHandle.anchoredPosition = Vector2.ClampMagnitude(direction, joystickRadius);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        canvasGroup.alpha = 0.3f;
        joystickHandle.anchoredPosition = Vector2.zero;
        inputDirection = Vector2.zero;
    }
}