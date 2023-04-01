using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class UIDraggable : MonoBehaviour,
    IBeginDragHandler,
    IDragHandler,
    IEndDragHandler
{
    private Image[] images;

    private Vector2 offset;

    private Vector3 origin;

    private void Awake()
    {
        images = GetComponentsInChildren<Image>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        var position = new Vector3(eventData.position.x, eventData.position.y, 0);
        offset = transform.position - position;

        origin = transform.position;

        Cursor.visible = false;

        for(int i = 0; i < images.Length; ++i)
            images[i].raycastTarget = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        transform.position = eventData.position + offset;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        Cursor.visible = true;

        for (int i = 0; i < images.Length; ++i)
            images[i].raycastTarget = true;

        transform.position = origin;
    }
}