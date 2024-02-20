using UnityEngine;
using System.Collections;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class Mushroop : Enemies
{
    private void Start()
    {
        attackRange = 2.2f;
        attackCooldown = 1.5f;
        lives = 6;

        StartCoroutine(BehaviorPattern());
    }

    private IEnumerator BehaviorPattern()
    {
        while (true)
        {
            yield return null;

            if (getDamage)
            {
                lastAttackTime = Time.time;
                continue;
            }

            if (CanAttackPlayer() && Time.time > lastAttackTime + attackCooldown)
                yield return StartCoroutine(ExecuteAttack());
            else
                State = States.idle;

            sprite.flipX = player.position.x < transform.position.x;
        }
    }
}



