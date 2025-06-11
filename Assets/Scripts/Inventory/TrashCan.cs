using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class TrashCan : MonoBehaviour
{
    private Inventory inventory;
    private Button trashCanButton;

    public void Start()
    {
        trashCanButton = GetComponent<Button>();
        if (inventory == null)
        {
            inventory = FindObjectOfType<Inventory>();
        }
        AddClickHandler();
        SetupDropEvent();
    }

    private void AddClickHandler()
    {
        EventTrigger trigger = trashCanButton.gameObject.GetComponent<EventTrigger>() ?? trashCanButton.gameObject.AddComponent<EventTrigger>();
        EventTrigger.Entry pointerDownEntry = new EventTrigger.Entry { eventID = EventTriggerType.PointerDown };
        pointerDownEntry.callback.AddListener((eventData) => OnButtonPointerDown());
        trigger.triggers.Add(pointerDownEntry);

        trashCanButton.onClick.AddListener(DeleteSelectedItem);
    }

    private void OnButtonPointerDown()
    {
        if (inventory != null && GameInput.Instance != null)
        {
            Debug.Log("Клик по кнопке UI: временно отключаем Combat");
            GameInput.Instance.SetCombatEnabled(false);
        }
    }

    private void SetupDropEvent()
    {
        EventTrigger trigger = trashCanButton.gameObject.GetComponent<EventTrigger>() ?? trashCanButton.gameObject.AddComponent<EventTrigger>();
        EventTrigger.Entry dropEntry = new EventTrigger.Entry { eventID = EventTriggerType.Drop };
        dropEntry.callback.AddListener((eventData) => OnDrop());
        trigger.triggers.Add(dropEntry);
    }

    private void OnDrop()
    {
        if (inventory != null && inventory.currentID != -1)
        {
            inventory.DeleteItem(inventory.currentID);
            inventory.currentID = -1;
            inventory.movingObject.gameObject.SetActive(false);
        }
    }

    public void DeleteSelectedItem()
    {
        if (inventory != null && inventory.currentID != -1)
        {
            inventory.DeleteItem(inventory.currentID);
        }
    }
}