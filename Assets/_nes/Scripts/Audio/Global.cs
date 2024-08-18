using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


namespace Audio
{
    public class Global : MonoBehaviour
    {
        public static Global instance;

        [Header("Audio")]

        public float bpm = 120f;

        private double time;

        private double nextEventTime;

        private bool globalPlay = false;

        public UnityEvent<int> OnEveryStep;

        //About Instruments
        List<InstGroup> instGroups = new List<InstGroup>();
        public float instrumentUIHeight = 108.33f;
        List<Creature> creatures = new List<Creature>();

        [Header("Gameplay")]
        public bool mergeCreatures = false;

        [Header("Objects")]
        public GameObject moveAnchor;

        [HideInInspector]
        public int beatIndex = 0;

        public delegate void OnEveryStepDelegate(int x);

        public event OnEveryStepDelegate OnEveryStepEvent;

        public float creatureUnifyDistance = 0.2f;


        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void Start()
        {
            //find all instGroups, nut not necessary because the InstGroups will add themselves to the list
            // InstGroup[] instGroups = FindObjectsOfType<InstGroup>();
            // foreach (InstGroup instGroup in instGroups)
            // {
            //     this.instGroups.Add(instGroup);
            // }

            globalPlay = true;
            nextEventTime = AudioSettings.dspTime + 0.4;
        }

        public void AddInstGroup(InstGroup instGroup)
        {
            if (!instGroups.Contains(instGroup))
            {
                instGroups.Add(instGroup);
            }
        }

        public void RemoveInstGroup(InstGroup instGroup)
        {
            if (instGroups.Contains(instGroup))
            {
                instGroups.Remove(instGroup);
            }
        }

        public void AddCreature(Creature creature)
        {
            if (!creatures.Contains(creature))
            {
                creatures.Add(creature);
            }
        }

        public void RemoveCreature(Creature creature)
        {
            if (creatures.Contains(creature))
            {
                creatures.Remove(creature);
            }
        }

        public List<Creature> GetCreatures()
        {
            return creatures;
        }

        private void Reset()
        {
            if (OnEveryStep == null)
            {
                OnEveryStep = new UnityEvent<int>();
            }
        }

        public void Update()
        {
            time = AudioSettings.dspTime;
            double sixteenthNote = 60.0 / (bpm * 4);
            if (globalPlay)
            {
                if (time + sixteenthNote > nextEventTime)
                {
                    foreach (InstGroup instGroup in instGroups)
                    {
                        instGroup.Schedule(beatIndex, nextEventTime);
                    }
                    nextEventTime += sixteenthNote;
                    beatIndex++;
                    TimeEvent te = new TimeEvent(nextEventTime, EveryStepAction, beatIndex);
                    StartCoroutine(CheckEventRoutine(te));
                }
            }
        }

        private int lateUpdateCount = 0;

        private void LateUpdate()
        {
            if (!globalPlay) //occasionally update stuff when globalPlay is false
            {
                if (lateUpdateCount % 64 == 0)
                {
                    EveryStepAction(AudioSettings.dspTime, beatIndex);
                }
                lateUpdateCount++;
            }
        }

        private IEnumerator CheckEventRoutine(TimeEvent timeEvent)
        {
            yield return new WaitUntil(() => AudioSettings.dspTime >= timeEvent.time);
            timeEvent.callback(timeEvent.time, timeEvent.beatIndex);
        }

        public void EveryStepAction(double time, int x)
        {
            if (time < AudioSettings.dspTime - 0.1) //if the time is too far in the past it's too late to run the action
            {
                return;
            }
            OnEveryStep?.Invoke(x);
            OnEveryStepEvent?.Invoke(x);
        }

        public void Play()
        {
            beatIndex = -1;

            nextEventTime = AudioSettings.dspTime;
            globalPlay = true;
        }

        public void Stop()
        {
            globalPlay = false;
        }
    }
}