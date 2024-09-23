using System.Collections.Generic;
using Events;
using FSM;
using Health;
using UnityEngine;

namespace Minion
{
    public class MinionAgent : Agent
    {
        [SerializeField] private GameObject player;
        [SerializeField] private GameObject model;
        [SerializeField] private GameObjectEventChannelSO onCollidePlayerEventChannel;

        [SerializeField] private HealthPoints healthPoints;

        [Header("Events")]
        [SerializeField] private ActionEventsWrapper idleEvents;
        [SerializeField] private ActionEventsWrapper moveEvents;
        [SerializeField] private ActionEventsWrapper chargeAttackEvents;
        [SerializeField] private ActionEventsWrapper attackEvents;
        
        private State _idleState;
        private State _moveState;
        private State _chargeAttackState;
        private State _attackState;

        protected override void Update()
        {
            Vector3 rotation = Quaternion.LookRotation(player.transform.position).eulerAngles;
            rotation.x = 0f;
            rotation.z = 0f;
            
            model.transform.rotation = Quaternion.Euler(rotation);
            base.Update();
        }
        
        protected override void OnEnable()
        {
            base.OnEnable();
            
            healthPoints?.OnDeathEvent.onEvent.AddListener(Die);
        }

        protected void OnDisable()
        {
            healthPoints?.OnDeathEvent.onEvent.RemoveListener(Die);
            healthPoints?.ResetHitPoints();
        }

        public GameObject GetPlayer()
        {
            Debug.Log($"PLAYER IN");
            return player;
        }

        public void ChangeStateToMove()
        {
            Fsm.ChangeState(_moveState);
        }

        public void ChangeStateToAttack()
        {
            Fsm.ChangeState(_attackState);
        }
        
        public void ChangeStateToChargeAttack()
        {
            Fsm.ChangeState(_chargeAttackState);
        }

        public void ChangeStateToIdle()
        {
             Fsm.ChangeState(_idleState);
        }
        
        protected override List<State> GetStates()
        {
            _idleState = new State();
            _idleState.EnterAction += idleEvents.ExecuteOnEnter;
            _idleState.UpdateAction += idleEvents.ExecuteOnUpdate;
            _idleState.ExitAction += idleEvents.ExecuteOnExit;
            
            _moveState = new State();
            _moveState.EnterAction += moveEvents.ExecuteOnEnter;
            _moveState.UpdateAction += moveEvents.ExecuteOnUpdate;
            _moveState.ExitAction += moveEvents.ExecuteOnExit;
                
            _chargeAttackState = new State();
            _chargeAttackState.EnterAction += chargeAttackEvents.ExecuteOnEnter;
            _chargeAttackState.UpdateAction += chargeAttackEvents.ExecuteOnUpdate;
            _chargeAttackState.ExitAction += chargeAttackEvents.ExecuteOnExit;
            
            _attackState = new State();
            _attackState.EnterAction += attackEvents.ExecuteOnEnter;
            _attackState.UpdateAction += attackEvents.ExecuteOnUpdate;
            _attackState.ExitAction += attackEvents.ExecuteOnExit;

            Transition idleToMoveTransition = new Transition(_idleState, _moveState);
            _idleState.AddTransition(idleToMoveTransition);

            Transition moveToChargeAttackTransition = new Transition(_moveState, _chargeAttackState);
            _moveState.AddTransition(moveToChargeAttackTransition);

            Transition chargeAttackToAttackTransition = new Transition(_chargeAttackState, _attackState);
            _chargeAttackState.AddTransition(chargeAttackToAttackTransition);

            Transition attackToIdleTransition = new Transition(_attackState, _idleState);
            _attackState.AddTransition(attackToIdleTransition);
            
            return new List<State>
                ()
                {
                    _idleState,
                    _moveState,
                    _chargeAttackState,
                    _attackState
                };
        }

        private void Die()
        {
            Destroy(this.gameObject);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.CompareTag("Player"))
            {
                onCollidePlayerEventChannel?.RaiseEvent(other.gameObject);
            }
        }
    }
}
