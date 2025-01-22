using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BossStage", menuName = "Boss/BossStage")]
public class BossStage : ScriptableObject
{
    public string stageName;
    public int healthThreshold;

    public BossAttacks[] stageAttacks;

    [System.Serializable]
    public struct StageAction
    {
        public string actionName;
        public int damage;
        public float probability;
        public bool isSpecialMove;
    }

    public StageAction[] stageActions;

    public int GetRandomActionIndex()
    {
        float totalWeight = 0;
        foreach (StageAction action in stageActions)
        {
            totalWeight += action.probability;
        }

        float randomValue = Random.Range(0, totalWeight);
        float cumulativeWeight = 0;

        for (int i = 0; i < stageActions.Length; i++)
        {
            cumulativeWeight += stageActions[i].probability;
            if (randomValue <= cumulativeWeight)
                return i;
        }
        return 0;
    }
}
