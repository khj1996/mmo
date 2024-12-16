using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : Poolable
{
    public override void OnGetFromPool()
    {
        gameObject.SetActive(true);
    }

    public override void OnReturnToPool()
    {
        gameObject.SetActive(false);
    }
}