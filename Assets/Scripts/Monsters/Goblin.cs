using System.Collections;
using UnityEngine;

public class Goblin : Enemies
{
    [SerializeField] private LayerMask playerLayer;
    [SerializeField] private float moveSpeed = 5f; // Скорость движения врага
    [SerializeField] private float viewDistance = 10f; // Радиус зрения
    [SerializeField] private float deathPushForce = 13f;
    [SerializeField] private LayerMask walkingEnemy;
    [SerializeField] private LayerMask standingEnemy;


    private Rigidbody2D rb;
    private Vector2 direction;

    private void Start()
    {
        attackRange = 1.5f;
        attackCooldown = 1f;
        lives = 3;
        rb = GetComponent<Rigidbody2D>();

        StartCoroutine(BehaviorPattern());
    }

    public override void GetDamage()
    {
        getDamage = true;
        lives--;
        if (lives <= 0)
        {    
            direction = transform.position - player.position;
            rb.AddForce(direction.normalized * deathPushForce, ForceMode2D.Impulse);
            StartCoroutine(Die());
            return;
        }

        getDamage = false;
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
                
            if (CanSeePlayer())
            {
                if (!CanAttackPlayer())
                    MoveTowardsPlayer();
                else if (CanAttackPlayer() && Time.time > lastAttackTime + attackCooldown)
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
        direction = player.position - transform.position;
        RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, viewDistance, LayerMask.GetMask("Default") | playerLayer | standingEnemy);

        return hit.collider != null && hit.collider.CompareTag("Hero");
    }

    private void MoveTowardsPlayer()
    {
        if (getDamage)
            return;
        
        State = States.run;
        transform.position = Vector3.MoveTowards(transform.position, player.position, moveSpeed * Time.deltaTime);
        sprite.flipX = player.position.x < transform.position.x;
    }
}
