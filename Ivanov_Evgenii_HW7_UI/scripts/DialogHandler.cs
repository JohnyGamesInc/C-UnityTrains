using UnityEngine;
using System.Collections.Generic;


namespace Game2D
{
    public sealed class DialogHandler : MonoBehaviour
    {

        #region Fields

        [SerializeField] private GameObject _dialogPanel;
        private TMPro.TextMeshProUGUI _textArea;

        private List<string> _texts;
        private int _msgPointer = 0;
        private float _messageLag = 0.0f;
        private float _messageRepeater = 3.0f;
        private bool _isDialog;

        #endregion


        #region UnityMethods  

        private void Start()
        {
            DialigInitialization();
        }

        private void Update()
        {
            if (_isDialog)
            {
                UpdateText();
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                _dialogPanel.SetActive(true);
                _textArea = gameObject.GetComponentInChildren<TMPro.TextMeshProUGUI>(true);
                _isDialog = true;
            }
        }

        #endregion


        #region Methods

        private void UpdateText()
        {
            _messageLag += Time.deltaTime;
            if (_messageLag >= _messageRepeater)
            {
                if (_msgPointer < _texts.Count)
                {
                    _textArea.SetText(_texts[_msgPointer]);
                    _msgPointer++;
                    _messageLag = 0;
                }
                else
                {
                    _isDialog = false;
                    _dialogPanel.SetActive(false);
                }
            }
        }

        private void DialigInitialization()
        {
            _texts = new List<string>() {
            "Do you wanna become hero?!",
            "I know, of course u wanna, Ha ha",
            "I have a task for you",
            "Firts: Find 2 buttons to pass through gates",
            "Then destroy all enemies on your way!"
        };
        }

        #endregion

    }
}