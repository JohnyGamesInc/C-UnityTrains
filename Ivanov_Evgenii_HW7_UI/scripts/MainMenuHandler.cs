using UnityEngine.SceneManagement;
using UnityEngine;


namespace Game2D
{
    public class MainMenuHandler : MonoBehaviour
    {

        [SerializeField] private GameObject _pauseMenu;
        [SerializeField] private GameObject _wealcomePanel;
        [SerializeField] private GameObject _optionsPanel;

        private bool _toggled;

        private void Start()
        {
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                PauseGame();
            }
        }

        public void PauseGame()
        {
            if (_pauseMenu.activeSelf)
            {
                Resume();
            }
            else
            {
                Time.timeScale = 0;
                _pauseMenu.SetActive(true);
            }
        }

        public void LoadLevel(int index)
        {
            SceneManager.LoadScene(index, LoadSceneMode.Single);
            Time.timeScale = 1;
        }

        public void Exit()
        {
            Application.Quit();
        }

        public void Resume()
        {
            _pauseMenu.SetActive(false);
            Time.timeScale = 1;

        }

        public void ToggleOptionsMenu()
        {
            if (!_toggled)
            {
                _toggled = true;
                _wealcomePanel.SetActive(false);
                _optionsPanel.SetActive(true);
            }
            else
            {
                _toggled = false;
                _wealcomePanel.SetActive(true);
                _optionsPanel.SetActive(false);
            }
        }
    }
}