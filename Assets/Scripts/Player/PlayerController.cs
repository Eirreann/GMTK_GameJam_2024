using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace GMTK_Jam.Player
{
    /// <summary>
    /// This class governs Player input to move the camera (and possibly other things? TBD)
    /// </summary>
    public class PlayerController : MonoBehaviour
    {
        public List<Camera> Cameras;
        public WorldBoundary Boundary;

        [Header("Attributes")]
        [SerializeField] private bool MovementPermitted = true;
        [SerializeField] private float _moveIncrement = 2;
        [SerializeField] private float _moveSpd = 5f;

        [Header("Zoom")]
        [SerializeField] private float _zoomInterval = 5f;
        [SerializeField] private float _maxZoom = 30f;
        [SerializeField] private float _minZoom = 90f;

        [HideInInspector] public UnityEvent<bool> OnScroll;

        // Inputs
        private PlayerInput _playerInput;
        private InputAction _camMoveKeys;
        private InputAction _camMoveMouse;
        private InputAction _levelUpDown;
        private InputAction _recenter;
        private InputAction _buy;
        private InputAction _zoomMod;

        private Vector3 _startPosition;
        private Vector2 _moveDir = Vector2.zero;
        private bool _isKeyPressed = false;
        private bool _zoomModPressed = false;
        private IScrollInteractable _prevHit;

        private void Awake()
        {
            _playerInput = GetComponent<PlayerInput>();
            _camMoveKeys = _playerInput.actions["Camera_Move_Keys"];
            _camMoveMouse = _playerInput.actions["Camera_Move_Mouse"];
            _levelUpDown = _playerInput.actions["Scrollwheel"];
            _recenter = _playerInput.actions["Reset_View"];
            _buy = _playerInput.actions["Buy"];
            _zoomMod = _playerInput.actions["Zoom_Mod"];

            _startPosition = transform.position;
        }

        /// <summary>
        /// Initialise the player's control inputs
        /// </summary>
        public void InitInputs()
        {
            _camMoveKeys.performed += _setKeyMovement;
            _camMoveKeys.canceled += _setKeyMovement;

            _camMoveMouse.performed += _setMouseMovement;
            _camMoveMouse.canceled += _setMouseMovement;

            _levelUpDown.performed += _setScrollValue;
            _levelUpDown.canceled += _setScrollValue;

            _recenter.performed += _resetView;
            _recenter.canceled += _resetView;

            //_buy.performed += _openBuyMenu;
            //_buy.canceled += _openBuyMenu;

            _zoomMod.performed += _onZoomMod;
            _zoomMod.canceled += _onZoomMod;
        }

        public void DeregisterInputs()
        {
            _camMoveKeys.performed -= _setKeyMovement;
            _camMoveKeys.canceled -= _setKeyMovement;

            _camMoveMouse.performed -= _setMouseMovement;
            _camMoveMouse.canceled -= _setMouseMovement;

            _levelUpDown.performed -= _setScrollValue;
            _levelUpDown.canceled -= _setScrollValue;

            _recenter.performed -= _resetView;
            _recenter.canceled -= _resetView;

            //_buy.performed -= _openBuyMenu;
            //_buy.canceled -= _openBuyMenu;

            _zoomMod.performed -= _onZoomMod;
            _zoomMod.canceled -= _onZoomMod;
        }

        /// <summary>
        /// Enables/disables the player's ability to move the camera
        /// </summary>
        /// <param name="state">Is movement enabled? Yes/no</param>
        public void EnableMovement(bool state)
        {
            MovementPermitted = state;
            if(!state)
                _moveDir = Vector2.zero;
        }

        private void Update()
        {
            Vector3 target = new(transform.position.x + (_moveIncrement * _moveDir.y), transform.position.y, transform.position.z + (_moveIncrement * _moveDir.x));

            if (Boundary.IsWithinBoundary(target))
            {
                if (transform.position != target)
                    transform.position = Vector3.MoveTowards(transform.position, target, _moveSpd * Time.deltaTime);
            }

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                if (hit.collider.tag == "ScrollInteractable")
                {
                    if (_prevHit != null && _prevHit != hit.collider.GetComponentInParent<IScrollInteractable>())
                    {
                        _prevHit.OnHover(false);
                        _prevHit = null;
                    }

                    //Debug.Log(hit.collider.name);
                    _prevHit = hit.collider.GetComponentInParent<IScrollInteractable>();
                    _prevHit.OnHover(true);
                }
                else
                {
                    if(_prevHit != null)
                    {
                        _prevHit.OnHover(false);
                        _prevHit = null;
                    }
                }
            }
        }

        private void _setKeyMovement(InputAction.CallbackContext context)
        {
            if (!MovementPermitted || GameManager.Instance.State != GameState.ACTIVE) return;

            Vector2 value = context.ReadValue<Vector2>();
            _moveDir = new Vector2(value.x, -value.y);
            _isKeyPressed =  _moveDir.x != 0 || _moveDir.y != 0;
            //Debug.Log(value);
        }

        private void _setMouseMovement(InputAction.CallbackContext context)
        {
            // Don't let mouse override key presses, don't move camera if 
            if (_isKeyPressed || !MovementPermitted || GameManager.Instance.State != GameState.ACTIVE) return;

            // Don't listen to mouse movement if the screen is not in focus (also reset moveDir to 0 so it doesn't keep moving)
            if (!Application.isFocused)
            {
                _moveDir = Vector2.zero;
                return;
            }

            Vector2 value = context.ReadValue<Vector2>();
            float widthFromCentre = Screen.width / 2;
            float heightFromCentre = Screen.height / 2;

            // Offset vector to calculate poosition from centre of screen
            value = new Vector2(value.x - widthFromCentre, value.y - heightFromCentre);

            // Convert output of vector to value that is either -1, 0, or 1
            float widthVal = value.x / widthFromCentre;
            float heightVal = value.y / heightFromCentre;
            if(widthVal >= 0.99)
                widthVal = 1;
            else if(widthVal <= -0.99)
                widthVal = -1;
            if(heightVal >= 0.99)
                heightVal = 1;
            else if(heightVal <= -0.99)
                heightVal = -1;
            value = new Vector2((int)Mathf.Clamp(widthVal, -1, 1), (int)Mathf.Clamp(heightVal, -1, 1));

            // Update move direction with result
            _moveDir = new Vector2(value.x, -value.y); ;
            //Debug.Log(value);
        }

        private void _setScrollValue(InputAction.CallbackContext context)
        {
            if (GameManager.Instance.State != GameState.ACTIVE) return;

            float value = context.ReadValue<float>();
            bool scrollDirection = value > 0;

            if(value != 0)
            {
                if (_zoomModPressed)
                {
                    float interval = _zoomInterval * (scrollDirection ? -1 : 1);
                    float newZoom = Camera.main.fieldOfView + interval;
                    if (newZoom > _minZoom)
                        newZoom = _minZoom;
                    if(newZoom < _maxZoom)
                        newZoom = _maxZoom;

                    Cameras.ForEach(c => c.fieldOfView = newZoom);
                }
                else
                {
                    Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                    if (Physics.Raycast(ray, out RaycastHit hit))
                    {
                        if (hit.collider.tag == "ScrollInteractable")
                        {
                            //Debug.Log(hit.collider.name);
                            hit.collider.GetComponentInParent<IScrollInteractable>().OnScrollValue(scrollDirection);
                        }
                    }

                    OnScroll?.Invoke(scrollDirection);
                }
            }
        }

        private void _onZoomMod(InputAction.CallbackContext context)
        {
            if (GameManager.Instance.State != GameState.ACTIVE) return;

            _zoomModPressed = (context.ReadValue<float>() == 1);
        }

        private void _resetView(InputAction.CallbackContext context)
        {
            if(!MovementPermitted || GameManager.Instance.State != GameState.ACTIVE) return;

            if (context.ReadValue<float>() == 1)
            {
                if(GameManager.Instance.State == GameState.ENDED)
                {
                    // TODO
                }
                else
                {
                    _isKeyPressed = true;
                    transform.position = _startPosition;
                }
            }
            else
                _isKeyPressed = false;
        }

        private void _openBuyMenu(InputAction.CallbackContext context)
        {
            if (GameManager.Instance.State != GameState.ACTIVE) return;

            if (context.ReadValue<float>() == 1)
            {
                GameManager.Instance.OpenBuyMenu();
                Debug.Log("Buy button pressed");
            }
        }
    }
}
