using UnityEngine;

public class Entity : MonoBehaviour
{
    protected int lives;

    public virtual void GetDamage()
    {
        lives--;
        if (lives < 1)
            Die();
    }

    public virtual void Die()
    {
        Destroy(gameObject);
    }
}


