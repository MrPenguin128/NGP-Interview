using UnityEngine;
using TMPro;
using InventorySystem;
using UnityEngine.UI;
using UnityEngine.InputSystem.UI;

public class ItemTooltipUI : Singleton<ItemTooltipUI>
{
    [Header("References")]
    [SerializeField] RectTransform rectTransform;
    [SerializeField] InputSystemUIInputModule inputModule;
    [SerializeField] Image itemIcon;
    [SerializeField] TextMeshProUGUI itemName;
    [SerializeField] TextMeshProUGUI description;

    [Header("Settings")]
    [SerializeField] Vector2 mouseOffset = new Vector2(16f, -16f);
    Canvas canvas;

    private void Awake()
    {
        canvas = GetComponentInParent<Canvas>();
        Hide();
    }

    private void Update()
    {
        if (!rectTransform.gameObject.activeSelf) return;

        FollowMouse();
    }

    public void Show(ItemObject item)
    {
        itemIcon.sprite = item.Icon;
        itemIcon.enabled = item.Icon != null;

        itemName.text = item.DisplayName;
        if (item is EquipmentObject equipment)
            description.text = ItemDescriptionParser.Parse(equipment.Description, equipment.modifiers);
        else
            description.text = item.Description;

        rectTransform.gameObject.SetActive(true);
        LayoutRebuilder.ForceRebuildLayoutImmediate(rectTransform);
    }

    public void Hide()
    {
        rectTransform.gameObject.SetActive(false);
    }

    private void FollowMouse()
    {
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvas.transform as RectTransform,
            inputModule.point.action.ReadValue<Vector2>(),
            canvas.renderMode == RenderMode.ScreenSpaceOverlay
                ? null
                : canvas.worldCamera,
            out Vector2 localPoint
        );

        Vector2 position = localPoint + mouseOffset;

        Vector2 size = rectTransform.sizeDelta;
        RectTransform canvasRect = canvas.transform as RectTransform;


        float minX = canvasRect.rect.min.x;
        float maxX = canvasRect.rect.max.x - size.x;

        float maxY = canvasRect.rect.max.y;
        float minY = canvasRect.rect.min.y + size.y;

        position.x = Mathf.Clamp(position.x, minX, maxX);
        position.y = Mathf.Clamp(position.y, minY, maxY);

        rectTransform.localPosition = position;
    }
}
