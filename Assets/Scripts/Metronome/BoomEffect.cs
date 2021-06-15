using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SimpleMetronome
{
    public class BoomEffect : MonoBehaviour
    {
        Vector3 originalScale;

        public bool onFirstCount;
        public bool onSecondCount;
        public bool onThirdCount;
        public bool onFourthCount;

        private bool isAnimating = false;
        private void OnEnable()
        {
            Metronome.tic += OnTick;
        }

        private void OnDisable()
        {
            Metronome.tic -= OnTick;
        }

        private void Start()
        {
            originalScale = transform.localScale;
        }

        void OnTick(int beatCounter)
        {

            if (IsDefault())
            {
                Animate();
            }
            else
            {
                int remainder = beatCounter % 4;
                if (onFirstCount && remainder == 0)
                {
                    Animate();
                }
                if (onSecondCount && remainder == 1)
                {
                    Animate();
                }
                if (onThirdCount && remainder == 2)
                {
                    Animate();
                }
                if (onFourthCount && remainder == 3)
                {
                    Animate();
                }
            }
        }

        bool IsDefault()
        {
            if (!onFirstCount && !onSecondCount && !onThirdCount && !onFourthCount)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        void Animate()
        {
            if (!isAnimating)
            {
                StartCoroutine(BoomScale());
            }
        }

        private float scalingFactor = 0.05f;
        IEnumerator BoomScale()
        {
            isAnimating = true;
            float destX = originalScale.x * 2.0f;
            while (transform.localScale.x <= destX)
            {
                transform.localScale += Vector3.one * scalingFactor;
                yield return null;
            }

            while (transform.localScale.x >= originalScale.x)
            {
                transform.localScale -= Vector3.one * scalingFactor;
                yield return null;
            }

            transform.localScale = originalScale;
            isAnimating = false;
        }
    }

}