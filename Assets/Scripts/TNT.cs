using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
            var surrondingObj = Physics2D.OverlapCircleAll(this.transform.position, explosionForce);
            Debug.Log("Trigger");
            foreach (var obj in surrondingObj)
            {
                Rigidbody2D rb = obj.GetComponent<Rigidbody2D>();
                if (rb == null) continue;
                Vector2 dir = obj.transform.position - transform.position;
                rb.AddForce(dir * explosionForce);

                if (obj.GetComponent<Brick>())
                {
                    Brick b = obj.GetComponent<Brick>();
                    b.TakeDamage(explosionDmg);
                }
            }

           // audioSource.Play();
            //Instantiate(fx, transform.position, Quaternion.identity);
            Destroy(this.gameObject);
        }
    }
}
