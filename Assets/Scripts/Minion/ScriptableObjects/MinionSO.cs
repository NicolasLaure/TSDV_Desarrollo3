using System;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

namespace Minion.ScriptableObjects
{
    [Serializable]
    public class ChargeAttack
    {
        public float duration;
        public float length;
        public AnimationCurve chargeCurve;
        public float delayAfterLine;
    }

    [Serializable]
    public class Attack
    {
        public int damage;
        public float speed;
    }

    [Serializable]
    public class Idle
    {
        public float minIdleTime;
        public float maxIdleTime;
    }

    [Serializable]
    public class Move
    {
        public float minMovingTime;
        public float maxMovingTime;
        public float speed;
        public float minDistance;
    }
    
    [CreateAssetMenu(menuName = "Minions/Config")]
    public class MinionSO : ScriptableObject
    {
        public GameObject minionPrefab;
        public ChargeAttack chargeAttackData;
        public Attack attackData;
        public Idle idleData;
        public Move moveData;

        public float GetRandomIdleTime()
        {
            return Random.Range(idleData.minIdleTime, idleData.maxIdleTime);
        }
        
        public float GetRandomMoveTime()
        {
            return Random.Range(moveData.minMovingTime, moveData.maxMovingTime);
        }
    }
}
