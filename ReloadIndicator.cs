using UnityEngine;

public class ReloadIndicator : MonoBehaviour
{
    public float rotationSpeed = 180f; // 초당 회전 속도
    public float reloadDuration = 2f;  // 장전 시간 (초)

    private bool isReloading = false;
    private float timer = 0f;

    public bool IsReloading => isReloading; // 외부에서 확인용

    void Update()
    {
        if (!isReloading) return;

        // 회전
        transform.Rotate(0f, 0f, -rotationSpeed * Time.deltaTime);

        // 타이머 진행
        timer += Time.deltaTime;

        // 종료 조건
        if (timer >= reloadDuration)
        {
            StopReload();
        }
    }

    public void StartReload()
    {
        isReloading = true;
        timer = 0f;
        gameObject.SetActive(true);
    }

    private void StopReload()
    {
        isReloading = false;
        gameObject.SetActive(false);
    }
}
