using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCDecision : Decision {
    private Vector2 direction;

    public override Vector2 GetMove()
    {
        return direction;
    }

    public override bool IsShooting() => Random.Range(0, 100) == 0;

    public override bool IsStartingAoe() => Random.Range(0, 300) == 0;

    public override bool IsStartingInvincibility() => Random.Range(0, 300) == 0;

    public override Color InitColor() => Color.red;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if (Random.Range(0, 50) == 0)
        {
            if (Random.Range(0, 3) != 0)
            {
                direction = new Vector2(Random.Range(-1.0f, 1.0f), Random.Range(-1.0f, 1.0f));
            }
            else
            {
                direction = Vector2.zero;
            }
        }
	}

    public override bool ShouldGrabCamera() => false;
}
