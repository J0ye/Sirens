using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

[RequireComponent(typeof(Collider2D))]
public class Evidence : MonoBehaviour
{
    private Light2D glow;
    private void Start()
    {
        if (GetComponent<Light2D>()) glow = GetComponent<Light2D>();
    }
    public void OnTriggerEnter2D(Collider2D other)
    {
        if(other.gameObject.CompareTag("Player"))
        {
            GameManager.Instance.CollectEvidence(this);
            gameObject.SetActive(false);
        }
    }
}
