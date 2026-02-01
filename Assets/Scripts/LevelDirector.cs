using System.Collections;
using Bullets;
using Enemy;
using UnityEngine;

public class LevelDirector : MonoBehaviour
{
    private EnemyManager _enemyManager;
    private BulletManager _bulletManager;
    
    private void Start()
    {
        _enemyManager = gameObject.GetComponent<EnemyManager>();
        _bulletManager = gameObject.GetComponent<BulletManager>();
        GameEventManager.Instance.ScheduleEvent(2.0f, SpawnFairy1);
        GameEventManager.Instance.ScheduleEvent(2.5f, SpawnBullets);
        GameEventManager.Instance.ScheduleEvent(3.0f, SpawnFairy1);
        GameEventManager.Instance.ScheduleEvent(3.2f, SpawnBullets);
        
        GameEventManager.Instance.BeginTimeline();
    }

    private IEnumerator SpawnFairy1()
    {
        var fairyPrefab = Resources.Load<GameObject>("Prefab/Enemy/Fairy1");
        var enemyPath = ScriptableObject.CreateInstance<EnemyPath>();
        enemyPath.points = new[]
        {
            new Vector2(400, 400),
            new Vector2(100, 100),
            new Vector2(100, 100),
            new Vector2(600, 100)
        };
        
        _enemyManager.SpawnEnemy(enemyPath, 10, 40, fairyPrefab, speed:0.5f);
        yield return null;
    }

    private IEnumerator SpawnBullets()
    {
        _enemyManager.SpawnPattern(Patterns.Spiral(duration: 2f));
        yield return new WaitForSeconds(1.0f);
    }
}
