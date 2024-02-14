using UnityEngine;

public class Enemies : Entity
{
    protected bool isDamaging = false;

    public virtual void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject == Hero.Instance.gameObject)
        {
            isDamaging = true;
            ApplyDamage();
        }

    }
    protected void OnCollisionExit2D(Collision2D collision)
    {
        isDamaging = false;
    }

    protected void ApplyDamage()
    {
        if (isDamaging)
        {
            Hero.Instance.GetDamage();
            Invoke("ApplyDamage", 1f);
        }
        
    }
}
