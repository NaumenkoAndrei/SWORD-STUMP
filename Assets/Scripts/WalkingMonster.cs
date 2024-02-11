using UnityEngine;

public class WalkingMonster : Enemies
{
    [SerializeField] float speed = 2.5f;
    private Vector3 dir;
    private SpriteRenderer sprite;

    private void Start()
    {
        dir = transform.right;
        lives = 6;
    }

    private void Awake()
    {
        sprite = GetComponentInChildren<SpriteRenderer>();
    }

    private void Update()
    {
        Move();
    }

    private void Move()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position + transform.up * 0.1f + transform.right * dir.x * 0.7f, -0.1f);

        if (colliders.Length > 0)
            if (colliders[0].gameObject != Hero.Instance.gameObject)
                dir *= -1f;


        transform.position = Vector3.MoveTowards(transform.position, transform.position + dir, speed * Time.deltaTime);

        sprite.flipX = dir.x > 0.0f;
    }
}