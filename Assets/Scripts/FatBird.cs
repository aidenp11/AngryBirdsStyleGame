using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FatBird : Bird
{
    private void OnCollisionEnter2D(Collision2D collision)
    {
        Rigidbody2D body = GetComponent<Rigidbody2D>();
        body.mass = Mathf.Max(1, body.mass * 0.75f);
    }

    public override void OnThrow()
    {
        base.OnThrow();
        GetComponent<Rigidbody2D>().mass = 5;
        GetComponent<CircleCollider2D>().radius *= 1.75f;
    }
}
