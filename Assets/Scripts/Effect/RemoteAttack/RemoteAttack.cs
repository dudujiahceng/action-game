using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RemoteAttack : MonoBehaviour {
    public Transform firePoint;
    public List<GameObject> remoteAttacks = new List<GameObject>();

    private void SpawnFireBall(Transform target, int index)
    {
        GameObject fire = GameObject.Instantiate(remoteAttacks[index], firePoint.transform.position, Quaternion.identity);
        fire.GetComponent<ProjectileController>().target = target;
        fire.GetComponent<ProjectileController>().speed = 100;
        fire.GetComponent<ProjectileController>().damage = 40;
        fire.transform.LookAt(target);
    }

    public void FireBallAttack(int index)
    {
        SpawnFireBall(transform.GetComponent<Monster>().lockedTarget.transform, index);
    }
}
