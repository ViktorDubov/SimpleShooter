using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UniRx;
using UniRx.Triggers;

using Scripts.Core.Enemy;

namespace Scripts.Core.Inputs
{
    public class CameraCentralGO : MonoBehaviour
    {
        private IObservable<GameObject> _centralGO;
        public IObservable<GameObject> CentralGoObservable
        {
            get
            {
                if (_centralGO==null)
                {
                    _centralGO = GetCentralGO();
                }
                return _centralGO;
            }
        }
        private IObservable<GameObject> GetCentralGO()
        {
            return this.UpdateAsObservable()
                .Select
                (
                _ =>
                {
                    Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5F, 0.5F, 0));
                    GameObject centralGO = null;
                    RaycastHit[] hits;
                    hits = Physics.RaycastAll(ray, 20.0F);
                    foreach (RaycastHit hit in hits)
                    {
                        bool _isTrigger = hit.collider.isTrigger;
                        if (!_isTrigger)
                        {
                            if (hit.collider.gameObject.TryGetComponent<EnemySwitchDangerous>(out EnemySwitchDangerous enemy))
                            {
                                centralGO = hit.collider.gameObject;
                            }
                        }
                    }
                    return centralGO;
                }
                );
        }
        private static CameraCentralGO _instance;
        public static CameraCentralGO Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindObjectOfType<CameraCentralGO>();
                    if (_instance == null)
                    {
                        throw new ArgumentNullException("there is no active GO with CameraCentralGO");
                    }
                }
                return _instance;
            }
        }
    }
}
