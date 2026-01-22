using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBrain : MonoBehaviour
{
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

    public void FixedUpdate()
    {
        if (GameState.instance.player != null)
        {
            MoveToPlayer?.Invoke(GameState.instance.playerTransform);
        }
    }
}
