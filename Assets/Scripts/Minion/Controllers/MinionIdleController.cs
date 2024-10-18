using System;
using System.Collections;
using Events;
using Events.ScriptableObjects;
using Minion.Manager;
using Minion.ScriptableObjects;
using UnityEngine;

namespace Minion.Controllers
{
    public class MinionIdleController : MinionController
    {
        [SerializeField] private MinionSO minionConfig;
        [SerializeField] private MinionAgentEventChannelSO onMinionWantsToAttack;
        private Coroutine _idleTime;
        private bool _canAttack;

        protected override void OnEnable()
        {
            StopIdleCoroutine();
            base.OnEnable();
        }

        private void OnDisable()
        {
            StopIdleCoroutine();
        }

        public void Enter()
        {
            transform.rotation = Quaternion.identity;
            StopIdleCoroutine();

            SetCanAttack(false);
            _idleTime = StartCoroutine(HandleIdleTime());
        }

        public void SetCanAttack(bool value)
        {
            _canAttack = value;
        }

        private IEnumerator HandleIdleTime()
        {
            yield return new WaitForSeconds(minionConfig.GetRandomIdleTime());
            onMinionWantsToAttack?.RaiseEvent(minionAgent);
            yield return new WaitUntil(() => _canAttack);
            minionAgent.ChangeStateToMove();
        }

        public void Exit()
        {
            StopIdleCoroutine();
        }

        private void StopIdleCoroutine()
        {
            if (_idleTime != null)
                StopCoroutine(_idleTime);
        }
    }
}