using Minion;
using UnityEngine;

namespace Events.ScriptableObjects
{
    [CreateAssetMenu(fileName = "MinionAgentEvent", menuName = "Events/MinionAgentEvent", order = 0)]
    public class MinionAgentEventChannelSO : EventChannelSO<MinionAgent>
    {
    }
}