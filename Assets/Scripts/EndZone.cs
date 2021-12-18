using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class EndZone : MonoBehaviour
{
    private float _delay = 2f;

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out PlayerMover player))
        {
            player.GetComponentInChildren<ParticleSystem>().Play();
            Invoke(nameof(FadeAnimation), _delay);
        }
    }

    private void FadeAnimation()
    {
        NavigationButtons.Instance.FadeInScene();
        Invoke(nameof(RestartLevel), _delay);
    }

    private void RestartLevel()
    {
        SceneManager.LoadScene(0);
    }
}
