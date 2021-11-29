using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UniRx;

namespace Scripts.Core
{
    public interface IHealth
    {
        public FloatReactiveProperty Health { get; }
        public IObservable<bool> IsDead { get; }
        public void GetDamage(float damage, Vector3 bulletVector);
        public void DeathLogic();
    }
}
