using System.Collections;
using UnityEngine;

public class Enemies : Entity
{
    [SerializeField] protected Transform player;
    [SerializeField] protected float attackRange;
    [SerializeField] protected float attackCooldown;
    protected float lastAttackTime;

    public bool CanAttackPlayer()
    {
        return Vector2.Distance(transform.position, player.position) <= attackRange;
    }

    public IEnumerator ExecuteAttack()
    {
        if (getDamage)
            yield break;

        sprite.flipX = player.position.x < transform.position.x;
        State = States.attack;
        yield return new WaitForSeconds(0.5f);

        if (CanAttackPlayer())
            Hero.Instance.GetDamage();

        lastAttackTime = Time.time;
    }

}