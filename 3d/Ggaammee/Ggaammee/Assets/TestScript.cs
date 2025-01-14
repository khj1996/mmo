using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class TestScript : MonoBehaviour
{
    private DecalProjector fullRange;
    private DecalProjector progressRange;
    private MaterialPropertyBlock propertyBlock;

    public void tttt()
    {/*
        propertyBlock = new MaterialPropertyBlock();
        fullRange.material.get(propertyBlock);
        propertyBlock.SetFloat("_Progress", progress); // 쉐이더의 프로퍼티 이름이 "_Progress"일 경우
        objRenderer.SetPropertyBlock(propertyBlock);*/
    }


    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
    }
}