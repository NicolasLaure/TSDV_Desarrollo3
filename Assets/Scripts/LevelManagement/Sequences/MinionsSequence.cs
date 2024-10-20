using System.Collections;
using Events;
using Minion.Manager;
using UnityEngine;
using Utils;

namespace LevelManagement.Sequences
{
    public class MinionsSequence : MonoBehaviour
    {
        [SerializeField] private MinionManager minionManager;
        
        [Header("Events")] [SerializeField] private VoidEventChannelSO onAllMinionsDestroyedEvent;
        
        private bool _areAllMinionsDestroyed;
        private IEnumerator _postAction;
        
        private void OnEnable()
        {
            onAllMinionsDestroyedEvent?.onEvent.AddListener(HandleAllMinionsDestroyed);
        }

        private void OnDisable()
        {
            onAllMinionsDestroyedEvent?.onEvent.RemoveListener(HandleAllMinionsDestroyed);
        }

        public void SetPostAction(IEnumerator postAction)
        {
            _postAction = postAction;
        }

        public void SetupSequence(MinionsData levelConfigMinionsData)
        {
            minionManager.SetupManager(levelConfigMinionsData);
            minionManager.gameObject.SetActive(false);
        }

        private IEnumerator MinionSequencePreActions()
        {
            _areAllMinionsDestroyed = false;

            yield return null;
        }
        
        private void HandleAllMinionsDestroyed()
        {
            _areAllMinionsDestroyed = true;
        }

        private IEnumerator SetMinionManager(bool value)
        {
            minionManager.gameObject.SetActive(value);

            yield return new WaitUntil(() => _areAllMinionsDestroyed);
        }

        public IEnumerator StartMinionPhase()
        {
            Sequence minionSequence = new Sequence();

            minionSequence.AddPreAction(MinionSequencePreActions());
            minionSequence.SetAction(SetMinionManager(true));
            minionSequence.AddPostAction(SetMinionManager(false));
            minionSequence.AddPostAction(_postAction);

            return minionSequence.Execute();
        }


        public void ClearSequence()
        {
            minionManager.Clear();
            HandleAllMinionsDestroyed();
        }
    }
}
