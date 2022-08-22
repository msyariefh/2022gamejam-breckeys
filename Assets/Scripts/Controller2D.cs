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
        [SerializeField] private Transform _rightCheck;
        [SerializeField] private Transform _leftCheck;

        [Header("Physics")]
        [SerializeField] private Collider2D _crouchDisable;
        [SerializeField] private float _jumpForce = 400f;
        [Range(0, 1)] [SerializeField] private float _crouchSpeed = 0.5f;
        [Range(0, .25f)] [SerializeField] private float _movementSmoothness = .05f;
        [SerializeField] private bool _allowedAirControl = false;
        [SerializeField] private bool _allowedToClimbLadder = false;

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

            bool _wasClimbing = _isClimbing;
            _isClimbing = false;
            Collider2D[] colls2 = Physics2D.OverlapCircleAll(_middleCheck.position,
                CHECK_RADIUS, _whatIsLadder);
            

        }

    }
}
