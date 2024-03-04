using UnityEngine;
using System.Collections;

public class Mushroop : Enemies
{
    private void Start()
    {
        _attackRange = 2.2f;
        _attackCooldown = 1.5f;
        _lives = 6;

        StartCoroutine(BehaviorPattern());
    }

    private IEnumerator BehaviorPattern()
    {
        while (true)
        {
            yield return null;

            if (_getDamage)
            {
                _lastAttackTime = Time.time;
                continue;
            }

            if (CanAttackPlayer() && Time.time > _lastAttackTime + _attackCooldown)
                yield return StartCoroutine(ExecuteAttack());
            else
                State = States.idle;

            _sprite.flipX = _player.position.x < transform.position.x;
        }
    }
}



