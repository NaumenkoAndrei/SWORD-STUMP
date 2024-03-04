using System.Collections;
using UnityEngine;

public class Goblin : Enemies
{
    [SerializeField] private LayerMask _playerLayer;
    [SerializeField] private float _moveSpeed = 5f;
    [SerializeField] private float _viewDistance = 10f;
    [SerializeField] private float _deathPushForce = 13f;
    [SerializeField] private LayerMask _walkingEnemy;
    [SerializeField] private LayerMask _standingEnemy;


    private Rigidbody2D _rigidbody;
    private Vector2 _direction;

    private void Start()
    {
        _attackRange = 1.5f;
        _attackCooldown = 1f;
        _lives = 3;
        _rigidbody = GetComponent<Rigidbody2D>();

        StartCoroutine(BehaviorPattern());
    }

    public override void GetDamage()
    {
        _getDamage = true;
        _lives--;
        if (_lives <= 0)
        {    
            _direction = transform.position - _player.position;
            _rigidbody.AddForce(_direction.normalized * _deathPushForce, ForceMode2D.Impulse);
            StartCoroutine(Die());
            return;
        }

        _getDamage = false;
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
                
            if (CanSeePlayer())
            {
                if (!CanAttackPlayer())
                    MoveTowardsPlayer();
                else if (CanAttackPlayer() && Time.time > _lastAttackTime + _attackCooldown)
                    yield return StartCoroutine(ExecuteAttack());
                else
                {
                    State = States.idle;
                    yield return new WaitForSeconds(0.35f);
                }
            }
            else
            {
                State = States.idle;
                yield return new WaitForSeconds(0.35f);
            }
        }
    }

    private bool CanSeePlayer()
    {
        _direction = _player.position - transform.position;
        RaycastHit2D hit = Physics2D.Raycast(transform.position, _direction, _viewDistance, LayerMask.GetMask("Default") | _playerLayer | _standingEnemy);

        return hit.collider != null && hit.collider.CompareTag("Hero");
    }

    private void MoveTowardsPlayer()
    {
        if (_getDamage)
            return;
        
        State = States.run;
        transform.position = Vector3.MoveTowards(transform.position, _player.position, _moveSpeed * Time.deltaTime);
        _sprite.flipX = _player.position.x < transform.position.x;
    }
}
