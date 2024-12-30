public interface IEnemy
{
    string EnemyName { get; }
    int EnemyCurrentHealth { get; }
    void Initialize(Difficulty difficulty);
    void TakeDamage(int damageAmount);
    void AttackPlayer(PlayerBattle player);
}

public enum Difficulty
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
