using UnityEngine;
using UnityEngine.EventSystems;

public class InventorySlotHandler : MonoBehaviour, IPointerClickHandler
{
    public void OnPointerClick(PointerEventData eventData)
    {
        // Вручную устанавливаем текущий объект как selected для EventSystem
        EventSystem.current.SetSelectedGameObject(gameObject);

        Inventory inventory = FindObjectOfType<Inventory>();
        if (inventory != null)
        {
            inventory.SelectObject(eventData.button);
        }
    }
}