using UnityEngine;


namespace Game2D
{
    public class TurretBullet : MonoBehaviour
    {
        private GameObject _turret;
        private float _startShootSpeed;


        private void Start()
        {
            _turret = GameObject.FindGameObjectWithTag("Turret");
            _startShootSpeed = _turret.GetComponent<GatlingGun>().BulletStartSpeed;
            var rigidBody = gameObject.GetComponent<Rigidbody>();
            var impulse = gameObject.transform.forward * rigidBody.mass * _startShootSpeed;
            rigidBody.AddForce(impulse, ForceMode.Impulse);
        }

        private void OnCollisionEnter(Collision collision)
        {
            Destroy(gameObject, 2.0f);

            if (collision.gameObject.CompareTag("Player"))
            {
                _turret.GetComponent<GatlingGun>().Damage((IDieble)collision.gameObject.GetComponent<PlayerStatusHandler>());
            }
        }
    }
}