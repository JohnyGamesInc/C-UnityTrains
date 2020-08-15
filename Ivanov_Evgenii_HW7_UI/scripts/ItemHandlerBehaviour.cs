using UnityEngine;


namespace Game2D
{
    public sealed class ItemHandlerBehaviour : MonoBehaviour
    {

        #region Fields

        private Animator _animator;
        private Item _currentItem;
        private GameObject _aimRay;
        private GameObject _player;
        private Transform _leftHand;

        private RaycastHit _hitRay;
        private Vector3 _targetAimDirection;

        private bool _isAiming;
        private int _preventCounter;
        private bool _isRay;      
        private float _rayLength = 40.0f;

        private bool _isCurrentItemChanged;

        #endregion


        #region Properties

        public Item ItemInHand
        {
            get => _currentItem;
            set => _currentItem = value;
        }

        public bool IsCurrentItemChanged
        {
            set => _isCurrentItemChanged = value;
        }

        #endregion


        #region UnityMethods

        private void Start()
        {
            _animator = GetComponentInParent<Animator>();
            _aimRay = GameObject.FindGameObjectWithTag("AimRay");
            _currentItem = GetComponentInParent<InventoryControl>().CurrentItem;
            _targetAimDirection = _aimRay.transform.up * _rayLength - Vector3.up * 3;
            _isRay = Physics.Raycast(_aimRay.transform.up, _targetAimDirection, out _hitRay, 1 << 8);
        }

        private void Update()
        {
            if (_player == null)
            {
                _player = GameObject.FindGameObjectWithTag("Player");
            }

            AimingAndShooting();

            if (_isCurrentItemChanged)
            {
                TriggerWeaponInicialization();
            }
        }

        private void TriggerWeaponInicialization()
        {
            switch (_currentItem.Type)
            {
                case ItemType.Weapon:
                    _currentItem.GameItem.GetComponent<IDamagable>().IsItemInHand = true;
                    break;

                default:
                    break;
            }
        }

        private void AimingAndShooting()
        {
            if (_currentItem != null && _currentItem.GameItem != null)
            {
                if (_currentItem.GameItem.activeSelf)
                {
                    if (_isAiming)
                    {
                        //_bulletSpawn = GameObject.FindGameObjectWithTag("BulletSpawn");
                        _targetAimDirection = _aimRay.transform.up * _rayLength - Vector3.up * 3;
                        Debug.DrawRay(_aimRay.transform.position, _targetAimDirection, Color.red);
                    }

                    if (_currentItem.Type.Equals(ItemType.Weapon))
                    {
                        if (!_isAiming && Input.GetMouseButton(1))
                        {
                            _preventCounter++;
                            _isAiming = true;

                            if (_preventCounter == 1)
                            {
                                _animator.SetBool("isAiming", _isAiming);
                            }
                        }

                        if (_isAiming && Input.GetMouseButtonDown(0))
                        {
                            var weapon = _currentItem.GameItem;
                            {
                                var weaponBehaviour = weapon.GetComponentInChildren<EnergyRevolver>();
                                weaponBehaviour.Shoot();

                            }
                        }

                        if (_isAiming && Input.GetMouseButtonUp(1))
                        {
                            _isAiming = false;
                            _animator.SetBool("isAiming", _isAiming);
                            _preventCounter = 0;
                        }
                    }
                }
            }
        }

        #endregion

    }
}