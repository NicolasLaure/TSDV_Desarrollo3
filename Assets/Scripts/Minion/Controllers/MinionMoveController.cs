using System;
using System.Collections;
using Events;
using Minion.ScriptableObjects;
using UnityEngine;

namespace Minion.Controllers
{
    public class MinionMoveController : MinionController
    {
        [SerializeField] private MinionSO minionConfig;
        [SerializeField] private Vector3EventChannelSO onPlayerMovedEvent;
        [SerializeField] private Vector3EventChannelSO onDashMovementEvent;

        private Vector3 _moveDir;
        private Coroutine _moveCoroutine;

        public void OnDisable()
        {
            onPlayerMovedEvent.onVectorEvent.RemoveListener(HandleMove);
            onDashMovementEvent.onVectorEvent.RemoveListener(HandleMove);

            if (_moveCoroutine != null)
                StopCoroutine(_moveCoroutine);
        }

        public void Enter()
        {
            _moveCoroutine = StartCoroutine(HandleChangeState());
        }

        private IEnumerator HandleChangeState()
        {
            bool isTargetFar = Vector3.Distance(transform.position, target.transform.position) >
                               minionConfig.moveData.minDistance;

            if (isTargetFar)
            {
                _moveDir = target.transform.position - transform.position;
                _moveDir.y = 0;
                while (Vector3.Distance(transform.position, target.transform.position) > minionConfig.moveData.minDistance)
                {
                    transform.Translate(_moveDir * (minionConfig.moveData.speed * Time.deltaTime));
                    yield return null;
                }
            }
            else
            {
                _moveDir = transform.position - target.transform.position;
                _moveDir.y = 0;
                while (Vector3.Distance(transform.position, target.transform.position) < minionConfig.moveData.minDistance)
                {
                    transform.Translate(_moveDir * (minionConfig.moveData.speed * Time.deltaTime));
                    yield return null;
                }
            }

            onPlayerMovedEvent.onVectorEvent.AddListener(HandleMove);
            onDashMovementEvent.onVectorEvent.AddListener(HandleMove);
            yield return new WaitForSeconds(minionConfig.GetRandomMoveTime());
            onPlayerMovedEvent.onVectorEvent.RemoveListener(HandleMove);
            onDashMovementEvent.onVectorEvent.RemoveListener(HandleMove);
            minionAgent.ChangeStateToChargeAttack();
        }

        private void HandleMove(Vector3 movement)
        {
            movement.y = 0;
            transform.position += movement;
        }

        public void OnUpdate()
        {
        }
    }
}