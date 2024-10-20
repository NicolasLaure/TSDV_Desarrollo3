using System.Collections;
using Events;
using Events.ScriptableObjects;
using UnityEngine;
using Utils;

namespace LevelManagement.Sequences
{
    public class EndLevelSequence : MonoBehaviour
    {
        [SerializeField] private GameObject player;
        [SerializeField] private float playerVelocity;
        [SerializeField] private float finishZPosition;
        [SerializeField] private string creditsScene = "Credits";

        [Header("Events")] 
        [SerializeField] private VoidEventChannelSO onCinematicStartEvent;
        [SerializeField] private VoidEventChannelSO onCinematicPlayerLockStart;
        [SerializeField] private VoidEventChannelSO onStartCinematicCanvas;
        [SerializeField] private VoidEventChannelSO onEndCinematicCanvas;
        [SerializeField] private VoidEventChannelSO onCinematicCanvasFinishedAnimation;
        [SerializeField] private BoolEventChannelSO onGameplayUICanvasEvent;
        [SerializeField] private BoolEventChannelSO onCinematicUICanvasEvent;
        [SerializeField] private StringEventChannelSo onChangeSceneEvent;
        
        private bool _isCinematicCanvasAnimating;
        
        private void OnEnable()
        {
            onCinematicCanvasFinishedAnimation?.onEvent.AddListener(HandleFinishedAnimation);
        }
        
        private void OnDisable()
        {
            onCinematicCanvasFinishedAnimation?.onEvent.RemoveListener(HandleFinishedAnimation);
        }

        public Sequence GetEndSequence()
        {
            Sequence endSequence = new Sequence();
            
            endSequence.AddPreAction(HandleStartCinematic());
            endSequence.SetAction(MovePlayerToHorizon());
            endSequence.AddPostAction(HandleOpenCredits());
            
            return endSequence;
        }

        private IEnumerator HandleOpenCredits()
        {
            onChangeSceneEvent?.RaiseEvent(creditsScene);
            yield return null;
        }

        private IEnumerator MovePlayerToHorizon()
        {
            while (player.transform.position.z < finishZPosition)
            {
                player.transform.position += new Vector3(0, 0, playerVelocity) * Time.deltaTime;
                yield return null;
            }
            
            player.SetActive(false);
        }


        private IEnumerator HandleStartCinematic()
        {
            onCinematicStartEvent?.RaiseEvent();
            onCinematicPlayerLockStart?.RaiseEvent();
            onGameplayUICanvasEvent?.RaiseEvent(false);
            onCinematicUICanvasEvent?.RaiseEvent(true);
            onStartCinematicCanvas?.RaiseEvent();
            _isCinematicCanvasAnimating = true;
            yield return new WaitWhile(() => _isCinematicCanvasAnimating);
        }


        private void HandleFinishedAnimation()
        {
            _isCinematicCanvasAnimating = false;
        }
    }
}
