using MapBounds;
using Minion.Manager;
using UnityEngine;

namespace Minion.Controllers
{
    public class MinionFallbackController : MinionController
    {
        [SerializeField] private MapBoundsSO minionBounds;
        [SerializeField] private float moveVelocity;
        
        public void MoveToBounds()
        {
            if (minionBounds.depthBounds.min > transform.position.z)
            {
                transform.position += GetBackVector() * moveVelocity * Time.deltaTime;
            }
            else
            {
                minionAgent.SetIsNotInAttackState();
                minionAgent.ChangeStateToIdle();
            }
        }

        private Vector3 GetBackVector()
        {
            return new Vector3(0, 0, minionBounds.depthBounds.min - transform.position.z).normalized;
        }
    }
}
