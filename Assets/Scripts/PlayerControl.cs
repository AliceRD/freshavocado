using UnityEngine;
using System.Collections;

public class PlayerControl : MonoBehaviour
{
	[HideInInspector]
	public bool facingRight = true;			// For determining which way the player is currently facing.
	[HideInInspector]
	public bool jump = false;				// Condition for whether the player should jump. 

	public float springyForce = 1.0f;

	public GameObject bullet;
	public Transform muzzle;

	public float moveForce = 65f;			// Amount of force added to move the player left and right.
	public float maxSpeed = 2f;				// The fastest the player can travel in the x axis.
	public AudioClip[] jumpClips;			// Array of clips for when the player jumps.
	public float jumpForce = 100f;			// Amount of force added when the player jumps.
	// public AudioClip[] taunts;				// Array of clips for when the player taunts.
	// public float tauntProbability = 50f;	// Chance of a taunt happening.
	// public float tauntDelay = 1f;			// Delay for when the taunt should happen.

	public GameObject gun;
	// private int tauntIndex;					// The index of the taunts array indicating the most recent taunt.
	// private Transform groundCheck;			// A position marking where to check if the player is grounded. ADDED BACK IN
 	private bool grounded = false;			// Whether or not the player is grounded. FALSE TO TRUE - AND BACK AGAIN
	private Animator anim;					// Reference to the player's animator component.

	public float recoil = 100.0f;

	private enum Direction
	{
		Left,
		Right
	};

	private Direction direction = Direction.Right;

	void Awake()
	{
		// Setting up references.
	    // groundCheck = transform.Find("groundCheck"); // ADDED BACK IN
		anim = GetComponent<Animator>();

		GetComponent<Rigidbody2D> ().mass = 2f;
		//GetComponent<Rigidbody2D>().centerOfMass = new Vector2()
	}

	void FireBullet()
	{
		GameObject newPod = Instantiate<GameObject> (bullet);
		Rigidbody2D body = newPod.GetComponent<Rigidbody2D> ();

		Vector2 vel = new Vector2 (15.0f, 0.0f);
		if (facingRight) {
			vel.x *= -1;
		}
		newPod.transform.position = muzzle.position;
		body.velocity = muzzle.rotation*vel;
		GetComponent<Rigidbody2D> ().AddForce (-(muzzle.rotation * vel) * recoil);
		newPod.transform.position += new Vector3(vel.x * 0.01f,0,0);
	}

	void Update()
	{
		// The player is grounded if a linecast to the groundcheck position hits anything on the ground layer.
		// grounded = Physics2D.Linecast(transform.position, groundCheck.position, 1 << LayerMask.NameToLayer("Ground"));
		grounded = Physics2D.IsTouchingLayers(GetComponent<BoxCollider2D>(), ~(1 << LayerMask.NameToLayer("Player")));

		// If the jump button is pressed and the player is grounded then the player should jump.
		if(Input.GetButtonDown("Jump") && grounded) 
			jump = true;

		if (Input.GetButtonDown ("Fire1")) {
			FireBullet ();
		}
		Vector3 v3T = Input.mousePosition;
		v3T.z = Mathf.Abs(Camera.main.transform.position.y - transform.position.y);
		v3T = Camera.main.ScreenToWorldPoint(v3T);
		v3T.z = gun.transform.position.z;
		gun.transform.LookAt (v3T);
		gun.transform.rotation = Quaternion.Euler (0, 0, -90) *(gun.transform.rotation*Quaternion.Euler (0, 90, 90)) ;

		//

		Vector3 pos = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0);
		float WorldXPos = Camera.main.ScreenToWorldPoint(pos).x;

		if(WorldXPos > transform.position.x) // character it's your char game object
		{
			// Facing left
			FlipToDirection (Direction.Left);
		} else {
			// Facing right
			FlipToDirection(Direction.Right);
		} 
	}

	void FixedUpdate ()
	{
		Rigidbody2D body = GetComponent<Rigidbody2D> ();
		// Cache the horizontal input.
		float h = Input.GetAxis("Horizontal");

		// The Speed animator parameter is set to the absolute value of the horizontal input.
		anim.SetFloat("Speed", Mathf.Abs(h));
		//anim.SetFloat("Speed", 1);
		// If the player is changing direction (h has a different sign to velocity.x) or hasn't reached maxSpeed yet...
		if(h * body.velocity.x < maxSpeed)
			// ... add a force to the player.
			body.AddForce(Vector2.right * h * moveForce);

		// If the player's horizontal velocity is greater than the maxSpeed...
		if(Mathf.Abs(body.velocity.x) > maxSpeed)
			// ... set the player's velocity to the maxSpeed in the x axis.
			body.velocity = new Vector2(Mathf.Sign(GetComponent<Rigidbody2D>().velocity.x) * maxSpeed, GetComponent<Rigidbody2D>().velocity.y);

		// If the input is moving the player right and the player is facing left... (changed from > to < to fix bug of player moonwalking)
		if(h < 0 && !facingRight)
			// ... flip the player.
			Flip();
		// Otherwise if the input is moving the player left and the player is facing right... (same as above)
		else if(h > 0 && facingRight)
			// ... flip the player.
			Flip();


		body.AddTorque (-springyForce * body.rotation * body.mass);
		//gameObject.transform.localEulerAngles.z;

		// if horizontal velocity is greater than one, play the walk animation?
		//if (Mathf.Abs (GetComponent<Rigidbody2D> ().velocity.x) > 0.1)
		//{
		//	anim.SetTrigger ("Walk");
		//}

		//anim.SetFloat("Walk",h);

		//if (Mathf.Abs (GetComponent<Rigidbody2D> ().velocity.x) < 0.01)
		//{
		//	anim.SetTrigger ("Idle");
		//}

		//float w = Input.GetAxis("Horizontal");

	//	animator.SetFloat("Walk",w);

		// If the player should jump...
		if(jump)
		{
			// Set the Jump animator trigger parameter.
		//	anim.SetTrigger("Jump");

			// Play a random jump audio clip.
			// int i = Random.Range(0, jumpClips.Length);
			// AudioSource.PlayClipAtPoint(jumpClips[i], transform.position);

			// Add a vertical force to the player.
			GetComponent<Rigidbody2D>().AddForce(new Vector2(1f, jumpForce));

			// Make sure the player can't jump again until the jump conditions from Update are satisfied.
			jump = false;
		}

	}


	void Flip ()
	{
		// Switch the way the player is labelled as facing.
		//facingRight = !facingRight;

		// Multiply the player's x local scale by -1.
		//Vector3 theScale = transform.localScale;
		//theScale.x *= -1;
		//transform.localScale = theScale;
		if (direction == Direction.Left)
			FlipToDirection (Direction.Right);
		else
			FlipToDirection (Direction.Left);
	}

	void FlipToDirection (Direction newDir)
	{
		if (newDir == direction) {
			return;
		}
		Vector3 theScale = transform.localScale;
		theScale.x *= -1;
		transform.localScale = theScale;
		direction = newDir;
	}

// 	public IEnumerator Taunt()
//	{
		// Check the random chance of taunting.
		// float tauntChance = Random.Range(0f, 100f);
		// if(tauntChance > tauntProbability)
		// {
			// Wait for tauntDelay number of seconds.
			// yield return new WaitForSeconds(tauntDelay);

			// If there is no clip currently playing.
		//	if(!GetComponent<AudioSource>().isPlaying)
		//	{
				// Choose a random, but different taunt.
		//		tauntIndex = TauntRandom();

				// Play the new taunt.
		//		GetComponent<AudioSource>().clip = taunts[tauntIndex];
		//		GetComponent<AudioSource>().Play();
	//		}
	//	}
//	}


//	int TauntRandom()
//	{
		// Choose a random index of the taunts array.
//		int i = Random.Range(0, taunts.Length);

		// If it's the same as the previous taunt...
//		if(i == tauntIndex)
			// ... try another random taunt.
//			return TauntRandom();
	//	else
			// Otherwise return this index.
	//		return i;
//	}
}
