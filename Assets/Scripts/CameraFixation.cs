using UnityEngine;

public class CameraFixation : MonoBehaviour
{
    private Transform player;
    public float zOffset = -10f;
    public float pixelsPerUnit = 100f; // Установите PPU, как в ваших спрайтах

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        if (player == null)
        {
            // Debug.LogError("Player not found! Check the 'Player' tag.");
        }
        UpdateCameraPosition();
    }

    void Update()
    {
        UpdateCameraPosition();
    }

    void UpdateCameraPosition()
    {
        if (player != null)
        {
            Vector3 temp = player.position;
            temp.z = zOffset;
            transform.position = temp;
            // Debug.Log($"Camera Position: {transform.position}, Player Position: {player.position}");
        }
        else
        {
            // Debug.LogWarning("Player is null!");
        }
    }
}