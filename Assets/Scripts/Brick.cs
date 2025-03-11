using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Brick : MonoBehaviour
{
    [SerializeField] private List<DamageModifier> damageModifiers;

    void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.GetComponent<Rigidbody2D>() == null) return;

        float damage = col.gameObject.GetComponent<Rigidbody2D>().velocity.magnitude * 10;

        TakeDamage(damage, col.gameObject.GetComponent<Multitag>());
    }

    public float Health = 70f;

    public void TakeDamage(float damage, Multitag damagerTags = null)
    {
        if(damagerTags != null)
        {
            foreach(DamageModifier modifier in damageModifiers)
            {
                if(damagerTags.Tags.Contains(modifier.condition))
                {
                    damage *= modifier.effect;
                }
            }
        }
        //don't play audio for small damages
        if (damage >= 10)
            GetComponent<AudioSource>().Play();
        //decrease health according to magnitude of the object that hit us
        Health -= damage;
        //if health is 0, destroy the block
        if (Health <= 0) Destroy(this.gameObject);
    }

    //wood sound found in 
    //https://www.freesound.org/people/Srehpog/sounds/31623/
}
