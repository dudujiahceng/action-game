﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackTree{
    public string attackModeStr;
    public AttackTree leftChild;
    public AttackTree rightChild;
    public AttackTree parent;
    public bool isLoop;
    public bool key;
    //public bool needWeapon;


    public string AttackModeStr
    {
        get
        {
            return attackModeStr;
        }
    }

    public AttackTree LeftChild
    {
        get
        {
            return leftChild;
        }
        set
        {
            leftChild = value;
        }
    }
    public AttackTree RightChild
    {
        get
        {
            return rightChild;
        }
        set
        {
            rightChild = value;
        }
    }
    public AttackTree Parent
    {
        get
        {
            return parent;
        }
        set
        {
            parent = value;
        }
    }
    public AttackTree()
    {
        attackModeStr   = null;
        leftChild       = null;
        rightChild      = null;
        parent          = null;
        isLoop          = false;
        key             = false;
    }
}
