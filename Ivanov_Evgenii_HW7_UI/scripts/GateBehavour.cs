using UnityEngine;


namespace Game2D
{
    public sealed class GateBehavour : MonoBehaviour
    {
        [SerializeField] private GameObject _gate;
        [SerializeField] private int _buttonPressedCounter;

        private EventHandler _eventHandler;
        private float _distance = 20.0f;
        private bool _isOpened;

        public int ButtonCounter
        {
            get => _buttonPressedCounter;
            set => _buttonPressedCounter = value;
        }


        private void Start()
        {
            _eventHandler = GameObject.FindGameObjectWithTag("GUI").GetComponent<EventHandler>();
        }

        private void Update()
        {
            if (_buttonPressedCounter == 2 && !_isOpened)
            {             
                _distance -= _distance * Time.deltaTime;
                _gate.transform.position = Vector3.MoveTowards(transform.position, transform.position - Vector3.right * _distance, _distance * Time.deltaTime);

                if ((transform.position - (transform.position - Vector3.right * _distance)).x <= 0.1f)
                {
                    _eventHandler.TriggerEvent(EventType.INFO, "GATE WAS OPENED - CHECK IT");
                    _isOpened = true;
                }
            }
        }
    }
}