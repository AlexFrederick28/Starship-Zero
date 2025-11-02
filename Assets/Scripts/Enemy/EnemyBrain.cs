using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBrain : MonoBehaviour
{
    [SerializeField] private PlayerBase player;

    public delegate void EnemyBrainDelegate(Transform playerTransform);
    public EnemyBrainDelegate MoveToPlayer;

    public static EnemyBrain instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this);
        }
    }

    private void OnEnable()
    {
        if (player == null)
        {
            player = FindAnyObjectByType<PlayerBase>();
        }
    }

    public void FixedUpdate()
    {
        if (player != null)
        {
            MoveToPlayer?.Invoke(player.transform);
        }
    }
}
