using UnityEngine;
using System.Collections;

public class Entity : MonoBehaviour
{
    protected int lives;
    protected bool getDamage;

    protected Animator animate;
    protected SpriteRenderer sprite;


    private void Start()
    {
        getDamage = false;
    }

    public States State
    {
        get { return (States)animate.GetInteger("state"); }
        set { animate.SetInteger("state", (int)value); }
    }

    private void Awake()
    {
        animate = GetComponent<Animator>();
        sprite = GetComponentInChildren<SpriteRenderer>();
    }

    public virtual void GetDamage()
    {
        getDamage = true;
        lives--;

        if (lives <= 0)
        {
            StartCoroutine(Die());
            return;
        }

        getDamage = false;
    }


    public IEnumerator Die(float timeUntilDeletion = 0.8f)
    {
        State = States.dead;
        yield return new WaitForSeconds(timeUntilDeletion);
        Destroy(gameObject);
    }

}

public enum States
{
    idle,
    attack,
    dead,
    run,
    jump
}