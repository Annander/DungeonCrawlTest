using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class UIDropTarget : MonoBehaviour,
    IDropHandler,
    IPointerEnterHandler,
    IPointerExitHandler
{
    [SerializeField]
    private Color defaultColor = Color.white;

    [SerializeField]
    private Color hoverColor = Color.white;

    private Image image;

    private void Awake()
    {
        image = GetComponent<Image>();
        image.color = defaultColor;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if(eventData.dragging)
            image.color = hoverColor;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        image.color = defaultColor;
    }

    public void OnDrop(PointerEventData eventData)
    {
        image.color = defaultColor;

        /*
        var droppedItem = eventData.pointerDrag.GetComponent<UISlot>();
        var local = GetComponent<UISlot>();

        if (droppedItem.SlotType == local.SlotType || local.SlotType == StashSlotType.Stash)
            OnDropEvent?.Invoke(droppedItem, local);
        */
    }
}