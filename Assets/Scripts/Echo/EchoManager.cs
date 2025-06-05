using UnityEngine;
using TMPro;

public class EchoManager : MonoBehaviour
{
    public TextMeshProUGUI echoCountText; // Текст для отображения количества
    private int currentEchoCount = 0; // Текущее количество Echo
    private const int maxEchoCount = 200; // Максимальное количество Echo

    void Start()
    {
        UpdateEchoUI();
    }

    public bool CanAddEcho(int count)
    {
        return currentEchoCount + count <= maxEchoCount;
    }

    public void AddEcho(int count)
    {
        currentEchoCount += count;
        if (currentEchoCount > maxEchoCount)
        {
            currentEchoCount = maxEchoCount;
        }
        UpdateEchoUI();
    }

    void UpdateEchoUI()
    {
        if (echoCountText != null)
        {
            echoCountText.text = currentEchoCount.ToString();
        }
    }
}