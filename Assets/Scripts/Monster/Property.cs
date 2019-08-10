using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Property : MonoBehaviour {
    public float walkSpeed;
    public float runSpeed;
    public bool onGround = true;
    private GameObject[] attackTargets;
    public List<GameObject> targetsList;
    public GameObject lockedTarget;
    private int stage = 1;



    protected void GetAllTarget()
    {
        attackTargets = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject obj in attackTargets)
        {
            targetsList.Add(obj);
        }
        lockedTarget = CurrentTarget();
    }

    public GameObject CurrentTarget()
    {
        if (targetsList.Count == 0)
            return null;
        else
        {
            foreach(GameObject obj in targetsList)
            {
                if (!obj)
                    targetsList.Remove(obj);
            }
            int index = Random.Range(0, targetsList.Count);
            lockedTarget = targetsList[index];
            return targetsList[index];
        }
    }

    public GameObject GetTarget()
    {
        return lockedTarget;
    }
    public float WalkSpeed
    {
        get
        {
            return walkSpeed;
        }
    }
    public float RunSpeed
    {
        get
        {
            return runSpeed;
        }
        set
        {
            runSpeed = value;
        }
    }
    public int GetStage
    {
        get
        {
            return stage;
        }
    }
}
