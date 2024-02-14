using UnityEngine;

public class Mushroop : Enemies
{
    [SerializeField] private Transform target; 
    [SerializeField] private float moveSpeed = 3f; 
    [SerializeField] private float attackRange = 1f; 
    [SerializeField] private float viewDistance = 15f;
    private SpriteRenderer sprite;

    private Rigidbody2D rb;
    private bool isAttacking = false;


    Vector3 lastKnownPlayerPosition;

    void Start()
    {
        lives = 3;
    }

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        sprite = GetComponentInChildren<SpriteRenderer>();
    }

    public override void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject == Hero.Instance.gameObject)
        {
            target = collision.gameObject.transform;
            isDamaging = true;
            ApplyDamage();
        }
        else
            rb.velocity = Vector2.zero;
    }


    private void FixedUpdate()
    {
        CheckPlayerPosition();
    }
    
    void CheckPlayerPosition()
    {
        Vector3 targetDirection = target.position - transform.position;

        RaycastHit2D[] hits = Physics2D.RaycastAll(transform.position, targetDirection.normalized, targetDirection.magnitude);
        foreach (RaycastHit2D hit in hits)
            if (!hit.collider.CompareTag("Player"))
                return;

        RaycastHit2D playerHit = Physics2D.Raycast(transform.position, targetDirection.normalized, Mathf.Infinity);
        if (playerHit.collider != null && playerHit.collider.CompareTag(tag: "Player"))
        {
            lastKnownPlayerPosition = playerHit.collider.transform.position;

            float distanceToTarget = Vector3.Distance(transform.position, target.position);

            if (distanceToTarget <= attackRange && !isAttacking)
                Attack();
            else if (distanceToTarget <= viewDistance)
                MoveTowardsTarget(target.position);
        }
        else
            MoveTowardsTarget(lastKnownPlayerPosition);
    }

    void MoveTowardsTarget(Vector3 targetPosition)
    {
        Vector3 direction = (target.position - transform.position).normalized;

        rb.velocity = new Vector2(direction.x * moveSpeed, rb.velocity.y);
        sprite.flipX = direction.x < 0.0f;
    }

    void Attack()
    {
        isAttacking = true;
        // Здесь можно добавить задержку между атаками
        // Например, через Coroutine или использование таймера
    }
}



