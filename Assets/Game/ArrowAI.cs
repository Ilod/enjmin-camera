using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowAI : MonoBehaviour {
    public float speed = 3;
    public PlayerAI player;
    public float lifeDuration = 10;

	// Use this for initialization
	void Start ()
    {
        gameObject.layer = LayerMask.NameToLayer($"{player.baseLayer}Arrows");
    }
	
	// Update is called once per frame
	void Update ()
    {
        gameObject.transform.Translate(Vector3.up * speed * Time.deltaTime);
        lifeDuration -= Time.deltaTime;
        if (lifeDuration < 0)
            Destroy(gameObject);
    }
}
