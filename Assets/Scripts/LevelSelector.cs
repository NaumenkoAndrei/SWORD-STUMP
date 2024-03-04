using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LevelSelector : MonoBehaviour
{
    public int LevelsAvailable = 1;
    private int _levelReached;

    public Button[] Levels;
    void Start()
    {
        _levelReached = PlayerPrefs.GetInt("levelReached", LevelsAvailable);

        for (int i = 0; i < Levels.Length; i++)
            if (i + 1 > _levelReached)
                Levels[i].interactable = false;
    }

    public void ToMenu()
    {
        SceneManager.LoadScene(0);
    }

    public void Select(int numberInBuild)
    {
        SceneManager.LoadScene(numberInBuild);
        Destroy(GameObject.Find("Audio Source"));
    }
}
