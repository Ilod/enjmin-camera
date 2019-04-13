using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public abstract class Decision : MonoBehaviour {
    
    public abstract bool ShouldGrabCamera();
    public abstract Color InitColor();
    public abstract Vector2 GetMove();
    public abstract bool IsShooting();
    public abstract bool IsStartingInvincibility();
    public abstract bool IsStartingAoe();
}
