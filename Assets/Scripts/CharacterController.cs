using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace PeekMee.Friends.Character
{
    public class CharacterController : MonoBehaviour
    {
        [SerializeField] private float _jumpForce = 400f;
        [Range(0, 1)] [SerializeField] private float _crouchSpeed = .40f;
        [Range(0, .25f)] [SerializeField] private float _movementSmoothness = .05f;
        [SerializeField] private bool _allowedAirControl = false;
        [SerializeField] private LayerMask _whatIsGround;
        [SerializeField] private LayerMask _whatIsLadder;
        [SerializeField] private Transform _groundCheck;
        [SerializeField] private Transform _ceilingCheck;
        [SerializeField] private Transform _ladderCheck;
        [SerializeField] private Collider2D _crouchDisableCollider;

        enum CharacterFacing { RIGHT, LEFT }

        const float GROUNDED_RADIUS = .25f;
        const float CEILING_RADIUS = .25f;
        const float LADDER_RADIUS = .3f;
        private bool _isGrounded;
        private Rigidbody2D _rigidBody;
        private CharacterFacing _facing = CharacterFacing.RIGHT;
        private Vector3 _velocity = Vector3.zero;

        [Header("Events")]
        public UnityEvent OnLandEvent;

        [System.Serializable]
        public class BoolEvent : UnityEvent<bool> { }

        public BoolEvent OnCrouchEvent;
        private bool _wasCrouching = false;

        private void Awake()
        {
            _rigidBody = GetComponent<Rigidbody2D>();

            if (OnLandEvent == null) OnLandEvent = new UnityEvent();
            if (OnCrouchEvent == null) OnCrouchEvent = new BoolEvent();
        }

        private void FixedUpdate()
        {
            bool _wasGrounded = _isGrounded;
            _isGrounded = false;

            Collider2D[] colls = Physics2D.OverlapCircleAll(_groundCheck.position,
                GROUNDED_RADIUS, _whatIsGround);
            for (int i = 0; i < colls.Length; i++)
            {
                if (colls[i].gameObject != gameObject)
                {
                    _isGrounded = true;
                    if (!_wasGrounded) OnLandEvent?.Invoke();
                }
            }
        }

        public void Move(float _horizontalMove, float _verticalMove, bool _crouch, bool _jump)
        {
            bool _isAllowedtoMoveVertically = false;

            if (_verticalMove != 0)
            {
                if (Physics2D.OverlapCircle(_ladderCheck.position,
                    LADDER_RADIUS, _whatIsLadder))
                {
                    _isAllowedtoMoveVertically = true;
                    _rigidBody.simulated = false;
                }
                else
                {
                    _rigidBody.simulated = true;
                }
                    

            }

            if (!_crouch)
            {
                if (Physics2D.OverlapCircle(_ceilingCheck.position,
                    CEILING_RADIUS, _whatIsGround))
                {
                    _crouch = true;
                }
            }

            if (_isGrounded || _allowedAirControl)
            {
                if (!_wasCrouching)
                {
                    _wasCrouching = true;
                    OnCrouchEvent?.Invoke(true);
                }

                _horizontalMove *= _crouchSpeed;

                if (_crouchDisableCollider != null) 
                    _crouchDisableCollider.enabled = false;
                
            }
            else
            {
                if (_crouchDisableCollider != null) 
                    _crouchDisableCollider.enabled = true;
                if (_wasCrouching)
                {
                    _wasCrouching = false;
                    OnCrouchEvent?.Invoke(false);
                }
            }
            Vector3 _targetVelocity = Vector3.zero;
            _targetVelocity.x = _horizontalMove * 10f;

            switch (_isAllowedtoMoveVertically)
            {
                case true:
                    _targetVelocity.x = _horizontalMove / 100f;
                    _targetVelocity.y = _verticalMove * 10f;
                    print(_targetVelocity);
                    transform.Translate(_targetVelocity);
                    break;
                case false:
                    _targetVelocity.y = _rigidBody.velocity.y;
                    _rigidBody.velocity = Vector3.SmoothDamp(_rigidBody.velocity, 
                        _targetVelocity, ref _velocity, _movementSmoothness);
                    break;
            }

            // Move
            //Vector3 _targetVelocity = new Vector2(_horizontalMove * 10f, _rigidBody.velocity.y);
            

            if (_horizontalMove > 0 && _facing == CharacterFacing.LEFT) Flip();
            else if (_horizontalMove < 0 && _facing == CharacterFacing.RIGHT) Flip();

            if (_isGrounded && _jump)
            {
                _isGrounded = false;
                _rigidBody.AddForce(new Vector2(0f, _jumpForce));
            }
        }

        private void Flip()
        {
            switch (_facing)
            {
                case CharacterFacing.RIGHT:
                    _facing = CharacterFacing.LEFT;
                    break;
                case CharacterFacing.LEFT:
                    _facing = CharacterFacing.RIGHT;
                    break;
            }

            Vector3 _localScale = transform.localScale;
            _localScale.x *= -1;
            transform.localScale = _localScale;
        }
    } 

}
