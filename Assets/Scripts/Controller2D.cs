using UnityEngine;
using UnityEngine.Events;

namespace PeekMee.Friends.Character
{
    public class Controller2D : MonoBehaviour
    {
        [Header("Layers")]
        [SerializeField] private LayerMask _whatIsGround;
        [SerializeField] private LayerMask _whatIsLadder;

        [Header("Transfrom for Physics Check")]
        [SerializeField] private Transform _upCheck;
        [SerializeField] private Transform _bottomCheck;
        [SerializeField] private Transform _middleCheck;
        //[SerializeField] private Transform _rightCheck;
        //[SerializeField] private Transform _leftCheck;

        [Header("Physics")]
        [SerializeField] private Collider2D _crouchDisable;
        [SerializeField] private float _jumpForce = 400f;
        [Range(0, 1)] [SerializeField] private float _crouchSpeed = 0.5f;
        [Range(0, .25f)] [SerializeField] private float _movementSmoothness = .05f;
        [SerializeField] private bool _allowedAirControl = false;
        //[SerializeField] private bool _allowedToClimbLadder = false;

        enum CharacterFacing { RIGHT, LEFT };

        const float CHECK_RADIUS = .25f;
        private bool _isGrounded;
        private bool _isClimbing;
        private Rigidbody2D _rigidbody2D;
        private CharacterFacing _facing = CharacterFacing.RIGHT;
        private Vector3 _velocity = Vector3.zero;

        [Header("Events")]
        public UnityEvent OnLandEvent;
        [System.Serializable]
        public class BoolEvent : UnityEvent<bool> { }

        public BoolEvent OnCrouchEvent;
        private bool _wasCroucing = false;
        private bool _wasClimbing = false;

        private void Awake()
        {
            _rigidbody2D = GetComponent<Rigidbody2D>();
            if (OnLandEvent == null)
                OnLandEvent = new UnityEvent();
            if (OnCrouchEvent == null)
                OnCrouchEvent = new BoolEvent();
        }

        private void FixedUpdate()
        {
            bool _wasGrounded = _isGrounded;
            _isGrounded = false;

            Collider2D[] colls = Physics2D.OverlapCircleAll(_bottomCheck.position,
                CHECK_RADIUS, _whatIsGround);
            for (int i = 0; i < colls.Length; i++)
            {
                if (colls[i].gameObject != gameObject)
                {
                    _isGrounded = true;
                    if (!_wasGrounded) OnLandEvent?.Invoke();
                }
            }

            //_wasClimbing = _isClimbing;
            _isClimbing = false;
            Collider2D[] colls2 = Physics2D.OverlapCircleAll(_middleCheck.position,
                CHECK_RADIUS, _whatIsLadder);
            for (int i = 0; i < colls2.Length; i++)
            {
                if (colls2[i].gameObject != gameObject)
                    _isClimbing = true;
            }

            if (_wasClimbing)
            {
                if (_isGrounded && _isClimbing) _isClimbing = false;
            }
            else _wasClimbing = _isClimbing;
            
            

        }

        public void Move(float _horizontalMove, float _verticalMove,
            bool _crouch, bool _jump)
        {
            if (_isClimbing) _rigidbody2D.simulated = false;
            else _rigidbody2D.simulated = true;

            if (!_crouch)
            {
                if (Physics2D.OverlapCircle(_upCheck.position,
                    CHECK_RADIUS, _whatIsGround)) _crouch = true;
                if (_isClimbing) _crouch = false;
            }

            if (_isGrounded || _allowedAirControl)
            {
                if (_crouch)
                {
                    _wasCroucing = true;
                    OnCrouchEvent?.Invoke(true);
                    _horizontalMove *= _crouchSpeed;
                    if (_crouchDisable != null)
                    {
                        _crouchDisable.enabled = false;
                        //_crouchDisable.gameObject.SetActive(false);
                    }
                }
                else
                {
                    if (_crouchDisable != null)
                        //_crouchDisable.gameObject.SetActive(true);
                        _crouchDisable.enabled = true;
                    if (_wasCroucing)
                    {
                        _wasCroucing = false;
                        OnCrouchEvent?.Invoke(false);
                    }
                }
            }
            

            Vector3 _vectorChange = Vector3.zero;
            
            switch (_isClimbing)
            {
                case true:
                    _vectorChange.x = _horizontalMove /5f;
                    _vectorChange.y = _verticalMove * 10f;
                    transform.Translate(_vectorChange);
                    break;
                case false:
                    _vectorChange.x = _horizontalMove * 10f;
                    _vectorChange.y = _rigidbody2D.velocity.y;
                    _rigidbody2D.velocity = Vector3.SmoothDamp(_rigidbody2D.velocity,
                        _vectorChange, ref _velocity, _movementSmoothness);
                    break;
            }

            if (_horizontalMove > 0 && _facing == CharacterFacing.LEFT) Flip();
            else if (_horizontalMove < 0 && _facing == CharacterFacing.RIGHT) Flip();

            if (_isGrounded && _jump)
            {
                _isGrounded = false;
                _rigidbody2D.AddForce(new Vector2(0f, _jumpForce));
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
