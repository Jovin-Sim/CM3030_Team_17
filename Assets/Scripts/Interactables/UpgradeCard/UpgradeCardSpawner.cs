using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgradeCardSpawner : MonoBehaviour
{
    public GameObject upgradeCardPrefab;
    public Transform spawnPoint;

    void Start()
    {
        SpawnUpgradeCard();
    }

    public void SpawnUpgradeCard()
    {
        Instantiate(upgradeCardPrefab, spawnPoint.position, spawnPoint.rotation);
    }
}

