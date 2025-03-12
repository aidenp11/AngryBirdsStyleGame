﻿using UnityEngine;
using System.Collections;

public class Brick : MonoBehaviour
{


    protected virtual void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.GetComponent<Rigidbody2D>() == null) return;

        float damage = col.gameObject.GetComponent<Rigidbody2D>().velocity.magnitude * 10;
        //don't play audio for small damages
        if (damage >= 10)
            GetComponent<AudioSource>().Play();
        //decrease health according to magnitude of the object that hit us

        if (col.gameObject.tag == extraBirdDmg) damage *= 2f;
        ApplyDmg(damage);
        //if health is 0, destroy the block
        if (Health <= 0) Destroy(this.gameObject);
    }

    public void ApplyDmg(float dmg)
    {
        Health -= dmg;
    }

    public string extraBirdDmg;
    public float Health = 70f;


    //wood sound found in 
    //https://www.freesound.org/people/Srehpog/sounds/31623/
}
