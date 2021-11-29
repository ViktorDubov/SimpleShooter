using System.Collections;
using System.Collections.Generic;
using System;
using UniRx;
using UnityEngine;
using Cysharp.Threading.Tasks;

namespace Scripts.Core
{
    [RequireComponent(typeof(Rigidbody))]
    public class PlayerStats : MonoBehaviour, IHealth
    {
        [SerializeField] private float _maxHealth = 100;
        [SerializeField] private float _recoilCoefficient = 1;
        private CompositeDisposable _disposables;
        private Rigidbody _rg;

        private FloatReactiveProperty _health;
        public FloatReactiveProperty Health 
        { 
            get
            {
                if (_health==null)
                {
                    _health = new FloatReactiveProperty(_maxHealth);
                }
                return _health;
            }
        }
        private IObservable<bool> _isDead;
        public IObservable<bool> IsDead
        {
            get
            {
                if (_isDead == null)
                {
                    _isDead = Health.Select(hp => { return hp <= 0; });
                }
                return _isDead;
            }
        }
        public void Awake()
        {
            _disposables = new CompositeDisposable();
            _rg = GetComponent<Rigidbody>();
        }
        public void Start()
        {

            IsDead
                .Where(isDead => { return isDead; })
                .Subscribe(_ => DeathLogic())
                .AddTo(_disposables);
        }
        public void OnDisable()
        {
            _disposables.Clear();
        }
        public void OnDestroy()
        {
            _disposables.Dispose();
        }
        public void DeathLogic()
        {
            
            DeathLogicUT().Forget();

        }
        private async UniTask DeathLogicUT()
        {
            
            await UniTask.Yield();
            _health.Value = _maxHealth;

        }
        public void GetDamage(float damage, Vector3 bulletVector)
        {
            Health.Value -= damage;
            BulletRecoil(bulletVector);
        }
        public void BulletRecoil(Vector3 bulletVector) //???????????????
        {
            _rg.AddForce(_recoilCoefficient * (new Vector3(bulletVector.x, 0, bulletVector.z)), ForceMode.Impulse);
        }
        public void ResetHelth()
        {
            Health.Value = _maxHealth;
        }
    }
}
