using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UniRx;
using UniRx.Triggers;

using Cysharp.Threading.Tasks;

namespace Scripts.Core.Inputs
{
    public class MouseInput : MonoBehaviour
    {
        private IObservable<Vector2> _rotationObservable;
        public IObservable<Vector2> RotationObservable
        {
            get
            {
                if (_rotationObservable == null)
                {
                    _rotationObservable = GetRotationObservable();
                }
                return _rotationObservable;
            }
        }
        private IObservable<Vector2> GetRotationObservable()
        {
            return this.FixedUpdateAsObservable()
                .Select
                (
                _ =>
                {
                    float x = Input.GetAxis("Mouse Y");
                    float z = Input.GetAxis("Mouse X");
                    return new Vector2(x, z).normalized;
                }
                );
        }

        private IObservable<bool> _fireObservable;
        public IObservable<bool> FireObservable
        {
            get
            {
                if (_fireObservable == null)
                {
                    _fireObservable = GetFireObservable();
                }
                return _fireObservable;
            }
        }
        private IObservable<bool> GetFireObservable()
        {
            return this.UpdateAsObservable()
                .Select
                (
                _ =>
                {
                    bool isDown = Input.GetAxis("Fire1") > 0 ? true : false;
                    return isDown;
                }
                );
        }
    }
}
