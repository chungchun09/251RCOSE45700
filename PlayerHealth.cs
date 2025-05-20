using UnityEngine;
using System.Collections;

public class PlayerHealth : MonoBehaviour {
    public int maxHealth = 5;
    private int currentHealth;

    public SpriteRenderer sr;
    public Sprite normalSprite;
    public Sprite hurtSprite;
    public Sprite deathSprite;

    private bool isDead = false;
    private float hurtDuration = 0.2f;

    void Start() {
        currentHealth = maxHealth;
        sr = GetComponent<SpriteRenderer>();
    }

    public void TakeDamage(int damage, Vector2 hitDirection) {
        if (isDead) return;

        currentHealth -= damage;
        Debug.Log("Player hit! Current HP: " + currentHealth);

        if (currentHealth <= 0) {
            Die();
        } else {
            StartCoroutine(ShowHurt());
        }
    }

    IEnumerator ShowHurt() {
        sr.sprite = hurtSprite;
        yield return new WaitForSeconds(hurtDuration);
        sr.sprite = normalSprite;
    }

    void Die() {
        isDead = true;
        sr.sprite = deathSprite;
        GetComponent<PlayerMovement>().enabled = false;
        GetComponent<PlayerShooting>().enabled = false;
    }
}
