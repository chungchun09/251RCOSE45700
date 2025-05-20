using System.Collections;
using UnityEngine;

public class Reload : MonoBehaviour
{
    public int maxAmmo = 6;              // 탄약 최대 수
    public int currentAmmo;             // 현재 탄약 수
    public float reloadTime = 1.5f;     // 장전 시간
    public bool isReloading = false;    // 장전 중 여부

    void Start()
    {
        currentAmmo = maxAmmo;
    }

    public bool CanShoot()
    {
        return currentAmmo > 0 && !isReloading;
    }

    public void ConsumeAmmo()
    {
        currentAmmo--;
    }

    public IEnumerator StartReload()
    {
        if (isReloading) yield break;

        isReloading = true;
        Debug.Log("🔄 장전 중...");

        yield return new WaitForSeconds(reloadTime);

        currentAmmo = maxAmmo;
        isReloading = false;

        Debug.Log("✅ 장전 완료!");
    }

    public bool IsAmmoFull(){
        return currentAmmo == maxAmmo;
    }

}
