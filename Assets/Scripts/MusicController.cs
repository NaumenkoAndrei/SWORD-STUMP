using UnityEngine;

public class MusicController : MonoBehaviour
{
    private static MusicController instance;

    public void Awake()
    {
        if (instance != null)
            Destroy(gameObject);
        else
        {
            instance = this;
            DontDestroyOnLoad(transform.gameObject);
        }
    }


}
