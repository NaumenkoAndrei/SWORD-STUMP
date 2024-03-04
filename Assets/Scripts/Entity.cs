using UnityEngine;

public class Entity : MonoBehaviour
{
    protected int _lives;
    protected bool _getDamage = false;

    protected Animator _animate;
    protected SpriteRenderer _sprite;

    public States State
    {
        get { return (States)_animate.GetInteger("state"); }
        set { _animate.SetInteger("state", (int)value); }
    }

    private void Awake()
    {
        _animate = GetComponent<Animator>();
        _sprite = GetComponentInChildren<SpriteRenderer>();
    }

    public void OnDestroy()
    {
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