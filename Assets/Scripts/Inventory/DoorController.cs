using UnityEngine;
using UnityEngine.UI;

public class DoorController : MonoBehaviour
{
    [SerializeField] private Inventory inventory; // Ссылка на Inventory
    [SerializeField] private GameObject interactText; // Текст "Открыть"
    [SerializeField] private GameObject doorObject; // Объект двери

    void Start()
    {
        if (inventory == null)
        {
            Debug.LogError("DoorController: Inventory не привязан в инспекторе!");
            enabled = false;
            return;
        }
        if (interactText == null)
        {
            Debug.LogError("DoorController: InteractText не привязан в инспекторе!");
            enabled = false;
            return;
        }
        if (doorObject == null)
        {
            Debug.LogError("DoorController: DoorObject не привязан в инспекторе!");
            enabled = false;
            return;
        }

        interactText.SetActive(false); // Скрываем текст по умолчанию
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player")) // Предполагаем, что у игрока есть тег "Player"
        {
            if (inventory.HasKey())
            {
                interactText.SetActive(true); // Показываем текст "Открыть"
            }
        }
    }

    void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (inventory.HasKey() && Input.GetKeyDown(KeyCode.E)) // Взаимодействие по клавише E
            {
                OpenDoor();
            }
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            interactText.SetActive(false); // Скрываем текст при выходе
        }
    }

    void OpenDoor()
    {
        doorObject.SetActive(false); // Отключаем дверь
        interactText.SetActive(false); // Скрываем текст после открытия
        Debug.Log("Door opened with key!");

        // Поиск и удаление ключа (если хочешь использовать ключ)
        int keyIndex = inventory.items.FindIndex(item => item.id == 3 && item.count > 0);
        if (keyIndex != -1)
        {
            inventory.DeleteItem(keyIndex); // Удаляем ключ
        }
    }
}