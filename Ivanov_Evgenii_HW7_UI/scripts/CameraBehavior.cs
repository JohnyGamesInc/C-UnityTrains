using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Rendering;


namespace Game2D
{
    public sealed class CameraBehavior : MonoBehaviour
    {
        [SerializeField] private GameObject _player;
        [SerializeField] private float _zoomSpeed;
        [SerializeField] private Vector3 _zoomHeight;

        private Vector3 _deltaOffset;
        private Vector3 _movement = Vector3.zero;
        private Vector3 _playerStartPosition;
        private Vector3 _rayCamOffset;

        private RaycastHit _hit;
        private bool _rayCast;
        private Vector3 _rayDirection;
        private LayerMask _mask;
        private float _defaultHeight; //4.29 39.52 -38.78  || // -14.29683 1.49 2.509445
        private Vector3 _defaultRayOffset;
        private Camera _mainCamera;
        private Camera _currentCamera;
        private bool _isCameraToggled;

        private List<GameObject> _hitMapObjects;


        private void Start()
        {
            _mainCamera = transform.GetComponentInParent<Camera>();
            _playerStartPosition = _player.transform.position;
            _mask = 1 << 8;
            _mask = ~_mask;
            _defaultHeight = transform.position.y;
            _defaultRayOffset = new Vector3(0.0f, 0.0f - _player.transform.position.y, 0.0f);
            _hitMapObjects = new List<GameObject>();
            _rayCamOffset = new Vector3(0.0f, Mathf.Abs(_player.transform.localPosition.y) - 1.0f, 0.0f);
            _rayDirection = _player.transform.position + _rayCamOffset - transform.position;
            _rayCast = Physics.Raycast(transform.position, _rayDirection, out _hit, _rayDirection.magnitude, _mask);
        }

        private void Update()
        {
            _deltaOffset = _playerStartPosition - _player.transform.position;
            _playerStartPosition = _player.transform.position;

            _movement.Set(_deltaOffset.x, 0f, _deltaOffset.z);
            transform.position -= _movement;

            //_rayDirection = _player.transform.position + _defaultRayOffset - transform.position;
            //_rayCast = Physics.Raycast(transform.position, _rayDirection, out _hit, _rayDirection.magnitude, _mask);

            _rayDirection = _player.transform.position + _rayCamOffset - transform.position;
            _rayCast = Physics.Raycast(transform.position, _rayDirection, out _hit, _rayDirection.magnitude, _mask);
            Debug.DrawRay(transform.position, _rayDirection, Color.blue);

            if (_rayCast && _mainCamera.isActiveAndEnabled)
            {
                ChangeHitMapVisibility();
            }
            else if (_hitMapObjects.Count != 0 || !_mainCamera.isActiveAndEnabled)
            {
                ChangeHitMapVisibility();
            }
        }

        private void ChangeHitMapVisibility()
        {
            if (_rayCast && _mainCamera.isActiveAndEnabled)
            {
                var hitObject = _hit.collider.gameObject;
                var layer = hitObject.layer;
                if (LayerMask.LayerToName(layer).Equals("Map"))
                {
                    if (!_hitMapObjects.Contains(hitObject))
                    {
                        _hitMapObjects.Add(hitObject);
                        var material = hitObject.GetComponent<Renderer>().material;

                        StandardShaderUtils.ChangeRenderMode(material,  StandardShaderUtils.BlendMode.Transparent);
                        print("CHANGE TO Transparent");

                        var color = material.color;
                        color.a = 0.0f;
                        material.color = color;
                    }
                }
            }
            else if (_hitMapObjects.Count != 0 || !_mainCamera.isActiveAndEnabled)
            {
                for (int i = 0; i < _hitMapObjects.Count; i++)
                {
                    var material = _hitMapObjects[i].GetComponent<Renderer>().material;

                    StandardShaderUtils.ChangeRenderMode(material, StandardShaderUtils.BlendMode.Opaque);
                    print("CHANGE TO Opaque");

                    var color = material.color;
                    color.a = 1.0f;
                    material.color = color;
                    _hitMapObjects.Remove(_hitMapObjects[i]);
                }
            }

        }

        //private void ChangeHitMapVisibility()
        //{
        //    if (_rayCast)
        //    {
        //        var hitObject = _hit.collider.gameObject;
        //        var layer = hitObject.layer;
        //        if (LayerMask.LayerToName(layer).Equals("Map"))
        //        {
        //            if (!_hitMapObjects.Contains(hitObject))
        //            {
        //                _hitMapObjects.Add(hitObject);
        //                var material = hitObject.GetComponent<Renderer>().material;
        //                material.renderQueue = 0; // 0 - Transparent rendering mode

        //                var color = material.color;
        //                color.a = 0.0f;
        //                material.color = color;
        //            }
        //        }
        //    }
        //    else
        //    {
        //        foreach (var gameObject in _hitMapObjects)
        //        {
        //            var material = gameObject.GetComponent<Renderer>().material;
        //            material.renderQueue = -1; // -1 - Transparent rendering mode

        //            var color = material.color;
        //            color.a = 1.0f;
        //            material.color = color;
        //            _hitMapObjects.Remove(gameObject);
        //        }
        //    }

        //}
    }
}