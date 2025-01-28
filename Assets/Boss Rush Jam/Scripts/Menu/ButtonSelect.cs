using Battle.Ability;
using Menu.Radial;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Menu.RadialMenuSelect
{
    public class ButtonSelect : MonoBehaviour
    {
        [SerializeField] private AttackAbility ability;
        [SerializeField] private Sprite abilitySprite;
        private Image abilityImage;

        public static event Action<AttackAbility> OnAttack;
        //public static event Action<AttackAbility> OnAttackAnimation;

        private void Start ()
        {
            //abilityImage.sprite = abilitySprite;
        }

        private void OnEnable()
        {
            RadialMenu.OnUpdateButton += UpdateButton;
        }

        private void OnDisable()
        {
            RadialMenu.OnUpdateButton -= UpdateButton;
        }

        private void UpdateButton(AttackAbility newAbility)
        {
            if (ability == newAbility) return;

            ability = newAbility;
            //currentAbility = newAbility;
            abilitySprite = newAbility.abilityIcon;

            if (abilityImage == null)
            {
                abilityImage = GetComponent<Image>();
            }

            if (abilityImage != null)
            {
                abilityImage.sprite = abilitySprite;
            }
        }

        private void UseAbility()
        {
            //Debug.Log($"Selected abilities name is {ability.abilityName}, this can do between {ability.minDamage} - {ability.maxDamage} of damage");
            OnAttack?.Invoke(ability);
        }
    }
}

