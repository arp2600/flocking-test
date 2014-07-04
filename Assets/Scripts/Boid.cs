using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// Boid created by the BoidController class
public class Boid : MonoBehaviour 
{
	private static List<Rigidbody2D> _boids = new List<Rigidbody2D>(); // A list of all the boids rigidbodies in the scene
	private BoidController _boid_controller; // The boid controller

	private float _left, _right, _top, _bottom, _width, _height; // Screen positions in world space, used for wrapping the boids at the edge of the screen

	void Start ()
	{
		// Get the boid controller from the parent
		_boid_controller = GetComponentInParent<BoidController>();

		// Add the boids rigidbody2D to the boids list
		_boids.Add(rigidbody2D);

		// Give the boid a random starting velocity
		Vector2 vel = Random.insideUnitCircle;
		vel *= Random.Range(20, 40);
		rigidbody2D.velocity = vel;

		// Get some camera coordinates in world coordinates
		_left = Camera.main.ScreenToWorldPoint(Vector2.zero).x;
		_bottom = Camera.main.ScreenToWorldPoint(Vector2.zero).y;
		_top = Camera.main.ScreenToWorldPoint(new Vector2(0, Screen.height)).y;
		_right = Camera.main.ScreenToWorldPoint(new Vector2(Screen.width, 0)).x;
		_width = _right - _left;
		_height = _top - _bottom;
	}

	// Fixed update used when dealing with rigid bodies
	void FixedUpdate () 
	{
		// Get the cohesion, alignment, and separation components of the flocking
		Vector2 acceleration = Cohesion() * _boid_controller._cohesion_weight;
		acceleration += Alignment() * _boid_controller._alignment_weight;
		acceleration += Separation() * _boid_controller._separation_weight;
		// Clamp the acceleration to a maximum value
		acceleration = Vector2.ClampMagnitude(acceleration, _boid_controller._max_acceleration);

		// Add the force to the rigid body and face the direction of movement
		rigidbody2D.AddForce(acceleration * Time.fixedDeltaTime);
		FaceTowardsHeading();

		// When going off screen, wrap to the opposite screen edge
		Wrap();
	}

	// Face the rigid body towards the direction of travel
	void FaceTowardsHeading()
	{
		Vector2 heading = rigidbody2D.velocity.normalized;
		float rotation = -Mathf.Atan2(heading.x, heading.y)*Mathf.Rad2Deg;
		rigidbody2D.MoveRotation(rotation);
	}

	// Wrap the edges of the screen to keep the boids from going off screen
	void Wrap ()
	{
		if (rigidbody2D.position.x < _left)
			rigidbody2D.position = rigidbody2D.position + new Vector2(_width, 0);
		else if (rigidbody2D.position.x > _right)
			rigidbody2D.position = rigidbody2D.position - new Vector2(_width, 0);
		if (rigidbody2D.position.y < _bottom)
			rigidbody2D.position = rigidbody2D.position + new Vector2(0, _height);
		else if (rigidbody2D.position.y > _top)
			rigidbody2D.position = rigidbody2D.position - new Vector2(0, _height);
	}

	// Calculate the cohesive component of the flocking algorithm
	Vector2 Cohesion ()
	{
		Vector2 sum_vector = new Vector2();
		int count = 0;

		// For each boid, check the distance from this boid, and if withing a neighbourhood, add to the sum_vector
		for (int i=0; i<_boids.Count; i++)
		{
			float dist = Vector2.Distance(rigidbody2D.position, _boids[i].position);

			if (dist < _boid_controller._cohesion_radius && dist > 0) // dist > 0 prevents including this boid
			{
				sum_vector += _boids[i].position;
				count++;
			}
		}

		// Average the sum_vector and return value
		if (count > 0)
		{
			sum_vector /= count;
			return  sum_vector - rigidbody2D.position;
		}

		return sum_vector; // Sum vector is empty here
	}

	// Calculate the alignment component of the flocking algorithm
	Vector2 Alignment ()
	{
		Vector2 sum_vector = new Vector2();
		int count = 0;

		// For each boid, check the distance from this boid, and if withing a neighbourhood, add to the sum_vector
		for (int i=0; i<_boids.Count; i++)
		{
			float dist = Vector2.Distance(rigidbody2D.position, _boids[i].position);

			if (dist < _boid_controller._cohesion_radius && dist > 0) // dist > 0 prevents including this boid
			{
				sum_vector += _boids[i].velocity;
				count++;
			}
		}

		// Average the sum_vector and clamp magnitude
		if (count > 0)
		{
			sum_vector /= count;
			sum_vector = Vector2.ClampMagnitude(sum_vector, 1);
		}

		return sum_vector;
	}

	// Calculate the separation component of the flocking algorithm
	Vector2 Separation ()
	{
		Vector2 sum_vector = new Vector2();
		int count = 0;

		// For each boid, check the distance from this boid, and if withing a neighbourhood, add to the sum_vector
		for (int i=0; i<_boids.Count; i++)
		{
			float dist = Vector2.Distance(rigidbody2D.position, _boids[i].position);

			if (dist < _boid_controller._separation_radius && dist > 0) // dist > 0 prevents including this boid
			{
				sum_vector += (rigidbody2D.position - _boids[i].position).normalized / dist;
				count++;
			}
		}

		// Average the sum_vector
		if (count > 0)
		{
			sum_vector /= count;
		}
		return  sum_vector;
	}

	// Draw the radius of the cohesion neighbourhood in green, and the radius of the separation neighbourhood in red, in the scene view
	void OnDrawGizmosSelected ()
	{
		Gizmos.color = Color.green;
		Gizmos.DrawWireSphere(transform.position, _boid_controller._cohesion_radius);
		Gizmos.color = Color.red;
		Gizmos.DrawWireSphere(transform.position, _boid_controller._separation_radius);
	}
}
