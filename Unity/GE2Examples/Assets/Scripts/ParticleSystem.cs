using UnityEngine;
using System.Collections;

public class ParticleSystem : MonoBehaviour {

    public float aliveTime;

	void Start () {
        Destroy(gameObject, aliveTime);
	}
	
}
