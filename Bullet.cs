using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 10f;
    public float lifeTime = 2f;

    void Start()
    {
        Destroy(gameObject, lifeTime); 
    }

    void Update()
    {
        transform.Translate(Vector2.right * speed * Time.deltaTime); 
    }
}
