using TMPro;
using UnityEngine;

public class Goblin : Enemies
{
    [SerializeField] private Transform target; 
    [SerializeField] private float moveSpeed = 5f; 
    [SerializeField] private float attackRange = 2f; 
    [SerializeField] private float viewDistance = 10f;
    private SpriteRenderer sprite;
    private Animator anim;


    private Rigidbody2D rb;
    private bool isAttacking = false;


    Vector3 lastKnownPlayerPosition;

    protected States State
    {
        get { return (States)anim.GetInteger("state"); }
        set { anim.SetInteger("state", (int)value); }
    }

    void Start()
    {
        lives = 1;
    }

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        sprite = GetComponentInChildren<SpriteRenderer>();
    }

    public override void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject == Hero.Instance.gameObject)
        {
            State = States.attack;
            target = collision.gameObject.transform;
            isAttacking = true;
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
            {
                State = States.idle;
                return;
            }
                

        RaycastHit2D playerHit = Physics2D.Raycast(transform.position, targetDirection.normalized, Mathf.Infinity);
        if (playerHit.collider != null && playerHit.collider.CompareTag(tag: "Player"))
        {
            lastKnownPlayerPosition = playerHit.collider.transform.position;

            float distanceToTarget = Vector3.Distance(transform.position, target.position);

            if (distanceToTarget <= attackRange && !isAttacking)
                Attack();
            else if (distanceToTarget <= viewDistance && !isAttacking)
                MoveTowardsTarget(lastKnownPlayerPosition);
            else if (!isAttacking)
                State = States.idle;
            else
                isAttacking = false;
                
        }
        else
            MoveTowardsTarget(lastKnownPlayerPosition);
            
    }

    void MoveTowardsTarget(Vector3 targetPosition)
    {
        State = States.run;
        Vector3 direction = (target.position - transform.position).normalized;

        rb.velocity = new Vector2(direction.x * moveSpeed, rb.velocity.y);
        sprite.flipX = direction.x < 0.0f;
    }

    void Attack()
    {


    }
}


