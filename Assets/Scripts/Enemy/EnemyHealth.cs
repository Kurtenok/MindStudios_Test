using System;
using UnityEngine;

public class EnemyHealth : MonoBehaviour, IEnemyDeathNotifier
{
    public event Action<IEnemyDeathNotifier> Died;

    public void Kill()
    {
        Died?.Invoke(this);

        Destroy(gameObject);
    }
}