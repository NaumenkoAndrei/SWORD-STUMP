using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Hero : Entity
{
    [Header("Audio")]
    [SerializeField] private AudioSource jumpSound;
    [SerializeField] private AudioSource damageSound;
    [SerializeField] private AudioSource attackMissSound;
    [SerializeField] private AudioSource attackMobSound;
    [SerializeField] private AudioSource deadSound;

    [Header("Movement")]
    [SerializeField] private float jumpForce = 0.6f;
    [SerializeField] private float speedMin = 3f;
    [SerializeField] private float speedMax = 6f;
    private float speedCurrent;

    [Header("Health")]
    [SerializeField] private Image[] hearts;
    [SerializeField] private Sprite aliveHeart;
    [SerializeField] private Sprite deadHeart;
    [SerializeField] private float health;

    [Header("Attack")]
    [SerializeField] private float attackRange;
    [SerializeField] private Transform attackPositionRight;
    [SerializeField] private Transform attackPositionLeft;
    private Transform currentAttackPosition;
    private bool isAttacking = false;

    [Header("Controls")]
    public Joystick joystik;

    private bool isGrounded = false;
    private bool isRecharged = true;

    private Rigidbody2D rb;
    private Collider2D colider;
    [SerializeField] private LayerMask LayerMaskHero;

    public static Hero Instance { get; set; }

    private void Start()
    {
        Initialize();
    }

    private void Initialize()
    {
        Instance = this;
        lives = 500;
        health = lives;
        currentAttackPosition = attackPositionRight;

        rb = GetComponent<Rigidbody2D>();
        animate = GetComponent<Animator>();
        sprite = GetComponentInChildren<SpriteRenderer>();
        colider = GetComponentInChildren<Collider2D>();
    }

    private void FixedUpdate()
    {
        UpdateHearts();
        CheckGround();
    }

    private void UpdateHearts()
    {
        for (int i = 0; i < hearts.Length; i++)
        {
            hearts[i].sprite = i < health ? aliveHeart : deadHeart;
            hearts[i].enabled = i < lives;
        }
    }

    private void Update()
    {
        if (getDamage || isAttacking)
            return;

        if (isGrounded)
            State = States.idle;

        if (joystik.Horizontal != 0)
        {
            speedCurrent = Math.Abs(joystik.Horizontal) >= 0.5f ? speedMax : speedMin;
            Run();
        }

        if (isGrounded && joystik.Vertical > 0.5f)
            Jump();
    }

    public override void GetDamage()
    {
        if (getDamage) return;

        getDamage = true;

        health--;
        if (health > 0)
            damageSound.Play();

        StartCoroutine(EnemyOnAttack(colider));
        if (health == 0)
        {
            foreach (var heart in hearts)
                heart.sprite = deadHeart;
            deadSound.Play();
            StartCoroutine(Die(1.4f));
            return;
        }
        getDamage = false;
    }

    private void Run()
    {
        if (isGrounded)
            State = States.run;

        Vector3 dir = transform.right * joystik.Horizontal;
        transform.position = Vector3.MoveTowards(transform.position, transform.position + dir, speedCurrent * Time.deltaTime);
        sprite.flipX = dir.x < 0.0f;

        currentAttackPosition = dir.x < 0.0f ? attackPositionLeft : attackPositionRight;
    }

    private void Jump()
    {
        jumpSound.Play();
        rb.velocity = Vector2.up * jumpForce;
    }

    public void Attack()
    {
        if (isGrounded && isRecharged && !getDamage)
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
        Collider2D[] colliders = Physics2D.OverlapCircleAll(currentAttackPosition.position, attackRange, ~LayerMaskHero);

        if (colliders.Length == 0)
            attackMissSound.Play();
        else
            attackMobSound.Play();

        foreach (Collider2D collider in colliders)
        {
            collider.GetComponent<Entity>().GetDamage();
            StartCoroutine(EnemyOnAttack(collider));
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(currentAttackPosition.position, attackRange);
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