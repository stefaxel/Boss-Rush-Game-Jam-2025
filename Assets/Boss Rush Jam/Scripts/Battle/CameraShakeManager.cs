using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Battle.CameraShake
{
    public class CameraShakeManager : MonoBehaviour
    {
        public static CameraShakeManager instance;

        [SerializeField] private float globalShakeForce;

        private void Awake()
        {
            if(instance == null)
            {
                instance = this;
            }
        }

        public void CameraShake(CinemachineImpulseSource impulseSource)
        {
            impulseSource.GenerateImpulseWithForce(globalShakeForce);
        }
    }
}

