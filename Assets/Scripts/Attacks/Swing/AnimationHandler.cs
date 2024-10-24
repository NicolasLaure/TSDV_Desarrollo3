using Events;
using UnityEngine;

namespace Attacks.Swing
{
    public class AnimationHandler : StateMachineBehaviour
    {
        [SerializeField] private VoidEventChannelSO onAnimationStart;
        [SerializeField] private VoidEventChannelSO onAnimationEnd;
        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            onAnimationStart?.RaiseEvent();
        }

        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            onAnimationEnd?.RaiseEvent();
        }
    }
}
