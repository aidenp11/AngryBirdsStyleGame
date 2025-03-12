using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombBird : Bird
{
    [SerializeField] private float explosionRadius = 2.5f;
    [SerializeField] private float explosionKnockback = 100f;
    [SerializeField] private GameObject explosionFX;
    [SerializeField] private AudioSource explosionSX;
    bool abilityAvailable = false;
    float explosionDelay = 0;
    bool collided = false;
    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0) && abilityAvailable)
        {
            abilityAvailable = false;
            Explode();
        }
        if(collided)
        {
            if(explosionDelay > 0)
            {
                explosionDelay -= Time.deltaTime;
            }
            else
            {
                collided = false;
                Explode();
            }
        }
    }

    public override void OnThrow()
    {
        base.OnThrow();
        abilityAvailable = true;
        collided = false;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(abilityAvailable && !collided)
        {
            collided = true;
            abilityAvailable = false;
            explosionDelay = 2.0f;
        }
    }

    private void Explode()
    {
        if (explosionFX != null)
        {
            Instantiate(explosionFX, transform.position, Quaternion.identity);
        }
        if(explosionSX != null)
        {
            explosionSX.Play();
        }
        foreach (Collider2D col in Physics2D.OverlapCircleAll(transform.position, explosionRadius))
        {
            if(col.gameObject.TryGetComponent<Brick>(out Brick brick))
            {
                col.attachedRigidbody.AddForce((Vector2)(brick.transform.position - transform.position) * explosionKnockback / Mathf.Max(0.1f, Vector2.Distance(transform.position, brick.transform.position) / explosionRadius));
                brick.TakeDamage(35 / Mathf.Max(0.01f, Vector2.Distance(transform.position, brick.transform.position) / explosionRadius));
            }
            else if(col.gameObject.TryGetComponent<Pig>(out Pig pig))
            {
                col.attachedRigidbody.AddForce((Vector2)(pig.transform.position - transform.position) * explosionKnockback * 0.5f / Mathf.Max(0.1f, Vector2.Distance(transform.position, pig.transform.position) / explosionRadius));
                pig.TakeDamage(350 / Mathf.Max(0.01f, Vector2.Distance(transform.position, pig.transform.position) / explosionRadius));
            }
            else if(col.gameObject.TryGetComponent<Bird>(out Bird bird) && bird != this)
            {
                col.attachedRigidbody.AddForce((Vector2)(bird.transform.position - transform.position) * explosionKnockback * 0.5f / Mathf.Max(0.1f, Vector2.Distance(transform.position, bird.transform.position) / explosionRadius   ));
            }
        }
    }
}
