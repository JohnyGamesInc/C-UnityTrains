using UnityEngine;


namespace Game2D
{
    public class BulletBehaviour : MonoBehaviour
    {
        [SerializeField] private float _energyBulletDamage;
      
        private GameObject _parentBox;
        private Transform _mapObject;


        private void Start()
        {
            _parentBox = transform.parent.parent.gameObject;
            _mapObject = GameObject.FindGameObjectWithTag("Map").gameObject.transform;          
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (!_parentBox.CompareTag("AmmoBox"))
            {
                Destroy(gameObject, 10.0f);

                if (collision.gameObject.CompareTag("Turret"))
                {
                    var gun = gameObject.GetComponentInParent<EnergyRevolver>();
                    gun.Damage(collision.gameObject.GetComponent<GatlingGun>());                   
                }
                gameObject.transform.SetParent(_mapObject);
            }
        }

        public float BulletDamage
        {
            get => _energyBulletDamage;
        }
    }
}