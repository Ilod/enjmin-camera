using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

public class PlayerAI : MonoBehaviour {
    public string baseLayer;

    public Decision decision;

    public float speed = 2;
    public GameObject rotationObject;
    public GameObject arrowPrefab;
    public float arrowDelay = .5f;
    private float timeBeforeNextArrow = 0;
    public float respawnTime = 3;

    public AoeAI aoe;
    public GameObject invincibility;

    public float spawnInvincibilityDuration = 1;
    public float invincibilityDuration = 1;
    public float invincibilityCooldown = 20;
    private float invincibilityRemaining = 0;
    private float timeBeforeNextInvincibility = 0;

    public float aoeDuration = 3;
    public float aoeTimeToKill = 0.75f;
    public float aoeCooldown = 15;
    private float aoeRemaining = 0;
    public float aoeHealSpeed = 0.5f;
    private float timeBeforeNextAoe = 0;
    
    private float insideAoeDuration = 0;

    public bool dead = false;
    public Color color;

    [Serializable]
    public class IntEvent : UnityEvent<int> { }

    public IntEvent onScoreChanged;

	// Use this for initialization
	void Start ()
    {
        if (decision.ShouldGrabCamera())
        {
            var camera = Camera.main;
            camera.transform.parent = gameObject.transform;
            var position = camera.transform.localPosition;

            position.x = 0;
            position.y = 0;
            camera.transform.localPosition = position;
        }

        color = decision.InitColor();
        GetComponentInChildren<SpriteRenderer>().color = color;
        gameObject.layer = LayerMask.NameToLayer(baseLayer);
        aoe.player = this;
    }
	
	// Update is called once per frame
	void Update ()
    {
        if (timeBeforeNextArrow > 0)
            timeBeforeNextArrow -= Time.deltaTime;
        if (timeBeforeNextInvincibility > 0)
            timeBeforeNextInvincibility -= Time.deltaTime;
        if (timeBeforeNextAoe > 0)
            timeBeforeNextAoe -= Time.deltaTime;

        if (dead)
            return;

        insideAoeDuration -= Time.deltaTime * aoeHealSpeed;
        if (insideAoeDuration < 0)
            insideAoeDuration = 0;

        if (invincibilityRemaining > 0)
        {
            invincibilityRemaining -= Time.deltaTime;
            if (invincibilityRemaining <= 0)
                EndInvincibility();
        }

        if (aoeRemaining > 0)
        {
            aoeRemaining -= Time.deltaTime;
            if (aoeRemaining <= 0)
                EndAoe();
        }

        var horizontal = Input.GetAxis("Horizontal");
        var vertical = Input.GetAxis("Vertical");

        Vector2 move = decision.GetMove().normalized * Math.Max(Math.Abs(horizontal), Math.Abs(vertical)) * speed * Time.deltaTime;

        gameObject.transform.Translate(move.x, move.y, 0);

        if (move != Vector2.zero)
        {
            rotationObject.transform.eulerAngles = (Vector3.forward * Vector2.SignedAngle(Vector2.up, move));
        }
        if (decision.IsShooting() && timeBeforeNextArrow <= 0)
        {
            ShootArrow();
        }
        if (decision.IsStartingInvincibility() && timeBeforeNextInvincibility <= 0)
        {
            StartInvincibility();
        }
        if (decision.IsStartingAoe() && timeBeforeNextAoe <= 0)
        {
            StartAoe();
        }
    }

    public void StartInvincibility()
    {
        invincibilityRemaining = invincibilityDuration;
        timeBeforeNextInvincibility = invincibilityCooldown;
        invincibility.SetActive(true);
    }

    public void EndInvincibility()
    {
        invincibilityRemaining = 0;
        invincibility.SetActive(false);
    }

    public void StartAoe()
    {
        aoeRemaining = aoeDuration;
        timeBeforeNextAoe = aoeCooldown;
        aoe.gameObject.SetActive(true);
    }

    public void EndAoe()
    {
        aoeRemaining = 0;
        aoe.gameObject.SetActive(false);
    }

    private void ShootArrow()
    {
        timeBeforeNextArrow = arrowDelay;
        var arrow = Instantiate(arrowPrefab, rotationObject.transform.position, rotationObject.transform.rotation);
        arrow.GetComponent<ArrowAI>().player = this;
        arrow.GetComponent<SpriteRenderer>().color = color;
        IncrementScore(-1);
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.GetComponent<AoeAI>() != null)
        {
            if (invincibilityRemaining > 0)
            {
                insideAoeDuration = 0;
            }
            else
            {
                insideAoeDuration += Time.deltaTime;
                if (insideAoeDuration >= aoeTimeToKill)
                    KillBy(collision.GetComponent<AoeAI>().player);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (dead)
            return;
        if (collision.GetComponent<ArrowAI>() != null)
        {
            KillBy(collision.GetComponent<ArrowAI>().player);
            Destroy(collision.gameObject);
        }
        if (collision.GetComponent<ArrowAI>() != null)
        {
            KillBy(collision.GetComponent<ArrowAI>().player);
            Destroy(collision.gameObject);
        }
        if (collision.GetComponent<ContactKill>() != null)
        {
            KillBy(null);
        }
    }

    private void KillBy(PlayerAI killer)
    {
        if (invincibilityRemaining > 0)
            return;
        IncrementScore(-10);
        StartCoroutine(Reactivate());
        rotationObject.SetActive(false);
        EndInvincibility();
        invincibilityRemaining = spawnInvincibilityDuration;
        EndAoe();
        insideAoeDuration = 0;
        if (killer != null)
            killer.IncrementScore(20);
        dead = true;
    }

    private void IncrementScore(int increment)
    {
        onScoreChanged.Invoke(increment);
    }

    private IEnumerator Reactivate()
    {
        yield return new WaitForSeconds(respawnTime);
        if (dead)
        {
            rotationObject.SetActive(true);
            dead = false;
        }
    }

    public void EndGame()
    {
        timeBeforeNextArrow = 0;
        EndInvincibility();
        EndAoe();
        timeBeforeNextInvincibility = 0;
        timeBeforeNextAoe = 0;
        insideAoeDuration = 0;
        StopAllCoroutines();
        rotationObject.SetActive(true);
        dead = false;
        gameObject.SetActive(false);
    }

    public void StartGame()
    {
        gameObject.SetActive(true);
    }
}
