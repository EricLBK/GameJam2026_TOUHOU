using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;

public class GameEventManager : MonoBehaviour
{
    public static GameEventManager Instance { get; private set; }

    [Serializable]
    public class TimedEvent
    {
        public float triggerTime;
        public Func<IEnumerator> MethodToRun; 

        public TimedEvent(float time, Func<IEnumerator> method)
        {
            triggerTime = time;
            MethodToRun = method;
        } 
    }
    private List<TimedEvent> _eventQueue = new List<TimedEvent>();
    private float _timer;
    private bool _isPaused;
    private bool _isRunning;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }

        Instance = this;
    }

    // Update is called once per frame
    private void Update()
    {
        if (_isPaused || !_isRunning) return;

        _timer += Time.deltaTime;

        if (_eventQueue.Count > 0 && _timer >= _eventQueue[0].triggerTime)
        {
            StartCoroutine(ProcessEvent(_eventQueue[0]));
        }

    }

    public void ScheduleEvent(float time, Func<IEnumerator> method)
    {
        _eventQueue.Add(new TimedEvent(time, method));
        
        _eventQueue = _eventQueue.OrderBy(e => e.triggerTime).ToList();
    }

    public void BeginTimeline()
    {
        _timer = 0f;
        _isRunning = true;
        _isPaused = false;
        Debug.Log("Timeline started");
    }
    
    private IEnumerator ProcessEvent(TimedEvent gameEvent)
    {
        _isPaused = true;
        
        _eventQueue.RemoveAt(0);

        Debug.Log($"Event Triggered at {gameEvent.triggerTime}s. Pausing timer...");

        if (gameEvent.MethodToRun != null)
        {
            yield return StartCoroutine(gameEvent.MethodToRun());
        }

        Debug.Log("Event Finished. Resuming timer.");
        _isPaused = false;
    }
    
    public void JumpToTime(float newTime)
    {
        if (!(newTime > _timer)) return;
        _timer = newTime;
        Debug.Log($"Timer forwarded to {_timer}");
    }
    
    public void SkipToNextEvent()
    {
        if (_eventQueue.Count <= 0) return;
        var nextTime = _eventQueue[0].triggerTime;
        JumpToTime(nextTime);
    }
}
