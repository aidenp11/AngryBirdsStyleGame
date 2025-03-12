using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlueBird : Bird
{
    [SerializeField] GameObject childPrefab;
    bool abilityAvailable = false;

    public override void OnThrow()
    {
        base.OnThrow();
        abilityAvailable = true;
        GetComponent<Rigidbody2D>().mass = 0.5f;
        GetComponent<CircleCollider2D>().radius = 0.15f;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        abilityAvailable = false;
    }

    private void Update()
    {
        if (abilityAvailable && Input.GetMouseButtonDown(0))
        {
            abilityAvailable = false;
            for(int i = 0; i < 2; i++)
            {
                GameObject go = Instantiate(childPrefab);
                go.transform.position = transform.position;
                go.GetComponent<Bird>().OnThrow();
                Rigidbody2D body = go.GetComponent<Rigidbody2D>();
                body.velocity = Quaternion.Euler(0, 0, 15 * (i == 1 ? 1 : -1)) * GetComponent<Rigidbody2D>().velocity;
                GameManager.Instance.cameraFollow.BirdToFollow.Add(go.transform);
                GameManager.Instance.AddBird(go);
            }
        }
    }
}
