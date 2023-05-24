using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

	[SerializeField] private float minTiltSmooth, maxTiltSmooth, hoverDistance, hoverSpeed;
    [SerializeField] private float gravity = -9.8f;
    [SerializeField] private float fallGravityScale = 2f;
    private float gravityScale = 1f;
    private Vector2 velocity = Vector2.zero;
    [SerializeField] private float jumpHeight = 1.2f;
    private bool start;
    private bool isDropped = false;
    private float timer, tiltSmooth, y;
	private Rigidbody2D playerRigid;
	private Quaternion downRotation, upRotation;

	void Start () {
		tiltSmooth = maxTiltSmooth;
		playerRigid = GetComponent<Rigidbody2D> ();
		downRotation = Quaternion.Euler (0, 0, -90);
		upRotation = Quaternion.Euler (0, 0, 35);
        isDropped = false;
        gravityScale = 1f;
    }

	void GravityUpdate()
	{
		if(isDropped) return;

        velocity.y += gravity * gravityScale * Time.deltaTime;
        playerRigid.velocity = velocity;
    }

	void Update () {
		if (!start) {
			// Hover the player before starting the game
			timer += Time.deltaTime;
			y = hoverDistance * Mathf.Sin (hoverSpeed * timer);
			transform.localPosition = new Vector3 (0, y, 0);
		} else {
			// Rotate downward while falling
			transform.rotation = Quaternion.Lerp (transform.rotation, downRotation, tiltSmooth * Time.deltaTime);
		}
		// Limit the rotation that can occur to the player
		transform.rotation = new Quaternion (transform.rotation.x, transform.rotation.y, Mathf.Clamp (transform.rotation.z, downRotation.z, upRotation.z), transform.rotation.w);
	}

	void LateUpdate () {
		if (GameManager.Instance.GameState ()) {
			if (Input.GetMouseButtonDown (0)) {
				if(!start){
					// This code checks the first tap. After first tap the tutorial image is removed and game starts
					start = true;
					GameManager.Instance.GetReady ();
					GetComponent<Animator>().speed = 2;
				}

				tiltSmooth = minTiltSmooth;
				transform.rotation = upRotation;
				playerRigid.velocity = Vector2.zero;
                // Push the player upwards
                velocity.y = Mathf.Sqrt(jumpHeight * -2 * gravity);
                gravityScale = 1f;

                SoundManager.Instance.PlayTheAudio("Flap");
			}
		}
		if (playerRigid.velocity.y < -1f) {
			// Increase gravity so that downward motion is faster than upward motion
			tiltSmooth = maxTiltSmooth;
            gravityScale = fallGravityScale;
        }
        GravityUpdate();
    }

	void OnTriggerEnter2D (Collider2D col) {
		if (col.transform.CompareTag ("Score")) {
			Destroy (col.gameObject);
			GameManager.Instance.UpdateScore ();
		} else if (col.transform.CompareTag ("Obstacle")) {
			// Destroy the Obstacles after they reach a certain area on the screen
			foreach (Transform child in col.transform.parent.transform) {
				child.gameObject.GetComponent<BoxCollider2D> ().enabled = false;
			}
			KillPlayer ();
		}
	}

	void OnCollisionEnter2D (Collision2D col) {
		if (col.transform.CompareTag ("Ground")) {
            playerRigid.simulated = false;
			KillPlayer ();
			transform.rotation = downRotation;
            isDropped = true;
		}
	}

	public void KillPlayer () {
        GameManager.Instance.EndGame ();

		// Stop the flapping animation
		GetComponent<Animator> ().enabled = false;
        // Change color to black and white
        GetComponent<SpriteRenderer>().material.SetFloat(Shader.PropertyToID("_EffectAmount"), 1.0f);
    }

	public void RandomFlappyColor()
	{
        GetComponent<Animator>().SetInteger(Animator.StringToHash("PlayerNum"), Random.Range(0, 3));
    }
}