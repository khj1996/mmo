using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HpBar : MonoBehaviour
{
    [SerializeField] private Image _healthBarSprite;
    [SerializeField] private float _reduceSpeed = 2f;
    [SerializeField] private float fillAmount = 1;

    private Camera _mainCamera;

    private void Start()
    {
        _mainCamera = Camera.main;
    }

    public void UpdateHealthBar(float current, float max)
    {
        fillAmount = current / max;
    }

    private void Update()
    {
        transform.rotation = Quaternion.LookRotation(transform.position - _mainCamera.transform.position);
        _healthBarSprite.fillAmount = Mathf.MoveTowards(_healthBarSprite.fillAmount, fillAmount, _reduceSpeed * Time.deltaTime);
    }
}