using UnityEngine;

public class LosePlatform : MonoBehaviour
{
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.GetComponent<Entity>())
            collision.gameObject.GetComponent<Entity>().OnDestroy();
        if (collision.gameObject.GetComponent<Hero>())
            collision.gameObject.GetComponent<Hero>().LosePanel();
    }
}
