using System;
using DukeOfThieves.Common;
using DukeOfThieves.Infrastructure;
using DukeOfThieves.Services;
using UI.Windows;
using UICore;
using UnityEngine;
using UnityEngine.EventSystems;

namespace DukeOfThieves.Player
{
    public class PlayerMove : MonoBehaviour
    {
        [SerializeField]
        private CollisionEventHandler _bottomColliderHandler;
        [SerializeField]
        private CollisionEventHandler _frontColliderHandler;
        [SerializeField]
        private Rigidbody2D _rigidBody;
        
        [Header("Parameters")]
        [SerializeField]
        private float _moveSpeed = 3.5f;
        [SerializeField]
        private float _jumpSpeed = 7.5f;
        [SerializeField]
        private float _wallSlideSpeed = -1.0f;

        private InputListener _inputListener;
        private bool _canJump;
        private bool _isStanding;
        private bool _isJumping;
        private bool _isTouchingWall;
        private bool _isFalling;

        private bool _canMove;

        private void Start()
        {
            GameLoopState.OnGameStop += Stop;
            PausePopUp.OnPauseTap += HandlePause;
            TapToStartPopUp.OnClose += HandlePopUpClose;
            _inputListener = AllServices.Container.Single<InputListener>();
            var uiManager = AllServices.Container.Single<UIManager>();
            _inputListener.Initialize(OnTap);
            InitColliderHandlers();
        }

        private void Stop()
        {
            TapToStartPopUp.OnClose -= HandlePopUpClose;
            GameLoopState.OnGameStop -= Stop;
            PausePopUp.OnPauseTap -= HandlePause;
        }
        private void FixedUpdate()
        {
            if(!_canMove)
                return;
            
            var speed = _rigidBody.velocity;
            if (!_isTouchingWall)
            {
                speed.x = transform.right.x * _moveSpeed;
            }

            if (_canJump)
            {
                speed.y = _jumpSpeed;
                _isJumping = true;
            }

            if (_isJumping && speed.y < 0.0f)
            {
                _isJumping = false;
                _isFalling = true;
            }

            if (_isFalling && _isTouchingWall)
            {
                speed.y = _wallSlideSpeed;            
            }

            _canJump = false;
            _rigidBody.velocity = speed;
        }
        
        

        private void InitColliderHandlers()
        {
            var bottomData = new CollisionEventData
            {
                OnCollisionEnter = OnBottomCollisionEnter,
                OnCollisionExit = OnBottomCollisionExit
            };
            _bottomColliderHandler.Initialize(bottomData);
            
            var frontData = new CollisionEventData
            {
                OnCollisionEnter = OnFrontCollisionEnter,
                OnCollisionExit = OnFrontCollisionExit
            };
            _frontColliderHandler.Initialize(frontData);
        }

        private void OnBottomCollisionEnter(Transform transform)
        {
            _isStanding = true;
            _isFalling = false;
        }

        private void OnBottomCollisionExit(Transform transform)
        {
            _isStanding = false;
            if (!_isJumping)
            {
                _isFalling = true;
            }
        }
        
        private void OnFrontCollisionEnter(Transform transform)
        {
            _isTouchingWall = true;
        }

        private void OnFrontCollisionExit(Transform transform)
        {
            _isTouchingWall = false;
        }

        private void OnTap()
        {
            if (_isStanding)
            {
                _canJump = true;
                return;
            }


            if (!_isTouchingWall) 
                return;
            
            transform.right *= -1.0f;
            _isFalling = false; 
            _canJump = true;
        }
        
        private void HandlePause(bool pauseState)
        {
            _canMove = !pauseState;
            if (pauseState)
            {
                _rigidBody.velocity = Vector3.zero;
                _rigidBody.angularVelocity = 0;
                
                _rigidBody.bodyType = RigidbodyType2D.Static;
            }
            else
            {
                _rigidBody.bodyType = RigidbodyType2D.Dynamic;
            }
        }

        private void HandlePopUpClose()
        {
            _canMove = true;
           
        }
    }
}