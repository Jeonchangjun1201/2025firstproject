using UnityEngine;
using System.Collections.Generic;

public class GemChestAssigner : MonoBehaviour
{
    public Chest[] gemChests; // 씬에 배치한 보석 상자 4개 (chestType=Gem)
    public GameObject[] gemPrefabs; // 4종류 보석 프리팹

    void Start()
    {
        AssignGems();
    }

    void AssignGems()
    {
        List<int> indices = new List<int> { 0, 1, 2, 3 };
        // Fisher-Yates 셔플
        for (int i = 0; i < indices.Count; i++)
        {
            int rand = Random.Range(i, indices.Count);
            int temp = indices[i];
            indices[i] = indices[rand];
            indices[rand] = temp;
        }
        for (int i = 0; i < gemChests.Length; i++)
        {
            gemChests[i].assignedGemPrefab = gemPrefabs[indices[i]];
        }
    }
}