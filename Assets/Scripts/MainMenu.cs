using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void PlayCurrentLevel()
    {

    }

    public void OpenLevelsList()
    {
        SceneManager.LoadScene(1);
    }

}
