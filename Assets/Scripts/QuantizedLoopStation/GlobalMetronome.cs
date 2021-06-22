using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace QuantizedLoopStation
{
    public class GlobalMetronome : MonoBehaviour
    {
        public delegate void GlobalMetronomeEvent(int count);
        public static event GlobalMetronomeEvent tic;
        public static event GlobalMetronomeEvent subTic;
        public static GlobalMetronome instance;

        [SerializeField] private float bpm = 100.0f;
        private float beatInvertal;
        public float BeatInvertal => beatInvertal;
        private float beatTimer;
        private int beatCount;

        private int numberBeatInBar = 4;
        public int NumberBeatInBar => numberBeatInBar;
        private int quantizeDegree = 4;
        public int QuantizeDegree => 4;


        private float subBeatTimer;
        private int subBeatCount;


        private AudioSource audioSource;
        public AudioClip[] metronomeClip;

        [SerializeField] private LoopStation curLoopStation;
        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
            }
            else
            {
                if (instance != this)
                {
                    Destroy(this.gameObject);
                }
            }

            QualitySettings.vSyncCount = 1;
            Application.targetFrameRate = -1;
        }

        private void OnEnable()
        {
            tic += OnTic;
            subTic += OnSubTic;
        }

        private void OnDisable()
        {
            tic -= OnTic;
            subTic -= OnSubTic;
        }

        void Start()
        {
            audioSource = GetComponent<AudioSource>();
        }

        private void FixedUpdate()
        {
            beatInvertal = 60f / bpm;

            // beatTimer += Time.fixedDeltaTime;
            // if (beatTimer >= beatInvertal)
            // {
            //     beatTimer -= beatInvertal;
            //     beatCount++;
            //     tic(beatCount);
            // }

            subBeatTimer += Time.fixedDeltaTime;
            if (subBeatTimer >= beatInvertal / quantizeDegree)
            {
                subBeatTimer -= beatInvertal / quantizeDegree;
                subBeatCount++;

                if (subBeatCount % 4 == 0)
                {
                    beatCount++;
                    tic(beatCount);
                }

                subTic(subBeatCount);
            }


        }

        float deltaTime;
        private void Update()
        {
            deltaTime += (Time.unscaledDeltaTime - deltaTime) * 0.1f;
            float fps = 1.0f / deltaTime;

            Debug.Log(fps);
        }

        public int GetCurrentQuantizedSubBeat()
        {
            if (subBeatTimer <= beatInvertal / quantizeDegree / 2f)
            {
                return subBeatCount;
            }
            else
            {
                return subBeatCount + 1;
            }
        }

        public void AddLoopNote(int _instrumentID, int _noteID)
        {
            if (!curLoopStation.IsNoteAddable()) return;
            curLoopStation.AddLoopNote(GetCurrentQuantizedSubBeat(), _instrumentID, _noteID);
        }

        private void OnTic(int beat)
        {
            if (beat % 4 == 0)
            {
                audioSource.PlayOneShot(metronomeClip[0], 1.0f);
            }
            else
            {
                audioSource.PlayOneShot(metronomeClip[0], 1.0f);
            }
        }

        private void OnSubTic(int subBeat)
        {
            // audioSource.PlayOneShot(metronomeClip[0], 0.05f);
        }
    }
}


