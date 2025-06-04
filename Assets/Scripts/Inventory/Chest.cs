using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Chest : MonoBehaviour
{
    public GameObject promptUI;
    public TextMeshProUGUI promptText;
    public float interactionRange = 2f;
    public GameObject[] itemPrefabs;
    public Transform spawnPoint;
    private bool isPlayerInRange = false;
    private bool isOpened = false;
    private Inventory inventory;
    private Transform player;
    private Animator animator;

    private readonly int[] itemIds = { 1, 2, 3 };
    private readonly float[] probabilities = { 0.6f, 0.2f, 0.1f };
    private readonly int itemCount = 3;

    void Start()
    {
        inventory = FindObjectOfType<Inventory>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
        animator = GetComponent<Animator>();
        if (animator == null)
        {
            Debug.LogError("Animator component not found on Chest!");
        }
        promptUI.SetActive(false);
        promptText.text = "E — Открыть";
    }

    void Update()
    {
        float distance = Vector3.Distance(transform.position, player.position);
        isPlayerInRange = distance <= interactionRange;

        promptUI.SetActive(isPlayerInRange && !isOpened);

        if (isPlayerInRange && !isOpened && Input.GetKeyDown(KeyCode.E))
        {
            Debug.Log("Opening chest...");
            OpenChest();
        }
    }

    void OpenChest()
    {
        if (!isOpened)
        {
            isOpened = true;
            promptUI.SetActive(false);
            if (animator != null)
            {
                animator.SetBool("IsOpening", true);
            }
            Debug.Log("Spawning items...");

            for (int i = 0; i < itemCount; i++)
            {
                int selectedId = GetRandomItemId();
                Debug.Log($"Spawning item with ID: {selectedId}");
                GameObject item = Instantiate(itemPrefabs[selectedId - 1], spawnPoint.position, Quaternion.identity);
                ItemPickup pickup = item.GetComponent<ItemPickup>();
                if (pickup != null)
                {
                    pickup.itemId = selectedId;
                    pickup.count = Random.Range(1, 10);
                    Debug.Log($"Item spawned at {spawnPoint.position}");
                }
                else
                {
                    Debug.LogError("ItemPickup component not found on prefab!");
                }
            }
        }
    }

    int GetRandomItemId()
    {
        float random = Random.value;
        float cumulative = 0f;
        for (int i = 0; i < itemIds.Length; i++)
        {
            cumulative += probabilities[i];
            if (random <= cumulative)
            {
                return itemIds[i];
            }
        }
        return itemIds[0];
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, interactionRange);
    }
}