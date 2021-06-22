using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace QuantizedLoopStation
{

    public class LoopNote
    {
        public int instrumentID;
        public int noteID;
        public bool isPlayed;
        public LoopNote(int _instrumentID, int _noteID)
        {
            instrumentID = _instrumentID;
            noteID = _noteID;
            isPlayed = false;
        }

    }

    public class LoopStation : MonoBehaviour
    {
        public enum LoopState { Standby, Waiting, Recording, Playing, Stopped }
        private LoopState _loopState = LoopState.Standby;
        public LoopState loopState
        {
            get
            {
                return _loopState;
            }
            set
            {
                _loopState = value;
                OnLoopStateChanged(_loopState);
            }

        }

        private void OnLoopStateChanged(LoopState loopState)
        {
            if (loopState == LoopState.Standby)
            {
                text_LoopState.text = "StandBy";
            }
            else if (loopState == LoopState.Waiting)
            {
                text_LoopState.text = "Waiting";
            }
            else if (loopState == LoopState.Recording)
            {
                text_LoopState.text = "Recording";
            }
            else if (loopState == LoopState.Playing)
            {
                text_LoopState.text = "Playing";
            }
            else if (loopState == LoopState.Stopped)
            {
                text_LoopState.text = "Stopped";
            }
        }

        private const float loopLength = 1.0f;

        private float timer;
        private float loopTime;


        private Dictionary<int, LoopNote> loopNotes = new Dictionary<int, LoopNote>();


        [SerializeField] private Text text_LoopState;
        [SerializeField] private Key keyPrefab;
        [SerializeField] private Transform content_keys;
        private AudioSource audioSource;
        private Library library;
        private void Start()
        {
            audioSource = GetComponent<AudioSource>();
            library = Library.instance;

            InstantiateKeys();
        }

        private void InstantiateKeys()
        {
            for (int i = 0; i < library.audioClips.Length; i++)
            {
                Key key = Instantiate<Key>(keyPrefab, content_keys);
                key.Init(0, i, audioSource);
            }
        }

        private void OnEnable()
        {
            GlobalMetronome.subTic += OnSubTic;
        }

        private void OnDisable()
        {
            GlobalMetronome.subTic -= OnSubTic;
        }


        private void FixedUpdate()
        {
            if (loopState == LoopState.Recording)
            {
                timer += Time.fixedDeltaTime;

                if (timer >= loopLength * GlobalMetronome.instance.BeatInvertal * GlobalMetronome.instance.NumberBeatInBar)
                {
                    loopState = LoopState.Playing;
                    timer = 0;
                }
            }
        }

        public void AddLoopNote(int _quantizedRawBeat, int _instrumentID, int _noteID)
        {
            if (loopState == LoopState.Waiting)
            {
                loopState = LoopState.Recording;
            }

            int divideFactor = GlobalMetronome.instance.NumberBeatInBar * GlobalMetronome.instance.QuantizeDegree;
            int localBeat = _quantizedRawBeat % divideFactor;
            loopNotes.Add(localBeat, new LoopNote(_instrumentID, _noteID));
        }

        public bool IsNoteAddable()
        {
            if (loopState == LoopState.Waiting || loopState == LoopState.Recording)
            {
                return true;
            }
            return false;
        }


        private void OnTic(int beat)
        {

        }

        private void OnSubTic(int rawSubBeat)
        {
            if (loopState != LoopState.Playing) return;

            int divideFactor = GlobalMetronome.instance.NumberBeatInBar * GlobalMetronome.instance.QuantizeDegree;
            int local = rawSubBeat % divideFactor;

            if (loopNotes.TryGetValue(local, out LoopNote loopNote))
            {
                audioSource.PlayOneShot(library.audioClips[loopNote.noteID]);
            }

        }


        #region UnityEvent
        public void MainButtonEvent()
        {
            if (loopState == LoopState.Standby)
            {
                loopState = LoopState.Waiting;
            }
            else if (loopState == LoopState.Waiting)
            {
                loopState = LoopState.Standby;
            }
            else if (loopState == LoopState.Recording)
            {
                loopState = LoopState.Playing;
            }
            else if (loopState == LoopState.Playing)
            {
                loopState = LoopState.Stopped;
            }
            else if (loopState == LoopState.Stopped)
            {
                loopState = LoopState.Playing;
            }
        }
        #endregion
    }

}
