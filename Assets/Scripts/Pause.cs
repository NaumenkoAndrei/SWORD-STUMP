using UnityEngine;
using UnityEngine.SceneManagement;

public class Pause : MonoBehaviour
{
    [SerializeField] private GameObject _pausePanel;

    private void Awake()
    {
        _pausePanel.SetActive(false);
    }

    public void SetPause()
    {
        _pausePanel.SetActive(true);
        Time.timeScale = 0;
    }

    public void PauseOff()
    {
        _pausePanel.SetActive(false);
        Time.timeScale = 1;
    }

    public void ToMenu()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(0);
    }
}
