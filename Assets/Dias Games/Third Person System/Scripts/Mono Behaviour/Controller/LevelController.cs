using UnityEngine;
using DiasGames.Components;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using DG.Tweening;

namespace DiasGames.Controller
{
    public class LevelController : MonoBehaviour
    {
        [SerializeField] private GameObject player = null;
        [SerializeField] private float delayToRestartLevel = 3f;
        [SerializeField] private Image fadeImage = null;
        [SerializeField] private Light directionalLight = null;

        // player components
        private Health _playerHealth;

        // controller vars
        private bool _isRestartingLevel;

        private void Awake()
        {
            if (player == null)
                player = GameObject.FindGameObjectWithTag("Player");

            _playerHealth = player.GetComponent<Health>();
            fadeImage.gameObject.SetActive(true);
            fadeImage.DOColor(new Color(0, 0, 0, 0), 1f).SetEase(Ease.Linear).OnComplete(() =>
            {
                fadeImage.gameObject.SetActive(false);
            });
        }

        private void OnEnable()
        {
            _playerHealth.OnDead += RestartLevel;
        }
        private void OnDisable()
        {
            _playerHealth.OnDead -= RestartLevel;
        }

        // Restarts the current level
        public void RestartLevel()
        {
            Debug.Log("RestartLevel");
            if (!_isRestartingLevel)
                StartCoroutine(OnRestart());
        }

        public void LoadScene(string name)
        {
            SceneManager.LoadScene(name);
        }

        private IEnumerator OnRestart()
        {
            _isRestartingLevel = true;
            fadeImage.gameObject.SetActive(true);
            DOTween.To(() => directionalLight.intensity, x => directionalLight.intensity = x, 0, 1f).SetEase(Ease.Linear).OnComplete(() =>
            {
                fadeImage.DOColor(new Color(0, 0, 0, 1), delayToRestartLevel-1f).SetEase(Ease.Linear);
            });


            yield return new WaitForSeconds(delayToRestartLevel);

            SceneManager.LoadScene(SceneManager.GetActiveScene().name);

            _isRestartingLevel = false;

        }
    }
}