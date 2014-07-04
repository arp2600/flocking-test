using UnityEngine;
using System.Collections;

// Creates a flock of boids and defines values for the boids to use
public class BoidController : MonoBehaviour 
{
	public GameObject _boid_prefab; // The prefab boid object
	public int _number_of_boids = 0; // The number of boids to create upon startup

	public float _cohesion_radius = 30; // The radius of the neighbourhood used for cohesion
	public float _cohesion_weight = 30; // The affect cohesion has on the acceleration
	public float _alignment_weight = 1000; // The affect alignment has on the acceleration
	public float _separation_radius = 20; // The radius of the neighbourhood user for the separation
	public float _separation_weight = 5000; // The affect separation has on the acceleration
	public float _max_acceleration = 650; // The maximum acceleration allowed

	void Start () 
	{
		// Create all the boids and add them as a child of the controller
		for (int i=0; i<_number_of_boids; i++)
		{
			GameObject go = (GameObject)Instantiate(_boid_prefab);
			go.transform.parent = transform;
		}
	}
}
