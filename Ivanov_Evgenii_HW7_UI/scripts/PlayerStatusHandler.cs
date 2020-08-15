using UnityEngine;


namespace Game2D
{
    public sealed class PlayerStatusHandler : MonoBehaviour, IDieble
    {

        [SerializeField] private float _health;

        private EventHandler _eventHandler;

        public float Health
        {
            get => _health;
            set
            {
                _eventHandler.TriggerEvent(EventType.DMG_RECEIVE, "" + (_health - value));
                _health = value; 
            }
        }

        private void Start()
        {
            _eventHandler = GameObject.FindGameObjectWithTag("GUI").GetComponent<EventHandler>();

        }

        private void Update()
        {       
        }

        public void Die()
        {
            print("U DEAD!!!");
        }
    }

}