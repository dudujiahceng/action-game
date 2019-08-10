using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XftWeapon;

public class WeaponTrail : MonoBehaviour {
    public XWeaponTrail ProTrailDistort;


    public void Start()
    {
        ProTrailDistort.Init();
        ProTrailDistort.Activate();
    }
}
