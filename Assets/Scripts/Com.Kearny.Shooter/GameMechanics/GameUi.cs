using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Com.Kearny.Shooter.GameMechanics
{
    public class GameUi : MonoBehaviour
    {
        public Image fadePlane;
        public GameObject gameOverUi;

        private Camera _mainCamera;

        private void Start()
        {
            _mainCamera = GameObject.FindWithTag("MainCamera").GetComponent<Camera>();
            _mainCamera.enabled = false;
            
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            
            FindObjectOfType<Player.Player>().OnDeath += OnGameOver;
        }

        private void OnGameOver()
        {
            _mainCamera.enabled = true;
            
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            
            StartCoroutine(Fade(Color.clear, Color.black, 1));
            gameOverUi.SetActive(true);
        }

        private IEnumerator Fade(Color from, Color to, float time)
        { 
            to.a = .5f;

            var speed = 1 / time;
            var percent = 0f;

            while (percent < 1)
            {
                percent += Time.deltaTime * speed;
                fadePlane.color = Color.Lerp(from, to, percent);
                yield return null;
            }
        }

        // UI Input
        public void StartNewGame()
        {
            _mainCamera.enabled = false;
            
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            
            SceneManager.LoadScene("FirstScene");
        }
    }
}