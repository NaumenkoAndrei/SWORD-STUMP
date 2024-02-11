using UnityEngine;

public class Enemies : Entity
{
    private bool isDamaging = false;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject == Hero.Instance.gameObject)
        {
            isDamaging = true;
            ApplyDamage();
        }

    }
    private void OnCollisionExit2D(Collision2D collision)
    {
        isDamaging = false;
    }
        
    private void ApplyDamage()
    {
        if (isDamaging)
        {
            Hero.Instance.GetDamage();
            Invoke("ApplyDamage", 1f);
        }
        
    }
}
