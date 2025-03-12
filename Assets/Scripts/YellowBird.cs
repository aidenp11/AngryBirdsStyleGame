using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class YellowBird : Bird
{
    private bool abilityAvailable = false;
    private void Update()
    {
        //Click attempts to use ability, with the ability for this bird being to increase its speed
        if(abilityAvailable && Input.GetMouseButtonDown(0))
        {
            //Disable ability after it has been used
            abilityAvailable = false;
            GetComponent<Rigidbody2D>().velocity *= 1.75f;
        }
    }

    public override void OnThrow()
    {
        base.OnThrow();
        //Only enable ability after the bird has been thrown to avoid it triggering while waiting in line
        abilityAvailable = true;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        //Disable ability if you hit something
        abilityAvailable = false;
    }
}
