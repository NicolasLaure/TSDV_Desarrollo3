using System.Collections;
using Events;
using Health;
using Minion.ScriptableObjects;
using UnityEngine;

namespace Minion.Controllers
{ public class MinionChargeAttackController : MinionController
    {
        [SerializeField] private MinionSO minionConfig;
        
        private LineRenderer _aimLine;
        private Vector3 _dir;
        private bool _isCharging;
        
        public void Enter()
        {
            _aimLine ??= gameObject.transform.Find("AimLine").gameObject.GetComponent<LineRenderer>();
            
            StartCoroutine(AttackCoroutine());
        }

        private void StartAimLine()
        {
            _aimLine.SetPosition(0, transform.position);
            _aimLine.SetPosition(1, transform.position);
            _aimLine.enabled = true;
        }

        private void SetNewAimPosition(float timer)
        {
            _dir = target.transform.position - transform.position;
            _dir.y = 0;

            Vector3 aimPosition = Vector3.Lerp(
                transform.position,
                transform.position + _dir.normalized * minionConfig.chargeAttackData.length, 
                timer / minionConfig.chargeAttackData.duration
                );
            _aimLine.SetPosition(1, aimPosition);
        }
        
        private IEnumerator AttackCoroutine()
        {
            float timer = 0;
            float startTime = Time.time;
            StartAimLine();
            
            while (timer < minionConfig.chargeAttackData.duration)
            {
                timer = Time.time - startTime;
                SetNewAimPosition(timer);
                yield return null;
            }

            _aimLine.enabled = false;
            MinionAgent.ChangeStateToAttack();
        }
    }
}