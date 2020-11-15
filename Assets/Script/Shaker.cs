using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Shaker : MonoBehaviour
{
    public float duration = 1f;
    public Vector3 strength;
    public int vibrato = 0;
    public float randomness = 0f;
    public bool snap = true;
    public bool fadeOut = true;

    private bool ready = true;

    void Update()
    {
        Shake();
    }

    public void Shake()
    {
        transform.DOShakePosition(duration, strength, vibrato, randomness, snap, fadeOut);
        StartCoroutine(Pause());
    }

    public IEnumerator Pause()
    {
        ready = false;
        yield return new WaitForSeconds(duration);
        ready = true;
    }
}
