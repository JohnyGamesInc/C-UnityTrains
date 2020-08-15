using UnityEngine;


namespace Game2D
{
    public sealed class GateButton : MonoBehaviour
    {
        [SerializeField] private bool _isPressed;


        private EventHandler _eventHandler;
        private bool _flag;

        private void Start()
        {
            _eventHandler = GameObject.FindGameObjectWithTag("GUI").GetComponent<EventHandler>();
        }


        private void Update()
        {
            if (_isPressed && !_flag)
            {
                UpdateButtonState();
            }
        }

        private void OnTriggerStay(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                if (Input.GetKeyDown(KeyCode.E))
                {
                    _isPressed = true;
                }
            }
        }

        private void UpdateButtonState()
        {
            transform.Rotate(45.0f, 0f, 0f);
            _flag = true;
            GameObject.FindGameObjectWithTag("Gate").GetComponent<GateBehavour>().ButtonCounter += 1;
            _eventHandler.TriggerEvent(EventType.INFO, "Gate button toggled");
        }
    }
}