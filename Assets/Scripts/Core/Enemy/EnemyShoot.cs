using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
using System;
using Cysharp.Threading.Tasks;

using Scripts.Core.Gun;

namespace Scripts.Core.Enemy
{
    public class EnemyShoot : MonoBehaviour
    {

        [SerializeField] [Range(0.1f, 2)] float _timeStepShooting = 3f;
        [SerializeField] [Range(0, 100)] float _damage = 10f;
        [SerializeField] [Range(0, 30)] float _speed = 10f;
        [SerializeField] [Range(0, 0.2f)] float _randomDeviation = 0.05f;

        private bool _isStartShoot = false;
        private CancellationTokenSource _cts;
        private GunController _gunController;

        public void Awake()
        {
            _gunController = GetComponentInChildren<GunController>();
        }

        public void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent<PlayerController>(out PlayerController player) && !_isStartShoot)
            {
                _isStartShoot = true;
                _cts = new CancellationTokenSource();
                Shoot(other.gameObject).Forget();
            }
        }
        public void OnTriggerExit(Collider other)
        {
            if (other.TryGetComponent<PlayerController>(out PlayerController player))
            {
                _cts.Cancel();
                _isStartShoot = false;
            }
        }
        public void OnDestroy()
        {
            if (_cts != null)
            {
                _cts.Dispose();
            }
        }
        public async UniTask Shoot(GameObject player)
        {
            while (!_cts.IsCancellationRequested)
            {
                if (_gunController!=null)
                {
                    _gunController.Shoot(_damage, player.transform.position - transform.position, _speed, _randomDeviation);
                }

                await UniTask.Delay((int)(_timeStepShooting * 1000));
            }
            _isStartShoot = false;
        }
    }
}
