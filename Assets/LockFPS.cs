using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LockFPS : MonoBehaviour
{
    [SerializeField] int FPSTarget = 60;
    int CurrentFPS;
    // Start is called before the first frame update
    void Start()
    {
        OnNLockFPS();
    }
    private void Update()
    {
        if (FPSTarget != CurrentFPS)
        {
            OnNLockFPS();
        }
    }
    void OnNLockFPS()
    {
        Application.targetFrameRate = FPSTarget;
        CurrentFPS = FPSTarget;
    }
}
