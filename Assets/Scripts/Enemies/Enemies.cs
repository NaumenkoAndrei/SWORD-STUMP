using System.Collections;
using UnityEngine;

public class Enemies : Entity
{
    [SerializeField] protected Transform _player;
    [SerializeField] private AudioSource _deadSound;
    [SerializeField] protected float _attackRange;
    [SerializeField] protected float _attackCooldown;
    protected float _lastAttackTime;

    public bool CanAttackPlayer()
    {
        return Vector2.Distance(transform.position, _player.position) <= _attackRange;
    }

    public IEnumerator ExecuteAttack()
    {
        if (_getDamage)
            yield break;

        _sprite.flipX = _player.position.x < transform.position.x;
        State = States.attack;
        yield return new WaitForSeconds(0.5f);

        if (CanAttackPlayer())
            Hero.Instance.GetDamage();

        _lastAttackTime = Time.time;
    }

    public virtual void GetDamage()
    {
        if (_getDamage) return;

        _getDamage = true;

        _lives--;
        if (_lives <= 0)
        {
            StartCoroutine(Die());
            return;
        }

        _getDamage = false;
    }

    public IEnumerator Die(float timeUntilDeletion = 1.5f)
    {
        State = States.dead;
        _deadSound.Play();
        yield return new WaitForSeconds(timeUntilDeletion);
        Destroy(gameObject);
    }
}