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
    private List<TimedEvent> eventQueue = new List<TimedEvent>();
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

        // Advance timer
        _timer += Time.deltaTime;

        // Check if we have events left and if the timer has passed the next event's trigger time
        if (eventQueue.Count > 0 && _timer >= eventQueue[0].triggerTime)
        {
            StartCoroutine(ProcessEvent(eventQueue[0]));
        }

    }

    public void ScheduleEvent(float time, Func<IEnumerator> method)
    {
        eventQueue.Add(new TimedEvent(time, method));
        
        // Sort the events by time so we always look at the earliest one first
        eventQueue = eventQueue.OrderBy(e => e.triggerTime).ToList();
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
        
        eventQueue.RemoveAt(0);

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
        // Only jump forward, never backward
        if (!(newTime > _timer)) return;
        _timer = newTime;
        Debug.Log($"Timer forwarded to {_timer}");
    }
    
    public void SkipToNextEvent()
    {
        if (eventQueue.Count <= 0) return;
        var nextTime = eventQueue[0].triggerTime;
        JumpToTime(nextTime);
    }
}
