using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActiveWeapon : MonoBehaviour
{
    public static ActiveWeapon Instance {get; private set; }
    [SerializeField] private Sword sword;

    private void Awake() {
        Instance = this;
        GameObject.Find("SwordVisual").GetComponent<Renderer>().enabled = false;
        GameObject.Find("SwordVisual").GetComponent<CapsuleCollider2D>().enabled = false;
    }
    

    public Sword GetActiveWeapon() {
        return sword; //дописать разные оружия
    }
}
