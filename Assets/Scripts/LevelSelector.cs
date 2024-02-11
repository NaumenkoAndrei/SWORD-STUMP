using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LevelSelector : MonoBehaviour
{
    public int levelsAvailable = 1;

    public Button[] levels;
    void Start()
    {
        int levelReached = PlayerPrefs.GetInt("levelReached", levelsAvailable);

        for (int i = 0; i < levels.Length; i++)
            if (i + 1 > levelReached)
                levels[i].interactable = false;
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
