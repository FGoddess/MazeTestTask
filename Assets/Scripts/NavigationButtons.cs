using System.Collections;
using UnityEngine;

public class NavigationButtons : MonoBehaviour
{
    private static NavigationButtons _instance;
    public static NavigationButtons Instance => _instance;

    [SerializeField] private CanvasGroup _pausePanel;
    [SerializeField] private CanvasGroup _fadePanel;
    [SerializeField] private float _lerpDuration = 1f;

    private void Awake()
    {
        if (_instance == null)
            _instance = this;
        else if (_instance != this)
            Destroy(this.gameObject);
    }

    private void OnEnable()
    {
        Debug.Log("da");
        FadeOutScene();
    }

    public void Exit()
    {
        Application.Quit();
    }

    public void PauseGame()
    {
        StartCoroutine(FadeAnimation(0, 1, _pausePanel));
        _pausePanel.blocksRaycasts = true;

    }

    public void ContinueGame()
    {
        
        StartCoroutine(FadeAnimation(1, 0, _pausePanel));
        _pausePanel.blocksRaycasts = false;
    }

    public void FadeInScene()
    {
        StartCoroutine(FadeAnimation(0.01f, 0.99f, _fadePanel));
        _pausePanel.blocksRaycasts = true;

    }

    public void FadeOutScene()
    {
        StartCoroutine(FadeAnimation(0.99f, 0.01f, _fadePanel));
        _pausePanel.blocksRaycasts = false;
    }

    private IEnumerator FadeAnimation(float startValue, float endValue, CanvasGroup panel)
    {
        if(endValue == 0)
        {
            Time.timeScale = 1;
        }

        float timeElapsed = 0;

        while (timeElapsed < _lerpDuration)
        {
            panel.alpha = Mathf.Lerp(startValue, endValue, timeElapsed / _lerpDuration);
            timeElapsed += Time.deltaTime;

            yield return null;
        }

        panel.alpha = endValue;

        if (endValue == 1)
        {
            Time.timeScale = startValue;
        }
    }
}
