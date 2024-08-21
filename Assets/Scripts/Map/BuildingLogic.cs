using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingLogic : MonoBehaviour
{
    Renderer roof;

    private void Awake()
    {
        roof = GetComponent<Renderer>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision == null) return;

        if (collision.gameObject == GameplayManager.instance.Player.gameObject) roof.enabled = false;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision == null) return;

        if (collision.gameObject == GameplayManager.instance.Player.gameObject) roof.enabled = true;
    }
}
