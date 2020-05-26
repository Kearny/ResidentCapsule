﻿using UnityEngine;

public class BlinkingLight : MonoBehaviour
{
    [Header("Light")] [Tooltip("Chance to modify light status between 0.0 and 1.0 (included)")]
    [Range(0f,1f)]
    public float chanceToChangeLightStatus;

    private Light _light;

    // Start is called before the first frame update
    private void Start()
    {
        _light = GetComponent<Light>();
    }

    // Update is called once per frame
    private void Update()
    {
        if (Random.value > chanceToChangeLightStatus)
        {
            _light.enabled = _light.enabled != true;
        }
    }
}