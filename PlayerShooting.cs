using System.Collections;
using UnityEngine;

public class PlayerShooting : MonoBehaviour {
    public GameObject bulletPrefab;
    public Transform firePoint;
    public float bulletSpeed = 100f;

    public Reload reload;

    public Transform gunTransform;

    public ReloadIndicator reloadIndicatorUI;
    public Transform reloadIndicatorFollowTarget;  // 🔥 인디케이터가 따라갈 대상 (머리 위 위치)

    void Start() {
        if (firePoint == null) {
            firePoint = GameObject.Find("FirePoint").transform;
        }

        reload = GetComponent<Reload>();

        if (reloadIndicatorUI != null) {
            reloadIndicatorUI.gameObject.SetActive(false);
        }
    }

    void Update() {
        // 장전 중엔 입력 무시
        if (reload.isReloading) return;

        if (Input.GetMouseButtonDown(0) && reload.CanShoot()) {
            Shoot();
            reload.ConsumeAmmo();
        }

        // ✅ 조건: 재장전 가능할 때만
        if (Input.GetKeyDown(KeyCode.R) && !reload.IsAmmoFull()) {
            StartCoroutine(StartReloadVisual());
        }

        // ✅ 인디케이터가 따라다니게 위치 갱신
        if (reloadIndicatorUI != null && reloadIndicatorUI.gameObject.activeSelf && reloadIndicatorFollowTarget != null) {
            reloadIndicatorUI.transform.position = reloadIndicatorFollowTarget.position;
        }
    }

    void Shoot() {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 direction = (mousePos - firePoint.position).normalized;

        Vector2[] directions = {
            Vector2.up, new Vector2(1, 1).normalized, Vector2.right, new Vector2(1, -1).normalized,
            Vector2.down, new Vector2(-1, -1).normalized, Vector2.left, new Vector2(-1, 1).normalized
        };

        Vector2 bestDir = directions[0];
        float maxDot = Vector2.Dot(direction, bestDir);
        foreach (var d in directions) {
            float dot = Vector2.Dot(direction, d);
            if (dot > maxDot) {
                maxDot = dot;
                bestDir = d;
            }
        }

        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);
        bullet.transform.right = bestDir;
        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
        rb.velocity = bestDir * bulletSpeed;

        bullet.GetComponent<SpriteRenderer>().flipY = bestDir.x < 0;
    }

    IEnumerator StartReloadVisual() {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        float angle = mousePos.x >= transform.position.x ? 45f : -45f;

        if (gunTransform != null) {
            gunTransform.localRotation = Quaternion.Euler(0, 0, angle);
        }

        // ✅ 인디케이터 켜기
        if (reloadIndicatorUI != null) {
            reloadIndicatorUI.StartReload();
            if (reloadIndicatorFollowTarget != null)
                reloadIndicatorUI.transform.position = reloadIndicatorFollowTarget.position;
        }

        yield return StartCoroutine(reload.StartReload());

        if (gunTransform != null) {
            gunTransform.localRotation = Quaternion.identity;
        }
    }
}
