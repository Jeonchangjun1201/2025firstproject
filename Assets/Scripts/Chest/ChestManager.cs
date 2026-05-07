using System.Collections.Generic;
using UnityEngine;

public class ChestManager : MonoBehaviour
{
    // Inspector에서 상자 오브젝트(Chest 스크립트가 붙은 오브젝트)를 직접 할당
    public List<Chest> chestList = new List<Chest>();

    void Start()
    {
        AssignSpecialItems(); // 게임 시작 시 특수 아이템 배치
    }

    // 특수 아이템(엔딩보석, 비싼보석, 잭팟) 배치 함수
    void AssignSpecialItems()
    {
        // 1. 상자 인덱스 리스트 생성 (0~N-1)
        List<int> indices = new List<int>();
        for (int i = 0; i < chestList.Count; i++)
            indices.Add(i);

        // 2. 인덱스 랜덤 셔플
        for (int i = 0; i < indices.Count; i++)
        {
            int j = Random.Range(i, indices.Count);
            int temp = indices[i];
            indices[i] = indices[j];
            indices[j] = temp;
        }

        // 3. 엔딩보석, 비싼보석1~3, 잭팟 상자 인덱스 지정
        int endingGemIdx = indices[0];
        int expensiveGem1Idx = indices[1];
        int expensiveGem2Idx = indices[2];
        int expensiveGem3Idx = indices[3];
        int jackpotIdx = indices[4];

        // 4. 각 상자에 특수 아이템 할당
        chestList[endingGemIdx].gemType = GemType.EndingGem;
        chestList[expensiveGem1Idx].gemType = GemType.ExpensiveGem1;
        chestList[expensiveGem2Idx].gemType = GemType.ExpensiveGem2;
        chestList[expensiveGem3Idx].gemType = GemType.ExpensiveGem3;
        chestList[jackpotIdx].isJackpot = true;

        // 디버그 로그로 어떤 상자에 어떤 아이템이 들어있는지 출력
        Debug.Log($"엔딩보석: {endingGemIdx}, 비싼보석1: {expensiveGem1Idx}, 비싼보석2: {expensiveGem2Idx}, 비싼보석3: {expensiveGem3Idx}, 잭팟: {jackpotIdx}");
    }
}
