using UnityEngine;


public sealed class BoxesHandler : MonoBehaviour
{
    [SerializeField] private GameObject _box;

    private Animator _boxAnimator;
    private bool _wasPressed;
    private int _boxPressedCounter;


    public int PressedECounter
    {
        get => _boxPressedCounter;
    }

    private void Start()
    {
        _boxAnimator = _box.GetComponent<Animator>();
    }

    private void Update()
    {
        //if (Input.GetKeyDown(KeyCode.E))
        //{
        //    _wasPressed = true;
        //    _boxPressedCounter++;
        //}
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                _wasPressed = true;
                _boxPressedCounter++;

                if (_wasPressed)
                {
                    _boxAnimator.SetTrigger("openCloseTrigger");
                    _wasPressed = false;
                }
            }

        }


    }
}
