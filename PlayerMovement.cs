using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour {
    public float moveSpeed = 5f;
    public float dodgeSpeed = 8f;
    public float dodgeDuration = 0.3f;
    public float dodgeCooldown = 1.0f;

    public float standDelay = 0.3f;     // 슬라이딩 후 일어나는 시간
    public float slideDistance = 3f;    // 총 슬라이딩 거리

    private bool isSliding = false;
    private float slidDistance = 0f;
    private float standDelayTimer = 0f;


    public Rigidbody2D rb;

    Vector2 movement;
    Vector3 mousePos;
    Vector2 lookDir;
    Vector2 dodgeDir;

    bool isDodging = false;
    float dodgeTimer = 0f;
    float dodgeCooldownTimer = 0f; // ✅ 쿨타임 타이머

    public GameObject Gun;

    public SpriteRenderer sr;
    public SpriteRenderer gunSr;

    public Sprite leftIdle, leftMove1, leftMove2;
    public Sprite rightIdle, rightMove1, rightMove2;

    public Sprite dodgeRight;
    public Sprite dodgeLeft;

    private float animTimer;
    private float frameTime = 0.2f;
    private bool toggleFrame = false;

    private bool facingRight = true;

    void Update() {
        // 쿨타임 타이머 감소
        if (dodgeCooldownTimer > 0f)
            dodgeCooldownTimer -= Time.deltaTime;

        // 마우스 시선 방향 계산
        mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        lookDir = (Vector2)(mousePos - transform.position);

        if (lookDir.x >= 0f) {
            facingRight = true;
            gunSr.flipX = false;
            Gun.transform.localPosition = new Vector3(0.5f, 0f, 0f);
        } else {
            facingRight = false;
            gunSr.flipX = true;
            Gun.transform.localPosition = new Vector3(-0.5f, 0f, 0f);
        }

        // 이동 입력
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");

        // 회피 입력 + 쿨타임 검사
        if (Input.GetKeyDown(KeyCode.C) && !isDodging && dodgeCooldownTimer <= 0f) {
            Vector2 inputDir = movement.normalized;

            if (inputDir != Vector2.zero) {
                isDodging = true;
                dodgeTimer = dodgeDuration;
                dodgeCooldownTimer = dodgeCooldown; // ✅ 쿨타임 시작
                dodgeDir = inputDir;

                sr.sprite = inputDir.x >= 0f ? dodgeRight : dodgeLeft;

                gunSr.enabled = false; // ✅ 회피 중 총 숨김
            }
        }

        // 회피 중일 때
        if (isDodging) {
            dodgeTimer -= Time.deltaTime;
            if (dodgeTimer <= 0f) {
                isDodging = false;
                gunSr.enabled = true; // ✅ 회피 끝나면 총 다시 보이게
            }
            return;
        }

        // 총 다시 보이기 (회피 중이 아니면 항상 켬)
        if (!gunSr.enabled) gunSr.enabled = true;

        // 애니메이션 처리
        animTimer -= Time.deltaTime;
        bool isMoving = movement.magnitude > 0.1f;

        if (animTimer <= 0 && isMoving) {
            toggleFrame = !toggleFrame;
            animTimer = frameTime;
        }

        if (isMoving) {
            sr.sprite = facingRight ? (toggleFrame ? rightMove1 : rightMove2)
                                    : (toggleFrame ? leftMove1 : leftMove2);
        } else {
            sr.sprite = facingRight ? rightIdle : leftIdle;
        }
    }

    void FixedUpdate() {
        if (isDodging) {
            rb.MovePosition(rb.position + dodgeDir * dodgeSpeed * Time.fixedDeltaTime);
        } else {
            rb.MovePosition(rb.position + movement * moveSpeed * Time.fixedDeltaTime);
        }
    }
}
