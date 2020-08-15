using UnityEngine;


namespace Game2D
{
    public sealed class EnergyRevolver : MonoBehaviour, IDamagable
    {

        [SerializeField] private GameObject _bullet;
        [SerializeField] private GameObject _bulletSpawn;
        [SerializeField] private float _damage;
        [SerializeField] private float _timeout;
        [SerializeField] private float _startShootSpeed;

        private EventHandler _eventHandler;
        private InventoryControl _inventoryControl;
        private GameObject _newBullet;
        private Item _ammo;

        private int _magazine;
        private bool _isEmpty;
        private float _time;
        private bool _isAimedAtFirstTime;
        private bool _isItemInHand;


        public int Magazine
        {
            get => _magazine;
        }

        public bool AimedAtFirstTime
        {
            set => _isAimedAtFirstTime = value;
        }

        public bool IsItemInHand
        {
            set => _isItemInHand = value;
        }

        private void Start()
        {
            _eventHandler = GameObject.FindGameObjectWithTag("GUI").GetComponent<EventHandler>();
            _inventoryControl = GameObject.FindGameObjectWithTag("Player").GetComponentInParent<InventoryControl>();
            _isEmpty = true;
        }

        private void Update()
        {
            _time += Time.deltaTime;

            if (_isItemInHand && !_isAimedAtFirstTime)
            {
                AimRevolverWhenJustTakeIt();
            }

            if (_isItemInHand && Input.GetKeyDown(KeyCode.E))
            {
                if (this.gameObject.activeSelf && _isEmpty)
                {
                    Reload();
                }
            }

        }

        private void AimRevolverWhenJustTakeIt()
        {
            _isAimedAtFirstTime = true;

            if (this.gameObject.activeSelf && _isEmpty)
            {
                Reload();
            }
        }

        public void Damage(IDieble enemy)
        {
            var commonDamage = _damage + _bullet.GetComponent<BulletBehaviour>().BulletDamage;
            enemy.Health -= commonDamage;

            var enemyTag = ((Component)enemy).transform.gameObject.tag;
            _eventHandler.TriggerEvent(EventType.DMG_ENEMY, $"{enemyTag} {commonDamage}");

            if (enemy.Health <= 0)
            {
                enemy.Die();
                _eventHandler.TriggerEvent(EventType.SMB_KILLED, $"{enemyTag} {commonDamage}");
            }
        }

        public void Shoot()
        {
            if (_magazine <= 0)
            {
                _isEmpty = true;
                _eventHandler.TriggerEvent(EventType.INFO, "MAGAZINE IS EMPTY! \n NEED TO RELOAD");
            }

            if (_time >= _timeout && !_isEmpty)
            {
                var transform = _bulletSpawn.transform;

                _newBullet = Instantiate(_bullet, _bulletSpawn.transform.position, _bulletSpawn.transform.rotation);
                _newBullet.transform.SetParent(_bulletSpawn.transform, true);

                var rigidBody = _newBullet.GetComponent<Rigidbody>();
                var impulse = _bulletSpawn.transform.forward * rigidBody.mass * _startShootSpeed;
                rigidBody.AddForce(impulse, ForceMode.Impulse);
                _magazine--;
                _ammo.Count--;
                _time = 0.0f;

                _eventHandler.TriggerEvent(EventType.AMMO_DISCHARGE, "-1 Bullet");
            }
        }

        private void Reload()
        {

            if (_ammo == null)
            {
                _ammo = _inventoryControl.GetAmmoForCurrentWeapon();

                if (_ammo != null)
                {
                    if (_ammo.Count <= 8)
                    {
                        _magazine = _ammo.Count;
                        _isEmpty = false;
                        _eventHandler.TriggerAmmoBarRecalc();
                        _eventHandler.TriggerEvent(EventType.INFO, "Reloaded!");
                    }
                    else
                    {
                        _magazine = 8;
                        _isEmpty = false;
                        _eventHandler.TriggerAmmoBarRecalc();
                        _eventHandler.TriggerEvent(EventType.INFO, "Reloaded!");
                    }
                }
            }
            else
            {
                if (_ammo.Count > 0 && _ammo.Count <= 8)
                {
                    _magazine = _ammo.Count;
                    _isEmpty = false;
                    _eventHandler.TriggerAmmoBarRecalc();
                    _eventHandler.TriggerEvent(EventType.INFO, "Reloaded!");
                }
                else if (_ammo.Count > 8)
                {
                    _magazine = 8;
                    _isEmpty = false;
                    _eventHandler.TriggerAmmoBarRecalc();
                    _eventHandler.TriggerEvent(EventType.INFO, "Reloaded!");
                }
                else if (_ammo.Count <= 0)
                {
                    RemoveEmptyAmmoObjectFromPlayer();

                    _ammo = _inventoryControl.GetAmmoForCurrentWeapon();
                    _isEmpty = true;
                }
            }
        }

        private void RemoveEmptyAmmoObjectFromPlayer()
        {
            var empty = _inventoryControl.AllPlayerItems.Find(e => e.Count == 0);
            _inventoryControl.AllPlayerItems.Remove(empty);
        }
    }
}