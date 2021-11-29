using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;
using Cysharp.Threading.Tasks;
using System;
using System.Threading;

using Scripts.Core;

namespace Scripts.Core.Gun
{
    public class BulletController : MonoBehaviour
    {
        private float _damage = 10f;
        private float _speed = 30f;
        private Vector3 _direction = Vector3.zero;
        private float _randomDeviation = 0.1f;
        private float _lifeTimeSeconds = 3;

        private CancellationTokenSource _cts;
        private VisualEffect _vfx;
        private GameObject _vfxGO;
        private GameObject _visualBulletGO;

        public void Awake()
        {
            _vfx = GetComponentInChildren<VisualEffect>();
            _vfxGO = _vfx.gameObject;
            _vfxGO.SetActive(false);

            _visualBulletGO = GetComponentInChildren<MeshRenderer>().gameObject;
            _visualBulletGO.SetActive(true);
        }
        public void OnDisable()
        {
            if (_cts != null)
            {
                _cts.Cancel();
            }
        }
        public void OnDestroy()
        {
            if (_cts != null)
            {
                _cts.Dispose();
            }
        }
        public void InstantiateBullet(float damage, float speed, Vector3 direction, float randomDeviation, float lifeTimeSeconds)
        {
            _damage = damage;
            _speed = speed;
            direction.Normalize();
            _direction = new Vector3(
                direction.x + UnityEngine.Random.Range(-1 * randomDeviation, randomDeviation),
                direction.y + UnityEngine.Random.Range(-1 * randomDeviation, randomDeviation),
                direction.z).normalized;
            _lifeTimeSeconds = lifeTimeSeconds;

            _visualBulletGO.SetActive(true);
        }
        public void DoShoot()
        {
            _cts = new CancellationTokenSource();
            Move().Forget();
            LifeCicle().Forget();
        }
        private async UniTask Move()
        {
            while (!_cts.IsCancellationRequested)
            {
                RaycastHit hit;
                if (this.gameObject == null)
                {
                    continue;
                }
                if (Physics.Raycast(transform.position, _direction, out hit, _speed * Time.deltaTime + 0.1f))
                {
                    if (hit.collider.isTrigger)
                    {
                        transform.Translate(_direction * _speed * Time.deltaTime, Space.World);
                    }
                    else
                    {
                        if (hit.collider.TryGetComponent<IHealth>(out IHealth health))
                        {
                            health.GetDamage(_damage, _direction);
                        }
                        _visualBulletGO.SetActive(false);
                        await DoEffect();
                        await EndMove();
                    }
                }
                else
                {
                    transform.Translate(_direction * _speed * Time.deltaTime, Space.World);
                }
                await UniTask.Yield();
            }
        }
        private async UniTask DoEffect()
        {
            _vfxGO.SetActive(true);
            await UniTask.Delay(1000);
            _vfxGO.SetActive(false);

        }
        private async UniTask LifeCicle()
        {
            await UniTask.Delay((int)(_lifeTimeSeconds * 1000));
            await EndMove();
        }
        private async UniTask EndMove()
        {
            _cts.Cancel();
            await UniTask.Yield();
            this.gameObject.SetActive(false);
        }
    }
}
