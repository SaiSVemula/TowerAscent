public interface IEnemy
{
    string EnemyName { get; }
    int EnemyCurrentHealth { get; }
    void Initialize(EnemyDifficulty difficulty);
    void TakeDamage(int damageAmount);
    void AttackPlayer(PlayerBattle player);
}

public enum EnemyDifficulty
{
    Easy,
    Medium,
    Hard
}

public enum EnemyLevel
{
    Level1,
    Level2,
    Level3
}
