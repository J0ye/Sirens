using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public static GameObject Player;

    public int score = 0;

    private List<Evidence> evidence = new List<Evidence>();


    void Start()
    {
        if (!Instance) Instance = this;
        if (Instance != this) Destroy(gameObject);
        if (!Player) Player = GameObject.FindGameObjectWithTag("Player");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void CollectEvidence(Evidence target)
    {
        evidence.Add(target);
        score = evidence.Count;
    }
}
