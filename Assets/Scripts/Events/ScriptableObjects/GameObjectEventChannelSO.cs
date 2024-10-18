using Events.ScriptableObjects;
using UnityEngine;
using UnityEngine.Events;

namespace Events
{
    [CreateAssetMenu(menuName = "Events/GameObject Channel")]
    public class GameObjectEventChannelSO : EventChannelSO<GameObject>
    {
        public UnityEvent<GameObject> onGameObjectEvent;

        public void RaiseEvent(GameObject value)
        {
            if (onGameObjectEvent != null)
            {
                onGameObjectEvent.Invoke(value);
            }
            else
            {
                LogNullEventError();
            }
        }
    }
}