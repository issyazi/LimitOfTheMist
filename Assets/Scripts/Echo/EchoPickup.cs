using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EchoPickup : MonoBehaviour
{
    public int count; // Количество Echo в этом объекте
    private EchoManager echoManager; // Ссылка на менеджер Echo

    void Start()
    {
        echoManager = FindObjectOfType<EchoManager>();
        if (echoManager == null)
        {
            Debug.LogError("EchoManager not found in scene!");
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (echoManager != null && echoManager.CanAddEcho(count))
            {
                echoManager.AddEcho(count);
                Destroy(gameObject);
            }
        }
    }
}
