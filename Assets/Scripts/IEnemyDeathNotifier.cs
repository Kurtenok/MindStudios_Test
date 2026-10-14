using System;

public interface IEnemyDeathNotifier
{
    event Action<IEnemyDeathNotifier> Died;
}