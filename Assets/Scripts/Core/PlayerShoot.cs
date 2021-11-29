using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UniRx;

using Scripts.Core.Inputs;
using Scripts.Core.Gun;

namespace Scripts.Core
{
    public class PlayerShoot : MonoBehaviour
    {
        [SerializeField] [Range(0.1f, 2)] float _timeStepShooting = 1f;
        [SerializeField] [Range(0, 100)] float _damage = 10f;
        [SerializeField] [Range(0, 30)] float _speed = 10f;
        [SerializeField] [Range(0, 0.2f)] float _randomDeviation = 0.05f;

        float _curentTime = 0f;
        private CompositeDisposable _disposables;
        private GunController _gunController;
        private Camera _camera;
        public void Awake()
        {
            _disposables = new CompositeDisposable();
            _gunController = GetComponentInChildren<GunController>();
            _camera = GetComponentInChildren<Camera>();
            if (_camera == null)
            {
                throw new ArgumentNullException("Add camera to child gameobject");
            }
        }
        public void OnEnable()
        {
            GeneralInput.Instance.FireObservable
                .Do(_ => { _curentTime += Time.deltaTime; })
                .Where(b => { return b; })
                .Subscribe(_ => ShootLogic())
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
        private void ShootLogic()
        {
            if (_curentTime > _timeStepShooting)
            {
                _gunController.Shoot(_damage, _camera.transform.forward, _speed, _randomDeviation);
                _curentTime = 0;
            }
        }
    }
}
