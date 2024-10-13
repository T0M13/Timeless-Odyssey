using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImpactGrenade : Grenade
{
    protected override void Start()
    {
        //Do Nothing
    }

    protected virtual void OnCollisionEnter(Collision collision)
    {
        Explode();
    }
}
