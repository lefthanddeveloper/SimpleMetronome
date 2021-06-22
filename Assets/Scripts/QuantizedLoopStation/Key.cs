using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace QuantizedLoopStation
{
    public class Key : MonoBehaviour, IPointerDownHandler
    {

        AudioSource audioSource;
        int instrumentID;
        int noteID;

        public void Init(int _instrumentID, int _noteID, AudioSource _audioSource)
        {
            instrumentID = _instrumentID;
            noteID = _noteID;
            audioSource = _audioSource;

            GetComponent<Image>().color = Library.instance.colors[_noteID];

        }

        public void OnPointerDown(PointerEventData eventData)
        {
                audioSource.PlayOneShot(Library.instance.audioClips[noteID]);
                GlobalMetronome.instance.AddLoopNote(0, noteID);
        }

        private void Update() {
            if(noteID != 0) return;

            if(Input.GetKeyDown(KeyCode.Space))
            {
                audioSource.PlayOneShot(Library.instance.audioClips[noteID]);
                GlobalMetronome.instance.AddLoopNote(0, noteID);
            }

        }
    }

}
