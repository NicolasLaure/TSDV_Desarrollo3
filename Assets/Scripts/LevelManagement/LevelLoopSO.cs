using System;
using UnityEngine;

namespace LevelManagement
{
    [Serializable]
    public class ObstacleData
    {
        public float obstaclesDuration;
        public float obstacleCooldown;
        public float minDistance;
    }

    [Serializable]
    public class RoadData
    {
        public Vector3 roadVelocity;
    }

    [Serializable]
    public class BossData
    {
        public int hitPointsToNextPhase;
        public FallingAttackData fallingAttackData;
    }

    [Serializable]
    public class FallingAttackData
    {
        public float spawnRadiusFromPlayer;
        public float timeBetweenSpawns;
        public float spawnCooldown;
        public int spawnQuantity;
        public float acceleration;
    }
    
    [CreateAssetMenu(menuName = "Level Loop Config", fileName = "LevelLoopConfig", order = 0)]
    public class LevelLoopSO : ScriptableObject
    {
        public ObstacleData obstacleData;
        public RoadData roadData;
        public BossData bossData;
    }
}
