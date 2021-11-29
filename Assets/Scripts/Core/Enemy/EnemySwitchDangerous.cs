using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UniRx;

using Scripts.Core.Inputs;

namespace Scripts.Core.Enemy
{
    public class EnemySwitchDangerous : MonoBehaviour
    {
        [SerializeField] Material _beCalm;
        [SerializeField] Material _underAttack;

        public void Start()
        {
            MeshRenderer meshRenderer = GetComponentInChildren<MeshRenderer>();
            CameraCentralGO.Instance.CentralGoObservable
                .Subscribe
                (
                go
                =>
                {
                    if (go == this.gameObject)
                    {
                        meshRenderer.material = _underAttack;
                    }
                    else
                    {
                        meshRenderer.material = _beCalm;
                    }
                }
                )
                .AddTo(this);
        }
    }
}
