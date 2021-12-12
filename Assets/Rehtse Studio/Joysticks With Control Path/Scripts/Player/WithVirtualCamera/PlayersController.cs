using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace RehtseStudio.JoystickWithControlPath.PlayerWithVirtualCamera.Scripts
{
    public class PlayersController : MonoBehaviour
    {
        private float _speed;

        private Vector2 _moveAction;
        private Vector2 _lookAction;

        private Vector3 _movement;
        private Rigidbody _rigidBody;
        private float _targetRotation = 0.0f;
        private float _rotationVelocity;

        [SerializeField] private GameObject _cinemachineTargetObject;
        private Camera _mainCamera;
        private float _cinemachineTargetX;
        private float _cinemachineTargetY;

        #region Player Input Section
        public void OnMove(InputValue onMoveValue)
        {
            _moveAction = onMoveValue.Get<Vector2>();
        }

        public void OnLook(InputValue onLookValue)
        {
            _lookAction = onLookValue.Get<Vector2>();
        }
        #endregion

        private void Start()
        {
            _rigidBody = GetComponent<Rigidbody>();
            _mainCamera = Camera.main;
        }
        private void Update()
        {
            CameraRotation();
        }

        private void FixedUpdate()
        {
            Movement();
        }

        #region Movement Section
        private void Movement()
        {
            if (_moveAction != Vector2.zero)
            {
                _speed = 5f;

                _targetRotation = Mathf.Atan2(_moveAction.x, _moveAction.y) * Mathf.Rad2Deg + _mainCamera.transform.eulerAngles.y;
                float rotation = Mathf.SmoothDampAngle(transform.eulerAngles.y, _targetRotation, ref _rotationVelocity, 0.12f);
                transform.rotation = Quaternion.Euler(0.0f, rotation, 0.0f);
                Vector3 targetDirection = Quaternion.Euler(0.0f, _targetRotation, 0.0f) * Vector3.forward;

                _movement = targetDirection * _speed + new Vector3(0, _rigidBody.velocity.y, 0);
            }
            else
            {
                _speed = 0.0f;
                _movement = new Vector3(0, _rigidBody.velocity.y, 0);
            }
            _rigidBody.velocity = _movement;
        }
        #endregion

        #region Camera Section
        private void CameraRotation()
        {
            _cinemachineTargetX += _lookAction.x * Time.deltaTime * 35f;
            _cinemachineTargetY += _lookAction.y * Time.deltaTime * 35f;

            _cinemachineTargetX = CameraClampAngle(_cinemachineTargetX, float.MinValue, float.MaxValue);
            _cinemachineTargetY = CameraClampAngle(_cinemachineTargetY, -30.0f, 70.0f);

            _cinemachineTargetObject.transform.rotation = Quaternion.Euler(_cinemachineTargetY + 0.0f, _cinemachineTargetX, 0.0f);
        }

        private float CameraClampAngle(float angle, float angleMin, float angleMax)
        {
            if (angle < -360f) angle += 360f;
            if (angle > 360f) angle -= 360f;

            return Mathf.Clamp(angle, angleMin, angleMax);
        }
        #endregion
    }
}

