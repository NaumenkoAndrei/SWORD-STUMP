using System.Collections;
using UnityEngine;
using UnityEngine.Scripting.APIUpdating;

public class Skeleton : Enemies
{
    [SerializeField] private float speed = 2.5f;
    [SerializeField] private float yOffset = 0.5f;
    [SerializeField] private float xOffsetMultiplier = 0.7f;
    [SerializeField] private float overlapCircleRadius = 0.1f;
    private Vector3 overlapCirclePosition;
    private bool foundHeroCollider = false;
    private bool collidedWithSomething;


    private Vector3 dir;

    private void Start()
    {
        dir = transform.right;
        lives = 6;

        sprite = GetComponentInChildren<SpriteRenderer>();
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

            // ¬ычисл€ем точку, вокруг которой будет происходить поиск коллайдеров
            overlapCirclePosition = transform.position + transform.up * yOffset + transform.right * dir.x * xOffsetMultiplier;

            // ¬ыполн€ем поиск коллайдеров в определенной области
            Collider2D[] colliders = Physics2D.OverlapCircleAll(overlapCirclePosition, overlapCircleRadius);



            collidedWithSomething = colliders.Length > 0;

            /*if (CanAttackPlayer() && Time.time > lastAttackTime + attackCooldown)
                yield return StartCoroutine(ExecuteAttack());*/


            // ≈сли столкнулс€ с каким-то объектом, мен€ем направление движени€
            if (CanAttackPlayer())
            {
                if (Time.time > lastAttackTime + attackCooldown)
                    yield return StartCoroutine(ExecuteAttack());
                else
                    State = States.idle;
            }
            else
            {
                Move(collidedWithSomething);
            }
            


            //transform.position = Vector3.MoveTowards(transform.position, transform.position + dir, speed * Time.deltaTime);

           
        }

    }

    private void Move(bool collidedWithSomething)
    {
        if (getDamage)
            return;

        State = States.run;

        if (collidedWithSomething)
            dir *= -1f;

        transform.position = Vector3.MoveTowards(transform.position, transform.position + dir, speed* Time.deltaTime);
        sprite.flipX = dir.x < 0.0f;
    }

}