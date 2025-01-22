using Battle.Ability;
using Menu.Radial;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Playables;
using UnityEngine;

namespace Menu.Ability
{
    public class ReplaceAbility : MonoBehaviour
    {
        [SerializeField] AttackAbility currentAbility;
        
        public void OnReplaceAbilityButtonClicked(AttackAbility newAbility)
        {
            RadialMenu radialMenu = GetComponentInParent<RadialMenu>();
            radialMenu.ReplaceAbility(currentAbility, newAbility);
        }

    }
}

