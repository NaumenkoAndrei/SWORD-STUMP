using System.Collections;
using UnityEngine;

public class Skeleton : Enemies
{
    [SerializeField] private float _speed = 2.5f;
    [SerializeField] private float _yOffset = 0.5f;
    [SerializeField] private float _xOffsetMultiplier = 0.7f;
    [SerializeField] private float _overlapCircleRadius = 0.1f;
    private Vector3 _overlapCirclePosition;
    private bool _collidedWithSomething;
    private Collider2D[] _colliders;

    private Vector3 _direction;

    private void Start()
    {
        _direction = transform.right;
        _lives = 6;

        _sprite = GetComponentInChildren<SpriteRenderer>();
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

            // ¬ычисл€ем точку, вокруг которой будет происходить поиск коллайдеров
            _overlapCirclePosition = transform.position + transform.up * _yOffset + transform.right * _direction.x * _xOffsetMultiplier;

            // ¬ыполн€ем поиск коллайдеров в определенной области
            _colliders = Physics2D.OverlapCircleAll(_overlapCirclePosition, _overlapCircleRadius);

            _collidedWithSomething = _colliders.Length > 0;

            // ≈сли столкнулс€ с каким-то объектом, мен€ем направление движени€
            if (CanAttackPlayer())
            {
                if (Time.time > _lastAttackTime + _attackCooldown)
                    yield return StartCoroutine(ExecuteAttack());
                else
                    State = States.idle;
            }
            else
                Move(_collidedWithSomething);
                       
        }

    }

    private void Move(bool collidedWithSomething)
    {
        if (_getDamage)
            return;

        State = States.run;

        if (collidedWithSomething)
            _direction *= -1f;

        transform.position = Vector3.MoveTowards(transform.position, transform.position + _direction, _speed* Time.deltaTime);
        _sprite.flipX = _direction.x < 0.0f;
    }
}