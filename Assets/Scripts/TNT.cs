using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Purchasing;

public class TNT : MonoBehaviour
{
    [SerializeField] private float triggerForce = 0.5f;
    [SerializeField] private float explosionRadius = 5;
    [SerializeField] private float explosionForce = 500;
    [SerializeField] private float explosionDmg = 50f;
    [SerializeField] private GameObject fx;
    [SerializeField] AudioSource audioSource;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.relativeVelocity.magnitude >= triggerForce)
        {
            var surrondingObj = Physics2D.OverlapCircleAll(this.transform.position, explosionRadius);
            Debug.Log("Trigger");
            foreach (var obj in surrondingObj)
            {
                Rigidbody2D rb = obj.GetComponent<Rigidbody2D>();
                if (rb == null) continue;
                Vector2 dir = obj.transform.position - transform.position;
                float relativeEffect = 1 / Mathf.Max(0.1f, Vector2.Distance(transform.position, rb.transform.position) / explosionRadius);
                rb.AddForce(explosionForce * relativeEffect * dir);

                if (obj.TryGetComponent(out Brick b))
                {
                    b.TakeDamage(explosionDmg * relativeEffect);
                }
                else if(obj.TryGetComponent(out Pig p))
                {
                    p.TakeDamage(explosionDmg * relativeEffect);
                }
            }

            audioSource.Play();
            Instantiate(fx, transform.position, Quaternion.identity);
            Destroy(this.gameObject);
        }
    }
}
