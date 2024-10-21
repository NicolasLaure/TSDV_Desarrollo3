using System;
using UnityEngine;

namespace Enemy
{
    public class EnemyMovementController : MonoBehaviour
    {
        [SerializeField] private EnemyConfigSO enemyConfig;
        [SerializeField] private Vector2 minMaxYMovement;
        [SerializeField] private float distanceToChangeDirection = 0.2f;

        private float _originalY;

        private bool _shouldGoDown = false;

        private void OnEnable()
        {
            SetOriginalY();
        }

        void Update()
        {
            Vector3 newPosition = GetHoverPosition();

            transform.position = newPosition;
        }

        Vector3 GetHoverPosition()
        {
            float minPosition = _originalY + minMaxYMovement.x;
            float maxPosition = _originalY + minMaxYMovement.y;


            if (Mathf.Abs(transform.position.y - minPosition) < distanceToChangeDirection)
                _shouldGoDown = false;
            else if (Mathf.Abs(transform.position.y - maxPosition) < distanceToChangeDirection)
                _shouldGoDown = true;

            Vector3 currentPos = transform.position;
            float targetY = _shouldGoDown ? minPosition : maxPosition;

            return Vector3.Lerp(currentPos, new Vector3(currentPos.x, targetY, currentPos.z), Time.deltaTime);
        }

        public void SetOriginalY()
        {
            _originalY = transform.position.y;
        }
    }
}