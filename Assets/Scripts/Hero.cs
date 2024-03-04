using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Hero : Entity
{
    [Header("Audio")]
    [SerializeField] private AudioSource _jumpSound;
    [SerializeField] private AudioSource _damageSound;
    [SerializeField] private AudioSource _attackMissSound;
    [SerializeField] private AudioSource _attackMobSound;
    [SerializeField] private AudioSource _deadSound;

    [Header("Movement")]
    [SerializeField] private float _jumpForce = 0.6f;
    [SerializeField] private float _speedMin = 3f;
    [SerializeField] private float _speedMax = 6f;
    private float speedCurrent;

    [Header("Health")]
    [SerializeField] private Image[] _hearts;
    [SerializeField] private Sprite _aliveHeart;
    [SerializeField] private Sprite _deadHeart;
    [SerializeField] private float _health;

    [Header("Attack")]
    [SerializeField] private float _attackRange;
    [SerializeField] private Transform _attackPositionRight;
    [SerializeField] private Transform _attackPositionLeft;
    private Transform _currentAttackPosition;
    private bool _isAttacking = false;

    [Header("Controls")]
    [SerializeField] private Joystick _joystik;

    private bool _isGrounded = false;
    private bool _isRecharged = true;

    private Rigidbody2D _rigidbody;
    private Collider2D _colider;
    private Collider2D[] _colliders;
    [SerializeField] private LayerMask _layerMaskHero;
    [SerializeField] private GameObject _losePanel;

    public static Hero Instance { get; set; }

    private void Start()
    {
        Initialize();
    }

    private void Initialize()
    {
        _losePanel.SetActive(false);
        Instance = this;
        _lives = 5;
        _health = _lives;
        _currentAttackPosition = _attackPositionRight;

        _rigidbody = GetComponent<Rigidbody2D>();
        _animate = GetComponent<Animator>();
        _sprite = GetComponentInChildren<SpriteRenderer>();
        _colider = GetComponentInChildren<Collider2D>();
    }

    private void FixedUpdate()
    {
        UpdateHearts();
    }

    private void UpdateHearts()
    {
        for (int i = 0; i < _hearts.Length; i++)
        {
            _hearts[i].sprite = i < _health ? _aliveHeart : _deadHeart;
            _hearts[i].enabled = i < _lives;
        }
    }

    private void Update()
    {
        if (_getDamage || _isAttacking)
            return;

        CheckGround();

        if (_isGrounded)
            State = States.idle;

        if (_joystik.Horizontal != 0)
        {
            speedCurrent = Math.Abs(_joystik.Horizontal) >= 0.5f ? _speedMax : _speedMin;
            Run();
        }

        if (_isGrounded && _joystik.Vertical > 0.5f)
            Jump();
    }

    public void GetDamage()
    {
        if (_getDamage) return;

        _getDamage = true;

        _health--;
        if (_health > 0)
            _damageSound.Play();

        StartCoroutine(EnemyOnAttack(_colider));
        if (_health == 0)
        {
            foreach (var heart in _hearts)
                heart.sprite = _deadHeart;
            _deadSound.Play();
            StartCoroutine(Die());
            return;
        }
        _getDamage = false;
    }

    private IEnumerator Die(float timeUntilDeletion = 1.4f)
    {
        State = States.dead;
        yield return new WaitForSeconds(timeUntilDeletion);
        Destroy(gameObject);
        LosePanel();
    }

    public void LosePanel()
    {
        _losePanel.SetActive(true);
        Time.timeScale = 0;
    }

    private void Run()
    {
        if (_isGrounded)
            State = States.run;

        Vector3 dir = transform.right * _joystik.Horizontal;
        transform.position = Vector3.MoveTowards(transform.position, transform.position + dir, speedCurrent * Time.deltaTime);
        _sprite.flipX = dir.x < 0.0f;

        _currentAttackPosition = dir.x < 0.0f ? _attackPositionLeft : _attackPositionRight;
    }

    private void Jump()
    {
        if (_isGrounded)
        {
            _jumpSound.Play();
            _rigidbody.velocity = Vector2.up * _jumpForce;
        }
    }

    public void Attack()
    {
        if (_isGrounded && _isRecharged && !_getDamage)
        {
            State = States.attack;
            _isAttacking = true;
            _isRecharged = false;

            StartCoroutine(AttackAnimation());
            StartCoroutine(AttackCoolDown());
        }
    }

    private void OnAttack()
    {
        _colliders = Physics2D.OverlapCircleAll(_currentAttackPosition.position, _attackRange, ~_layerMaskHero);

        if (_colliders.Length == 0)
            _attackMissSound.Play();
        else
            _attackMobSound.Play();

        foreach (Collider2D collider in _colliders)
        {
            collider.GetComponent<Enemies>().GetDamage();
            StartCoroutine(EnemyOnAttack(collider));
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(_currentAttackPosition.position, _attackRange);
    }

    private void CheckGround()
    {
        Collider2D[] collider = Physics2D.OverlapCircleAll(transform.position, 0.1f);
        _isGrounded = collider.Length > 1;

        if (!_isGrounded)
            State = States.jump;
    }

    private IEnumerator AttackAnimation()
    {
        yield return new WaitForSeconds(0.4f);
        _isAttacking = false;
    }

    private IEnumerator AttackCoolDown()
    {
        yield return new WaitForSeconds(0.5f);
        _isRecharged = true;
    }

    private IEnumerator EnemyOnAttack(Collider2D enemy)
    {
        SpriteRenderer enemyColor = enemy.GetComponentInChildren<SpriteRenderer>();
        enemyColor.color = new Color(1f, 0.5f, 0.5f);
        yield return new WaitForSeconds(0.2f);
        enemyColor.color = new Color(1, 1, 1);
    }
}