using UnityEngine;
using System.Collections;

public class Pig : MonoBehaviour
{

    [SerializeField] float Health;
    [SerializeField] Sprite SpriteShownWhenHurt;
    [SerializeField] float ChangeSpriteHealth;

    private float pastVelocity;
    private Rigidbody2D rb;
    private float vtimer = 0.05f;
    // Use this for initialization
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        pastVelocity = rb.velocity.y;
        //ChangeSpriteHealth = Health / 2;
    }

	private void Update()
	{
        vtimer -= Time.deltaTime;
		if (vtimer <= 0)
        {
            pastVelocity = rb.velocity.y;
            vtimer = 0.05f;
        }
	}

	void OnCollisionEnter2D(Collision2D col)
    {
        //if (col.gameObject.GetComponent<Rigidbody2D>() == null) return;

        //if we are hit by a bird
        if (col.gameObject.tag == "Bird")
        {          

            float damage = Mathf.Abs(col.gameObject.GetComponent<Rigidbody2D>().velocity.magnitude) * 15f;


			Health -= damage;
			//don't play sound for small damage
			if (damage >= 10)
				GetComponent<AudioSource>().Play();

			if (Health < ChangeSpriteHealth)
			{//change the shown sprite
				GetComponent<SpriteRenderer>().sprite = SpriteShownWhenHurt;
			}
			if (Health <= 0) Destroy(this.gameObject);
		}
        else //we're hit by something else
        {
            float damage;
			//calculate the damage via the hit object velocity
			if (col.gameObject.GetComponent<Rigidbody2D>() == null)
            {
                damage = Mathf.Abs(pastVelocity) * 15f;
            }
			else
			{
				damage = Mathf.Abs(col.gameObject.GetComponent<Rigidbody2D>().velocity.magnitude) * 15f;
			}

			
            Health -= damage;
            //don't play sound for small damage
            if (damage >= 10)
                GetComponent<AudioSource>().Play();

            if (Health < ChangeSpriteHealth)
            {//change the shown sprite
                GetComponent<SpriteRenderer>().sprite = SpriteShownWhenHurt;
            }
            if (Health <= 0) Destroy(this.gameObject);
        }
    }

	public void TakeDamage(float damage)
    {
        Health -= damage;
        //don't play sound for small damage
        if (damage >= 10)
            GetComponent<AudioSource>().Play();

        if (Health < ChangeSpriteHealth)
        {//change the shown sprite
            GetComponent<SpriteRenderer>().sprite = SpriteShownWhenHurt;
        }
        if (Health <= 0) Destroy(this.gameObject);
    }

    //sound found in
    //https://www.freesound.org/people/yottasounds/sounds/176731/
}
