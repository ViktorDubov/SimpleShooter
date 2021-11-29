using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UniRx;


namespace Scripts.Core.Enemy
{
    public class EnemyStats : MonoBehaviour, IHealth
    {
        [SerializeField] private float _maxHealth = 100;
        private CompositeDisposable _disposables;

        private FloatReactiveProperty _health;
        public FloatReactiveProperty Health
        {
            get
            {
                if (_health == null)
                {
                    _health = new FloatReactiveProperty(_maxHealth);
                }
                return _health;
            }
            set
            {
                _health.Value = value.Value;
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
        }

        public void OnEnable()
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
            
            Destroy(this.gameObject);

        }
        public void GetDamage(float damage, Vector3 bulletVector)
        {
            Health.Value -= damage;
        }
    }
}
