using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerDecision : Decision {
    private bool isLocalPlayer = false;

    void Start()
    {
        isLocalPlayer = GetComponent<NetworkIdentity>().isLocalPlayer;
    }

    public override Color InitColor() => Color.white;
    public override Vector2 GetMove()
    {
        if (!isLocalPlayer)
            return Vector2.zero;
        return new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
    }
    public override bool IsShooting()
    {
        if (!isLocalPlayer)
            return false;
        return Input.GetButton("Fire1");
    }
    public override bool IsStartingAoe()
    {
        if (!isLocalPlayer)
            return false;
        return Input.GetButton("Fire3");
    }
    public override bool IsStartingInvincibility()
    {
        if (!isLocalPlayer)
            return false;
        return Input.GetButton("Fire2");
    }

    public override bool ShouldGrabCamera() => isLocalPlayer;
}
