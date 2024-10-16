using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Events;
using Events.ScriptableObjects;
using Minion.Controllers;
using Minion.ScriptableObjects;
using UnityEngine;
using UnityEngine.Serialization;

namespace Minion.Manager
{
    public class MinionManager : MonoBehaviour
    {
        [SerializeField] private MinionSpawnerSO minionSpawnerConfig;
        [SerializeField] private MinionsManagerSO minionManagerConfig;
        [SerializeField] private GameObject player;
        
        [Header("Events")]
        [SerializeField] private GameObjectEventChannelSO onMinionDeletedEvent;
        [SerializeField] private VoidEventChannelSO onAllMinionsDestroyedEvent;
        [SerializeField] private VoidEventChannelSO onGameplayEndEvent;
        [SerializeField] private GameObjectEventChannelSO onMinionAttackingEvent;
        [SerializeField] private GameObjectEventChannelSO onMinionAttackedEvent;
        [SerializeField] private BoolEventChannelSO onCanMinionsAttackEvent;
        
        private List<GameObject> _minions;
        private bool _isSpawning;
        private Coroutine _spawnCoroutine;
        private List<GameObject> _attackingMinions;
    
        protected void OnEnable()
        {
            _spawnCoroutine = StartCoroutine(SpawnMinions());
            onMinionDeletedEvent?.onGameObjectEvent.AddListener(HandleDeletedEvent);
            onGameplayEndEvent?.onEvent.AddListener(RemoveAllMinions);
            onMinionAttackingEvent?.onGameObjectEvent.AddListener(AddAttackingMinion);
            onMinionAttackedEvent?.onGameObjectEvent.AddListener(RemoveAttackingMinion);
        }

        protected void OnDisable()
        {
            onMinionDeletedEvent?.onGameObjectEvent.RemoveListener(HandleDeletedEvent);
            onGameplayEndEvent?.onEvent.RemoveListener(RemoveAllMinions);
            onMinionAttackingEvent?.onGameObjectEvent.RemoveListener(AddAttackingMinion);
            onMinionAttackedEvent?.onGameObjectEvent.RemoveListener(RemoveAttackingMinion);
            StopCoroutine(_spawnCoroutine);
        }

        private void AddAttackingMinion(GameObject minion)
        {
            _attackingMinions.Add(minion);

            if (!CanMinionsAttack())
            {
                onCanMinionsAttackEvent?.RaiseEvent(false);
            }
        }

        private void RemoveAttackingMinion(GameObject minion)
        {
            _attackingMinions.RemoveAll(aMinion => aMinion == minion);

            if (CanMinionsAttack())
            {
                onCanMinionsAttackEvent?.RaiseEvent(true);
            }
        }
        
        private void RemoveAllMinions()
        {
            if (_minions == null) return;
            foreach (var minion in _minions.ToList())
            {
                _minions.Remove(minion);
                MinionObjectPool.Instance?.ReturnToPool(minion);
            }
        }

        private void HandleDeletedEvent(GameObject deletedMinion)
        {
            MinionObjectPool.Instance?.ReturnToPool(deletedMinion);
            
            _minions.Remove(deletedMinion);
            _attackingMinions.RemoveAll(aMinion => aMinion == deletedMinion);

            if (CanMinionsAttack())
            {
                onCanMinionsAttackEvent?.RaiseEvent(true);
            }
            
            if (_minions.Count == 0 && !_isSpawning)
            {
                onAllMinionsDestroyedEvent?.RaiseEvent();
            }
        }

        private bool CanMinionsAttack()
        {
            return _attackingMinions.Count < minionManagerConfig.maxMinionsAttackingAtSameTime;
        }

        private IEnumerator SpawnMinions()
        {
            _isSpawning = true;
            _minions = new List<GameObject>();
            _attackingMinions = new List<GameObject>();

            int minionsSpawned = 0;
            while(minionsSpawned < minionSpawnerConfig.minionsToSpawn)
            {
                if(minionsSpawned != 0) yield return new WaitForSeconds(minionSpawnerConfig.timeBetweenSpawns);
                if(_minions.Count >= minionManagerConfig.maxMinionsAtSameTime) continue;
                
                GameObject minion = MinionObjectPool.Instance?.GetPooledObject();
                if (minion == null)
                {
                    Debug.LogError("Minion was null");
                    break;
                }

                MinionAgent minionAgent = minion.GetComponent<MinionAgent>();
                minionAgent.SetPlayer(player);

                MinionIdleController minionIdleController = minion.GetComponent<MinionIdleController>();
                minionIdleController.SetCanAttack(CanMinionsAttack());
                
                minion.transform.position = minionSpawnerConfig.GetSpawnPoint();
                minion.SetActive(true);

                _minions.Add(minion);
                minionsSpawned++;
            }

            _isSpawning = false;
            yield return null;
        }

        public void Clear()
        {
            RemoveAllMinions();
        }
    }
}
