using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

//
using UnityEngine.Serialization;
using UnityEngine.InputSystem.Layouts;
using UnityEngine.InputSystem.OnScreen;
//

namespace RehtseStudio.UnityNewInputSystem_VirtualJoystickAndTouchLook_ControlPath.Scripts
{
    public class UIVirtualTouchZone : OnScreenControl, IPointerDownHandler, IPointerUpHandler, IDragHandler
    {
        [Header("Select The Control Path Of The Touch Zone")]
        [InputControl(layout = "Vector2")]
        [SerializeField] private string _controlPath;

        protected override string controlPathInternal
        {
            get => _controlPath;
            set => _controlPath = value;
        }

        [Header("Reference To The Virtual Toucha Zone Rect And Joystick Handle")]
        [SerializeField] private RectTransform _virtualTouchZoneConainterRect;
        [SerializeField] private RectTransform _joystickHandleRect;

        [Header("Virtual Touch Zone Settings")]
        private bool _isClampToMagnitude;
        [SerializeField] float _magnitdeMultiplier = 1.0f;
        [SerializeField] bool _doYouWantToInvertXOutputValue;
        [SerializeField] bool _doYouWantToInvertYOutputValue;

        [Tooltip("Store Pointer Values")]
        private Vector2 _pointerDownPosition;
        private Vector2 _currentPointerPosition;
        private Vector2 _positionDelta;
        private Vector2 _clampPosition;
        private Vector2 _outputPosition;

        private void Start()
        {
            SetupHandle();
        }

        private void SetupHandle()
        {
            if (_joystickHandleRect)
            {
                SetObjectiveActiveState(_joystickHandleRect.gameObject, false);
            }
        }

        public void OnPointerDown(PointerEventData _pointerDownData)
        {
            RectTransformUtility.ScreenPointToLocalPointInRectangle(_virtualTouchZoneConainterRect, _pointerDownData.position, _pointerDownData.pressEventCamera, out _pointerDownPosition);

            if (_joystickHandleRect)
            {
                SetObjectiveActiveState(_joystickHandleRect.gameObject, true);
                UpdateJoystickHandlePosition(_pointerDownPosition);
            }
        }

        public void OnDrag(PointerEventData _dragData)
        {
            RectTransformUtility.ScreenPointToLocalPointInRectangle(_virtualTouchZoneConainterRect, _dragData.position, _dragData.pressEventCamera, out _currentPointerPosition);

            _positionDelta = GetDeltaBetweenPositions(_pointerDownPosition, _currentPointerPosition);
            _clampPosition = ClampValuesToMagnitude(_positionDelta);
            _outputPosition = ApplyInversionFilter(_clampPosition);

            OutputValue(_outputPosition * _magnitdeMultiplier);
        }

        public void OnPointerUp(PointerEventData _pointerUpData)
        {
            _pointerDownPosition = Vector2.zero;
            _currentPointerPosition = Vector2.zero;

            OutputValue(Vector2.zero);

            if (_joystickHandleRect)
            {
                SetObjectiveActiveState(_joystickHandleRect.gameObject, false);
                UpdateJoystickHandlePosition(Vector2.zero);
            }
        }

        private void OutputValue(Vector2 pointerPos)
        {
            SendValueToControl(pointerPos);
        }

        Vector2 GetDeltaBetweenPositions(Vector2 firstPos, Vector2 secondPos)
        {
            return secondPos - firstPos;
        }

        Vector2 ClampValuesToMagnitude(Vector2 pos)
        {
            return Vector2.ClampMagnitude(pos, 1);
        }

        Vector2 ApplyInversionFilter(Vector2 pos)
        {
            if (_doYouWantToInvertXOutputValue)
                pos.x = InvertValue(pos.x);

            if (_doYouWantToInvertYOutputValue)
                pos.y = InvertValue(pos.y);

            return pos;
        }

        float InvertValue(float posValue)
        {
            return -posValue;
        }

        private void SetObjectiveActiveState(GameObject objectTarget, bool state)
        {
            objectTarget.SetActive(state);
        }

        private void UpdateJoystickHandlePosition(Vector2 newPos)
        {
            _joystickHandleRect.anchoredPosition = newPos;
        }

    }
}

