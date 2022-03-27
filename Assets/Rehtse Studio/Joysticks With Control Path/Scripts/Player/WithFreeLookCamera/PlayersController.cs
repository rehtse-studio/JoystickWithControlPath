using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using RehtseStudio.JoystickWithControlPath.PlayersInputActions.Scripts;

namespace RehtseStudio.JoystickWithControlPath.PlayerWithFreeLookCamera.Scripts
{
    public class PlayersController : MonoBehaviour
    {
        //Calling the Generated C# Script of the InputActions called Players_InputActions
        //The Script "Players_InputActions" is located on Assets->RehtseStudio->JoystickWithControlPath->Scripts->InputActions
        
        private Players_InputActions _playerInputs;
        private InputAction _moveAction;
        
        private float _speed;

        private Vector2 _moveVector;
        
        private Vector3 _movement;
        private Rigidbody _rigidBody;
        private float _targetRotation = 0.0f;
        private float _rotationVelocity;

        private Camera _mainCamera;

        #region Players Inputs
        private void OnEnable()
        {
            _playerInputs = new Players_InputActions();
            _playerInputs.Enable();

            _moveAction = _playerInputs.Player.Move;
            _moveAction.Enable();
        }

        private void OnDisable()
        {
            _playerInputs.Disable();
            _moveAction.Disable();
        }
        #endregion
        private void Start()
        {
            _rigidBody = GetComponent<Rigidbody>();

            _mainCamera = Camera.main;
        }

        private void FixedUpdate()
        {
            _moveVector = _moveAction.ReadValue<Vector2>();

            Movement();
        }

        #region Movement Section
        private void Movement()
        {
            if (_moveVector != Vector2.zero)
            {
                _speed = 5f;

                _targetRotation = Mathf.Atan2(_moveVector.x, _moveVector.y) * Mathf.Rad2Deg + _mainCamera.transform.eulerAngles.y;
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
    }
}

