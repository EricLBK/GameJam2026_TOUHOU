using System;
using System.Collections;
using Bullets;
using Enemy;
using UnityEngine;

public class LevelDirector : MonoBehaviour
{
    private EnemyManager _enemyManager;
    private BulletManager _bulletManager;
    [SerializeField] private GameObject player;
    [SerializeField] private Canvas dialogueCanvas;
    [SerializeField] private GameObject dialogueManager;
    [SerializeField] private DialogueAsset dialogueAsset1;
    [SerializeField] private DialogueAsset dialogueAsset2;
    [SerializeField] private GameObject _bullet2; 
    [SerializeField] private GameObject _bullet3;
    private GameObject _bullet1;
    private DialogueController _dialogueController;
    
    private GameObject _horsePrefab;
    private GameObject _fairy1Prefab;
    private GameObject _fairy2Prefab;
    private ReimuShooter _shooterController;

    
    private Rect _bounds;

    private void Awake()
    {
        dialogueCanvas.enabled = false;
    }

    private void Start()
    {
        _enemyManager = gameObject.GetComponent<EnemyManager>();
        _bulletManager = gameObject.GetComponent<BulletManager>();
        _bounds = FieldOfPlayBounds.Instance.Bounds;
        _horsePrefab = Resources.Load<GameObject>("Prefab/Enemy/Horse_1_0");
        _fairy1Prefab = Resources.Load<GameObject>("Prefab/Enemy/Fairy1");
        _fairy2Prefab = Resources.Load<GameObject>("Prefab/Enemy/Fairy2");

        _bullet1 = Resources.Load<GameObject>("Prefab/Bullets/AnimatedBullet/Pink");
        _dialogueController = dialogueManager.GetComponent<DialogueController>();
        _shooterController = player.GetComponent<ReimuShooter>();
        
        Phase1();
        GameEventManager.Instance.ScheduleEvent(12.0f, StartDialogue1);
        Phase2();
        GameEventManager.Instance.ScheduleEvent(43f, StartDialogue2);
        
        GameEventManager.Instance.BeginTimeline();
    }

    private IEnumerator SpawnFairy1()
    {
        var enemyPath = ScriptableObject.CreateInstance<EnemyPath>();
        enemyPath.points = new[]
        {
            new Vector2(_bounds.xMax, _bounds.yMax),
            new Vector2(0, 0),
            new Vector2(0, 0),
            new Vector2(_bounds.xMin, _bounds.yMin)
        };
        
        _enemyManager.SpawnEnemy(enemyPath, 10, 20, _fairy1Prefab, speed:0.5f);
        yield return null;
    }

    private IEnumerator SpawnFairy2()
    {
        var enemyPath = ScriptableObject.CreateInstance<EnemyPath>();
        enemyPath.points = new[]
        {
            new Vector2(_bounds.xMax, _bounds.yMax),                  // P0: Start (Top Center)
            new Vector2(_bounds.xMax, _bounds.yMax / 2),       // P1: Loop Right side
            new Vector2(_bounds.xMax / 3, _bounds.yMax / 3),       // P2: Loop Left side
            new Vector2(0, _bounds.yMax)                   // P3: End (Top Center Edge)
        };
        _enemyManager.SpawnEnemy(enemyPath, 10, 30, _fairy2Prefab, speed:0.3f);
        yield return null;
    }

    private IEnumerator SpawnHorse()
    {
        var enemyPath = ScriptableObject.CreateInstance<EnemyPath>();
        enemyPath.points = new[]
        {
            new Vector2(_bounds.xMax, _bounds.yMax),
            new Vector2(_bounds.xMax / 2, _bounds.yMax /2),
            new Vector2(_bounds.xMax / 2, _bounds.yMax /2),
            new Vector2(_bounds.xMin, _bounds.yMax)
        };
        _enemyManager.SpawnEnemy(enemyPath, 10, 40, _horsePrefab, speed:0.3f);
        yield return null;
    }

    private IEnumerator SpawnSpiral()
    {
        _enemyManager.SpawnPattern(Patterns.Spiral(bulletSpeed: 100), duration: 2f);
        yield return new WaitForSeconds(0.5f);
    }

    private IEnumerator SpawnSpread()
    {
        _enemyManager.SpawnPattern(Patterns.SingleShot(Shots.Spread((Vector2)player.transform.position)), duration: 0.5f);
        yield return new WaitForSeconds(0.5f);
    }

    private IEnumerator StartDialogue(DialogueAsset dialogue)
    {
        dialogueCanvas.enabled = true;
        _shooterController.disableShooting = true;
        
        _dialogueController.StartDialogue(dialogue);
        yield return new WaitUntil(() => _dialogueController.IsFinished);
        dialogueCanvas.enabled = false;
        _shooterController.disableShooting = false;
        
    }

    private IEnumerator StartDialogue1()
    {
        dialogueCanvas.enabled = true;
        _shooterController.disableShooting = true;
        
        _dialogueController.StartDialogue(dialogueAsset1);
        yield return new WaitUntil(() => _dialogueController.IsFinished);
        dialogueCanvas.enabled = false;
        _shooterController.disableShooting = false;
    }
    
    private IEnumerator StartDialogue2()
    {
        dialogueCanvas.enabled = true;
        _shooterController.disableShooting = true;
        
        _dialogueController.StartDialogue(dialogueAsset2);
        yield return new WaitUntil(() => _dialogueController.IsFinished);
        dialogueCanvas.enabled = false;
        _shooterController.disableShooting = false;
        
    }
    

    private void Phase1()
    {
        // horse
        for (var i = 2; i < 10; i++)
        {
            GameEventManager.Instance.ScheduleEvent(i, SpawnHorse);
            GameEventManager.Instance.ScheduleEvent(i + i * 0.1f, SpawnSpread);
        }

    }

    private void Phase2()
    {
        
        // fairy
        for (var j = 15; j < 30; j++)
        {
            GameEventManager.Instance.ScheduleEvent(j - 0.5f, SpawnFairy1);
            GameEventManager.Instance.ScheduleEvent(j + j * 0.3f, SpawnSpiral);
        }

        // fairy
        for (var i = 31; i < 40; i++)
        {
            GameEventManager.Instance.ScheduleEvent(i - 0.5f, SpawnFairy2);
            GameEventManager.Instance.ScheduleEvent(i + i * 0.1f, SpawnSpread);
        }
        
    }
    
}
