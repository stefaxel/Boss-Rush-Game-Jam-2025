using Battle;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Transitions
{
    public class Fade : MonoBehaviour
    {
        [SerializeField] private Animator fadeAnimator;
        [SerializeField] private float fadeDuration;

        public static event Action OnSetupBattle;

        private void OnEnable()
        {
            BattleInteract.OnInteract += PlayTransition;
        }

        private void OnDisable()
        {
            BattleInteract.OnInteract -= PlayTransition;
        }

        private void PlayTransition()
        {
            StartCoroutine(LoadBattle());
        }

        IEnumerator LoadBattle()
        {
            fadeAnimator.SetTrigger("Start");

            yield return new WaitForSeconds(fadeDuration);

            OnSetupBattle?.Invoke();

            fadeAnimator.SetTrigger("End");

            yield return new WaitForSeconds(fadeDuration);
        }
    }
}

