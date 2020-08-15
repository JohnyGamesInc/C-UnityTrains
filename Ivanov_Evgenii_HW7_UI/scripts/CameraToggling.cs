using UnityEngine;


namespace Game2D
{
    public sealed class CameraToggling : MonoBehaviour
    {
        private Camera _mainCamera;
        private Camera _currentCamera;
        private bool _isCameraToggled;

        private void Start()
        {
            _mainCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        }

        private void OnTriggerStay(Collider other)
        {
            if (other.name.Equals("CameraTrigger"))
            {
                _currentCamera = other.gameObject.transform.parent.gameObject.GetComponentInChildren<Camera>();
                _currentCamera.enabled = true;
                _mainCamera.enabled = false;
                _isCameraToggled = true;
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.name.Equals("CameraTrigger"))
            {
                _currentCamera.enabled = false;
                _mainCamera.enabled = true;
                _isCameraToggled = false;
            }
        }
    }
}