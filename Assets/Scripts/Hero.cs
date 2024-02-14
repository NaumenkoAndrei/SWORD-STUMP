using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Hero : Entity
{
    [SerializeField] private AudioSource jumpSound;
    [SerializeField] private AudioSource damageSound;
    [SerializeField] private AudioSource attackMissSound;
    [SerializeField] private AudioSource attackMobSound;
    [SerializeField] private AudioSource deadSound;

    [SerializeField] private float speed = 3f;
    [SerializeField] private float jumpForce = 0.6f;
    [SerializeField] private float health;
    private bool isGrounded = false;

    [SerializeField] private Image[] hearts;

    [SerializeField] private Sprite aliveHeart;
    [SerializeField] private Sprite deadHeart;

    private bool isAttacking = false;
    private bool isRecharged = true;

    [SerializeField] private Transform attackPositionRight;
    [SerializeField] private Transform attackPositionLeft;
    private Transform currenAttackPosition;
    [SerializeField] private float attackRange;
    public LayerMask enemy;
    public Joystick joystik;
    
    private Rigidbody2D rb;
    private Animator anim;
    private SpriteRenderer sprite;
    private Collider2D colider;

    public static Hero Instance { get; set; }

    private States State
    {
        get { return (States)anim.GetInteger("state"); }
        set { anim.SetInteger("state", (int)value); }
    }

    private void Start()
    {
        lives = 5;
        health = lives;
        currenAttackPosition = attackPositionRight;
    }

    private void Awake()
    {
        Instance = this;
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        sprite = GetComponentInChildren<SpriteRenderer>();
        colider = GetComponentInChildren<Collider2D>();
        isRecharged = true;
    }

    private void FixedUpdate()
    {
        CheckGround();
    }

    private void Update()
    {
        if (health > lives)
            health = lives;

        if (Math.Abs(joystik.Horizontal) >= 0.5f)
            speed = 5f;
        else
            speed = 3f;

        if (health > 0 && isGrounded && !isAttacking)
            State = States.idle;

        if (health > 0 && !isAttacking && joystik.Horizontal != 0)
            Run();

        if (health > 0 && !isAttacking && isGrounded && joystik.Vertical > 0.5f)
            Jump();
        //if (health > 0 && Input.GetButtonDown("Fire1"))
            //Attack();

        if (health <= 0)
            State = States.dead;

        for (int i = 0; i < hearts.Length; i++)
        {
            if (i < health)
                hearts[i].sprite = aliveHeart;
            else 
                hearts[i].sprite = deadHeart;

            if (i < lives)
                hearts[i].enabled = true;
            else
                hearts[i].enabled = false;
        }
    }

    public override void GetDamage()
    {
        health--;
        if (health > 0)
            damageSound.Play();
        StartCoroutine(EnemyOnAttack(colider));
        if (health == 0)
        {
            deadSound.Play();
            foreach (var heart in hearts)
                heart.sprite = deadHeart;
        }
    }

    private void Run()
    {
        if (isGrounded)
            State = States.run;

        Vector3 dir = transform.right * joystik.Horizontal;
        transform.position = Vector3.MoveTowards(transform.position, transform.position + dir, speed * Time.deltaTime);
        sprite.flipX = dir.x < 0.0f;

        currenAttackPosition = dir.x < 0.0f ? attackPositionLeft : attackPositionRight;
    }


    private void Jump()
    {
        jumpSound.Play();
        rb.velocity = Vector2.up * jumpForce;
    }

    public void Attack()
    {
        if (isGrounded && isRecharged)
        {
            State = States.attack;
            isAttacking = true;
            isRecharged = false;

            StartCoroutine(AttackAnimation());
            StartCoroutine(AttackCoolDown());
        }
    }

    private void OnAttack()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(currenAttackPosition.position, attackRange, enemy);
        
        if (colliders.Length == 0)
            attackMissSound.Play();
        else
            attackMobSound.Play();

        for (int i = 0; i < colliders.Length; i++)
        {
            colliders[i].GetComponent<Entity>().GetDamage();
            StartCoroutine(EnemyOnAttack(colliders[i]));
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(currenAttackPosition.position, attackRange);
    }


    private void CheckGround()
    {
        Collider2D[] collider = Physics2D.OverlapCircleAll(transform.position, 0.3f);
        isGrounded = collider.Length > 1;

        if (!isGrounded) 
            State = States.jump;
    }


    private IEnumerator AttackAnimation()
    {
        yield return new WaitForSeconds(0.4f);
        isAttacking = false;
    }

    private IEnumerator AttackCoolDown()
    {
        yield return new WaitForSeconds(0.5f);
        isRecharged = true;
    }

    private IEnumerator EnemyOnAttack(Collider2D enemy)
    {
        SpriteRenderer enemyColor = enemy.GetComponentInChildren<SpriteRenderer>();
        enemyColor.color = new Color(1f, 0.5f, 0.5f);
        yield return new WaitForSeconds(0.2f);
        enemyColor.color = new Color(1, 1, 1);
    }
}

public enum States
{
    idle,
    attack,
    damage,
    dead,
    run,
    jump
}

