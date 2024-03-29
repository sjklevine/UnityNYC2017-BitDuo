﻿using UnityEngine;
using System.Collections;

public class PlayerControl : MonoBehaviour
{
	[HideInInspector]
	public bool facingRight = true;			// For determining which way the player is currently facing.
	[HideInInspector]
	public bool jump = false;				// Condition for whether the player should jump.

	public GameObject sprite_idle;
	public GameObject sprite_walking;
	public GameObject sprite_jump;

	public float bounceForce = 50f;
	public float moveForce = 365f;			// Amount of force added to move the player left and right.
	public float maxSpeed = 5f;				// The fastest the player can travel in the x axis.
	public AudioClip[] jumpClips;			// Array of clips for when the player jumps.
	public float jumpForce = 1000f;			// Amount of force added when the player jumps.
	private bool grounded = false;			// Whether or not the player is grounded.
	private Animator anim;					// Reference to the player's animator component.
	private Vector3 lastGroundedPosition;
	private SidescrollerGameManager gm;     // Cached reference to game manager


	void Awake()
	{
		// Setting up references.
		anim = GetComponent<Animator>();
		InitializeSprites();
		gm = SidescrollerGameManager.instance;
	}


	void Update()
	{

		// If the jump button is pressed and the player is grounded then the player should jump.
		if (Input.GetButtonDown ("PCPlayerJump") && grounded) {
			jump = true;
		}
		if (grounded) {
			lastGroundedPosition = transform.position;
		}
	}


	void FixedUpdate ()
	{
		if (gm.state == SidescrollerGameManager.GameState.Running) {
			// Cache the horizontal input.
			float h = Input.GetAxis ("PCPlayerHorizontal");

			// The Speed animator parameter is set to the absolute value of the horizontal input.
			anim.SetFloat ("Speed", Mathf.Abs (h));

			// If the player is changing direction (h has a different sign to velocity.x) or hasn't reached maxSpeed yet...
			if (h * GetComponent<Rigidbody> ().velocity.x < maxSpeed)
				// ... add a force to the player.
				GetComponent<Rigidbody> ().AddForce (Vector2.left * h * moveForce);

			// If the player's horizontal velocity is greater than the maxSpeed...
			if (Mathf.Abs (GetComponent<Rigidbody> ().velocity.x) > maxSpeed)
				// ... set the player's velocity to the maxSpeed in the x axis.
				GetComponent<Rigidbody> ().velocity = new Vector2 (Mathf.Sign (GetComponent<Rigidbody> ().velocity.x) * maxSpeed, GetComponent<Rigidbody> ().velocity.y);

			// If the input is moving the player right and the player is facing left...
			if (h > 0 && !facingRight)
				// ... flip the player.
				Flip ();
			// Otherwise if the input is moving the player left and the player is facing right...
			else if (h < 0 && facingRight)
				// ... flip the player.
				Flip ();

			// If the player should jump...
			if (jump) {
				// Set the Jump animator trigger parameter.
				anim.SetTrigger ("Jump");

				// Play a random jump audio clip.
				int i = Random.Range (0, jumpClips.Length);
				AudioSource.PlayClipAtPoint (jumpClips [i], transform.position);

				// Add a vertical force to the player.
				GetComponent<Rigidbody> ().AddForce (new Vector2 (0f, jumpForce));

				// Make sure the player can't jump again until the jump conditions from Update are satisfied.
				jump = false;
			}
		}
	}


	void Flip ()
	{
		// Switch the way the player is labelled as facing.
		facingRight = !facingRight;

		// Multiply the player's x local scale by -1.
		Vector3 theScale = transform.localScale;
		theScale.x *= -1;
		transform.localScale = theScale;
	}

	private void InitializeSprites(){
		sprite_idle.SetActive (true);
		sprite_walking.SetActive (false);
		sprite_jump.SetActive (false);
	}

	public void TeleportToLastSafePosition() {
		this.transform.position = this.lastGroundedPosition + Vector3.up * 1.0f;
		GetComponent<Rigidbody> ().velocity = Vector3.zero;
	}

	void OnCollisionEnter(Collision col){
		if (col.collider.gameObject.layer.Equals(LayerMask.NameToLayer("Ground")) || (col.collider.gameObject.layer.Equals(LayerMask.NameToLayer("GroundUnmovable")))) {
			//this.transform.parent = col.gameObject.transform;
			grounded = true;
			//bounce if we hit a vertical surface
			if (Mathf.Abs(col.contacts [0].normal.x) == 1.0) {
				GetComponent<Rigidbody>().AddForce(new Vector2(col.contacts [0].normal.x * bounceForce, 0.0f));
			}
		}
	}

	void OnCollisionStay(Collision col){
		if (col.collider.gameObject.layer.Equals(LayerMask.NameToLayer("Ground")) || (col.collider.gameObject.layer.Equals(LayerMask.NameToLayer("GroundUnmovable")))) {
			grounded = true;
			//bounce if we hit a vertical surface
			if (Mathf.Abs(col.contacts [0].normal.x) == 1.0) {
				GetComponent<Rigidbody>().AddForce(new Vector2(col.contacts [0].normal.x * bounceForce, 0.0f));
			}
		}
	}

	void OnCollisionExit(Collision col){
		if (col.collider.gameObject.layer.Equals(LayerMask.NameToLayer("Ground")) || (col.collider.gameObject.layer.Equals(LayerMask.NameToLayer("GroundUnmovable")))) {
			grounded = false;
		}
	}
}
