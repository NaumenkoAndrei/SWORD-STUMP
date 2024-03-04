using UnityEngine;

public class MusicController : MonoBehaviour
{
    private static MusicController _instance;

    public void Awake()
    {
        if (_instance != null)
            Destroy(gameObject);
        else
        {
            _instance = this;
            DontDestroyOnLoad(transform.gameObject);
        }
    }
}
