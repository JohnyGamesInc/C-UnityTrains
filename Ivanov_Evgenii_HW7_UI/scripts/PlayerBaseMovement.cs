using UnityEngine;


namespace Game2D
{
    public sealed class PlayerBaseMovement : MonoBehaviour
    {
        #region Fields

        [SerializeField] private Vector3 _forceVector;
        [SerializeField] private float _turnSpeed;
        [SerializeField] private float _moveSpeed;
        [SerializeField] private float _jumpSpeed;
        [SerializeField] private bool _isJumpPressed;
        [SerializeField] private float _fallSpeed;

        private Animator _animator;
        private Rigidbody _rigidbody;

        private Vector3 _movement;
        private Vector3 _jumpMovement;
        private Vector3 _direction;
        private Vector3 _rotation;

        private float _acсeleration;
        private float _rotAcceleration;
        private float _turnMovement;
        private bool _isJumpInProcess;
        private bool _isMoving;
        private Vector3 _rot;

        private bool _isRayCast;
        private RaycastHit _rayHit;
        private float _rayLength = 1.0f;
        private LayerMask _mask = 1 << 9;     

        #endregion


        #region Properties

        public Animator Animator
        {
            get => _animator;
        }

        #endregion


        #region UnityMethods

        private void Start()
        {
            _animator = GetComponent<Animator>();
            _rigidbody = GetComponent<Rigidbody>();
            _isJumpPressed = false;
        }

        private void Update()
        {
            SetAnimationVariables();
            UpdateDirections();
        }

        private void FixedUpdate()
        {
            if (!_isJumpPressed)
            {
                MoveAndRotate();
            }

            Jump();

            _isRayCast = Physics.Raycast(transform.position, Vector3.down, _rayLength, _mask, QueryTriggerInteraction.Collide);
            Debug.DrawRay(transform.position, Vector3.down * _rayLength, Color.red);

            if (!_isRayCast && !_isJumpInProcess)
            {
                _rigidbody.AddForce(_forceVector * (-1) * _fallSpeed, ForceMode.Impulse);
            }
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject.CompareTag("Map") && _isJumpPressed == true && _isJumpInProcess == true)
            {
                _isJumpPressed = false;
                _isJumpInProcess = false;
            }
        }

        #endregion


        #region Methods

        private void MoveAndRotate()
        {
            _rigidbody.velocity = _movement * Time.fixedDeltaTime;
        }

        private void UpdateDirections()
        {
            float horizontal = Input.GetAxis("Horizontal");
            float vertical = Input.GetAxis("Vertical");

            if (Input.GetKeyDown(KeyCode.Space))
                _isJumpPressed = true;

            if (!_isJumpPressed)
            {
                _direction.Set(vertical, 0f, vertical);
                _direction.Normalize();
                _acсeleration = (_direction.sqrMagnitude > 0) ? _moveSpeed : 0;
                _movement = transform.forward * _acсeleration * Mathf.Sign(_direction.x);

                _rotation.Set(0.0f, horizontal, 0.0f);
                _rotation.Normalize();
                _rotAcceleration = (_rotation.sqrMagnitude > 0) ? _turnSpeed : 0;
                _rot = transform.forward * _rotAcceleration;
                _rot = transform.up * _rotAcceleration * Mathf.Sign(_rotation.y);
                transform.Rotate(_rot * Time.deltaTime);

                bool hasHorizontalInput = !Mathf.Approximately(horizontal, 0f);
                bool hasVerticalInput = !Mathf.Approximately(vertical, 0f);

                _isMoving = hasHorizontalInput || hasVerticalInput;
            }
        }

        public void Die()
        {
           
        }

        private void Jump()
        {
            if (_isJumpPressed)
            {
                _jumpMovement = (transform.forward * Mathf.Sign(_direction.x) + _forceVector) * _jumpSpeed;
                _rigidbody.transform.position += _jumpMovement * Time.fixedDeltaTime;

                _isJumpInProcess = true;
            }
        }

        private void SetAnimationVariables()
        {
            _animator.SetBool("isJumpPressed", _isJumpPressed);
            _animator.SetBool("isMoving", _isMoving);
        }

        private void StabilizeFall()
        {

        }

        #endregion
    }
}

