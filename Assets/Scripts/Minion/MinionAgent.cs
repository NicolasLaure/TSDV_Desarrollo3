using System;
using System.Collections.Generic;
using Events.ScriptableObjects;
using FSM;
using Health;
using UnityEngine;

namespace Minion
{
    public class MinionAgent : Agent
    {
        [SerializeField] private GameObject model;

        [SerializeField] private HealthPoints healthPoints;

        [Header("Events")]
        [SerializeField] private MinionAgentEventChannelSO onMinionDeletedEvent;
        [SerializeField] private MinionAgentEventChannelSO onMinionAttackedEvent;

        [Header("Internal Events")]
        [SerializeField] private ActionEventsWrapper idleEvents;
        [SerializeField] private ActionEventsWrapper moveEvents;
        [SerializeField] private ActionEventsWrapper chargeAttackEvents;
        [SerializeField] private ActionEventsWrapper attackEvents;
        [SerializeField] private ActionEventsWrapper fallbackEvents;

        private GameObject _player;
        private State _idleState;
        private State _moveState;
        private State _chargeAttackState;
        private State _attackState;
        private State _fallbackState;


        protected override void OnDisable()
        {
            healthPoints?.ResetHitPoints();

            base.OnDisable();
        }

        public void SetIsNotInAttackState()
        {
            onMinionAttackedEvent?.RaiseEvent(this);
        }

        public GameObject GetPlayer()
        {
            return _player;
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

        public void ChangeStateToFallingBack()
        {
            Fsm.ChangeState(_fallbackState);
        }

        public void SetPlayer(GameObject player)
        {
            _player = player;
        }

        protected override List<State> GetStates()
        {
            _idleState = CreateStateWithEvents(idleEvents);
            _moveState = CreateStateWithEvents(moveEvents);
            _chargeAttackState = CreateStateWithEvents(chargeAttackEvents);
            _attackState = CreateStateWithEvents(attackEvents);
            _fallbackState = CreateStateWithEvents(fallbackEvents);

            Transition idleToMoveTransition = new Transition(_idleState, _moveState);
            _idleState.AddTransition(idleToMoveTransition);

            Transition moveToChargeAttackTransition = new Transition(_moveState, _chargeAttackState);
            _moveState.AddTransition(moveToChargeAttackTransition);

            Transition chargeAttackToAttackTransition = new Transition(_chargeAttackState, _attackState);
            _chargeAttackState.AddTransition(chargeAttackToAttackTransition);

            Transition attackToFallbackTransition = new Transition(_attackState, _fallbackState);
            _attackState.AddTransition(attackToFallbackTransition);

            Transition fallbackToIdleTransition = new Transition(_fallbackState, _idleState);
            _fallbackState.AddTransition(fallbackToIdleTransition);

            Transition moveToIdle = new Transition(_moveState, _idleState);
            _moveState.AddTransition(moveToIdle);
            
            Transition chargeToIdle = new Transition(_chargeAttackState, _idleState);
            _chargeAttackState.AddTransition(chargeToIdle);
            
            Transition attackToIdle = new Transition(_attackState, _idleState);
            _attackState.AddTransition(attackToIdle);

            return new List<State>
                ()
                {
                    _idleState,
                    _moveState,
                    _chargeAttackState,
                    _fallbackState,
                    _attackState
                };
        }

        private State CreateStateWithEvents(ActionEventsWrapper eventsWrapper)
        {
            State state = new State();
            state.EnterAction += eventsWrapper.ExecuteOnEnter;
            state.UpdateAction += eventsWrapper.ExecuteOnUpdate;
            state.ExitAction += eventsWrapper.ExecuteOnExit;

            return state;
        }

        private void ClearStateWithEvent(State state, ActionEventsWrapper eventsWrapper)
        {
            state.EnterAction -= eventsWrapper.ExecuteOnEnter;
            state.UpdateAction -= eventsWrapper.ExecuteOnUpdate;
            state.ExitAction -= eventsWrapper.ExecuteOnExit;
        }


        public void Die()
        {
            if (healthPoints.CurrentHp <= 0)
                onMinionDeletedEvent?.RaiseEvent(this);
        }

        public GameObject GetModel()
        {
            return model;
        }

        [ContextMenu("CurrentState")]
        public void DebugCurrentState()
        {
            State currentState = Fsm.GetCurrentState();
            if(currentState == _idleState)
                Debug.Log("Idle");
            else if(currentState == _moveState)
                Debug.Log("Move");
            else if(currentState == _chargeAttackState)
                Debug.Log("Charge");
            else if(currentState == _fallbackState)
                Debug.Log("Fallback");
            else if(currentState == _attackState)
                Debug.Log("Attack");
                
        }
    }
}