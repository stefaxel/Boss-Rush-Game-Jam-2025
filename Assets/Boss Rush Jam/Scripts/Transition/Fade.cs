using Battle.Handler;
using Battle.Interact;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Transitions.FadeBlack
{
    public class Fade : MonoBehaviour
    {
        [SerializeField] private Animator fadeAnimator;
        [SerializeField] private float fadeDuration;

        public static event Action OnSetupBattle;
        public static event Action OnReturnToWorld;

        private void OnEnable()
        {
            BattleInteract.OnInteract += PlayTransition;
            BattleHandler.OnBattleEnd += ReturnToWorld;
        }

        private void OnDisable()
        {
            BattleInteract.OnInteract -= PlayTransition;
            BattleHandler.OnBattleEnd -= ReturnToWorld;
        }

        private void PlayTransition()
        {
            StartCoroutine(LoadBattle());
        }

        private void ReturnToWorld()
        {
            StartCoroutine(BattleWon());
        }

        IEnumerator LoadBattle()
        {
            fadeAnimator.SetTrigger("Start");

            yield return new WaitForSeconds(fadeDuration);

            OnSetupBattle?.Invoke();

            fadeAnimator.SetTrigger("End");

            yield return new WaitForSeconds(fadeDuration);
        }

        IEnumerator BattleWon()
        {
            fadeAnimator.SetTrigger("Start");

            yield return new WaitForSeconds(fadeDuration);

            OnReturnToWorld?.Invoke();

            fadeAnimator.SetTrigger("End");

            yield return new WaitForSeconds(fadeDuration);
        }
    }
}

