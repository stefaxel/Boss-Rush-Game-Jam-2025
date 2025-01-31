using Battle.Ability;
using Menu.RadialMenuSelect;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationController : MonoBehaviour
{
    public static AnimationController instance;

    [SerializeField] private Transform particlePosition;
    private GameObject currentParticleEffect;

    private AttackAbility currentAttack;
    private Animator anim;
    private AudioSource audioSource;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        anim = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
    }

    private void OnEnable()
    {
        ButtonSelect.OnAttack += PlayAttackAnimation;
    }

    private void OnDisable()
    {
        ButtonSelect.OnAttack -= PlayAttackAnimation;
    }

    private void PlayAttackAnimation(AttackAbility abilityAnimation)
    {
        //StopAllCoroutines();
        currentAttack = abilityAnimation;

        Debug.Log($"Event received from ButtonSelect, playing the animation with trigger {abilityAnimation.animationTrigger}");

        anim.SetBool(abilityAnimation.animationTrigger, true);

        //StartCoroutine(ReturnToIdle(abilityAnimation));
        currentParticleEffect = abilityAnimation.particlePrefab;

        if (abilityAnimation.attackAudioClip != null)
        {
            Debug.Log("Playing attack for player");
            audioSource.PlayOneShot(abilityAnimation.attackAudioClip);
        }
    }

    public float GetAnimationLength(string animationTrigger)
    {
        AnimatorStateInfo stateInfo = anim.GetCurrentAnimatorStateInfo(0);
        return stateInfo.length;
    }

    public void SpawnParticleEffect()
    {
        if(currentParticleEffect != null)
        {
            Debug.Log("Spawning particle effect");
            Instantiate(currentParticleEffect, particlePosition);

            //Destroy(particle, particle.GetComponent<ParticleSystem>().main.duration);
        }
    }

    private void ReturnToIdle()
    {
        //yield return new WaitForSeconds(anim.GetCurrentAnimatorStateInfo(0).length);
        anim.SetBool(currentAttack.animationTrigger, false);
    }
}
