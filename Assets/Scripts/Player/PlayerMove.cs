using System;
using DukeOfThieves.Common;
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

        [SerializeField] private InputListener _inputListener;
        
        [Header("Parameters")]
        [SerializeField]
        private float _moveSpeed = 3.5f;
        [SerializeField]
        private float _jumpSpeed = 7.5f;
        [SerializeField]
        private float _wallSlideSpeed = -1.0f;

        private bool _canJump;
        private bool _isStanding;
        private bool _isJumping;
        private bool _isTouchingWall;
        private bool _isFalling;

        private void Start()
        {
            _inputListener.Initialize(OnTap);
            InitColliderHandlers();
        }
        
        private void FixedUpdate()
        {
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
    }
}