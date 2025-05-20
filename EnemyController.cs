using UnityEngine;
using System.Collections;

public class EnemyController : MonoBehaviour {
    public float moveSpeed = 2f;
    public float attackRange = 1.2f;
    public int health = 3;
    public int attackDamage = 1;
    public float attackCooldown = 1f;
    public float deathDuration = 0.5f;

    public Transform player;
    private Rigidbody2D rb;
    private SpriteRenderer sr;

    // 스프라이트들
    public Sprite idleSprite;
    public Sprite moveSprite1;
    public Sprite moveSprite2;
    public Sprite moveSprite3;
    public Sprite moveSprite4;
    public Sprite attackSprite1;
    public Sprite attackSprite2;
    public Sprite deathSprite;

    // 파티클 프리팹 추가
    public GameObject hitEffectPrefab;

    private Vector2 movement;
    private float lastAttackTime = 0f;
    private float animTimer = 0f;
    private float frameTime = 0.2f;
    private int moveFrameIndex = 0;
    private bool toggleAttackFrame = false;
    private bool isDead = false;
    private Sprite[] moveSprites;

    private void Start() {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();

        if (player == null) {
            GameObject foundPlayer = GameObject.FindWithTag("Player");
            if (foundPlayer != null)
                player = foundPlayer.transform;
        }

        moveSprites = new Sprite[] { moveSprite1, moveSprite2, moveSprite3, moveSprite4 };
    }

    private void Update() {
        if (isDead || player == null) return;

        Vector2 direction = player.position - transform.position;
        float distance = direction.magnitude;
        animTimer -= Time.deltaTime;

        if (distance > attackRange) {
            movement = direction.normalized;

            if (animTimer <= 0f) {
                moveFrameIndex = (moveFrameIndex + 1) % moveSprites.Length;
                animTimer = frameTime;
            }

            sr.sprite = moveSprites[moveFrameIndex];
        } else {
            movement = Vector2.zero;

            if (animTimer <= 0f) {
                toggleAttackFrame = !toggleAttackFrame;
                animTimer = frameTime;
            }

            sr.sprite = toggleAttackFrame ? attackSprite1 : attackSprite2;

            if (Time.time - lastAttackTime >= attackCooldown) {
                lastAttackTime = Time.time;
                PlayerHealth playerHealth = player.GetComponent<PlayerHealth>();
                if (playerHealth != null) {
                    Vector2 hitDir = player.position - transform.position;
                    playerHealth.TakeDamage(attackDamage, hitDir);
                }
            }
        }

        sr.flipX = direction.x < 0;
    }

    private void FixedUpdate() {
        if (isDead) return;
        rb.MovePosition(rb.position + movement * moveSpeed * Time.fixedDeltaTime);
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        if (isDead) return;

        if (collision.CompareTag("Bullet")) {
            Vector2 hitDir = (transform.position - collision.transform.position).normalized;
            TakeDamage(1, hitDir);

            Destroy(collision.gameObject);
        }
    }

    private void TakeDamage(int damage, Vector2 hitDirection) {
        health -= damage;

        // ✅ 피격 이펙트 생성
        if (hitEffectPrefab != null) {
            GameObject effect = Instantiate(hitEffectPrefab, transform.position, Quaternion.identity);
            Destroy(effect, 1f); // 1초 후 자동 제거
        }

        // (선택) 넉백 적용하려면 아래 줄 추가
        // rb.AddForce(hitDirection * 2f, ForceMode2D.Impulse);

        if (health <= 0) {
            Die();
        }
    }

    private void Die() {
        isDead = true;
        movement = Vector2.zero;

        sr.sprite = deathSprite;
        sr.flipX = false;

        rb.simulated = false;
        Collider2D col = GetComponent<Collider2D>();
        if (col != null) col.enabled = false;

        StartCoroutine(DieAndDisappear());
    }

    private IEnumerator DieAndDisappear() {
        yield return new WaitForSeconds(deathDuration);
        Destroy(gameObject);
    }
}
