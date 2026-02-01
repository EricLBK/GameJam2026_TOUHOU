using System.Collections;
using Bullets;
using Enemy;
using UnityEngine;

public class LevelDirector : MonoBehaviour
{
    private EnemyManager _enemyManager;
    private BulletManager _bulletManager;
    private Rect _bounds;
    
    private void Start()
    {
        _enemyManager = gameObject.GetComponent<EnemyManager>();
        _bulletManager = gameObject.GetComponent<BulletManager>();
        _bounds = FieldOfPlayBounds.Instance.Bounds;
        Phase1();
        
        GameEventManager.Instance.BeginTimeline();
    }

    private IEnumerator SpawnFairy1()
    {
        var fairyPrefab = Resources.Load<GameObject>("Prefab/Enemy/Fairy1");
        var enemyPath = ScriptableObject.CreateInstance<EnemyPath>();
        enemyPath.points = new[]
        {
            new Vector2(_bounds.xMax, _bounds.yMax),
            new Vector2(0, 0),
            new Vector2(0, 0),
            new Vector2(_bounds.xMin, _bounds.yMin)
        };
        
        _enemyManager.SpawnEnemy(enemyPath, 10, 20, fairyPrefab, speed:0.5f);
        yield return null;
    }

    private IEnumerator SpawnBullets()
    {
        _enemyManager.SpawnPattern(Patterns.Spiral(duration: 2f));
        yield return new WaitForSeconds(0.5f);
    }

    // private IEnumerator StartDialogue1()
    // {
    //     
    // }
    //
    // private IEnumerator StartDialogue2()
    // {
    //     
    // }
    //
    // private IEnumerator StartDialogue3()
    // {
    //     
    // }

    private void Phase1()
    {
        for (var i = 2; i < 10; i++)
        {
            GameEventManager.Instance.ScheduleEvent(i - 0.5f, SpawnFairy1);
            GameEventManager.Instance.ScheduleEvent(i + i * 0.1f, SpawnBullets);
        }
    }
    
}
