using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Threading;
using UniRx;
using UniRx.Triggers;

using Cysharp.Threading.Tasks;

using Scripts.Core.Level;
using Scripts.Core.UI;

namespace Scripts.Core
{
    public class GameManager : MonoBehaviour
    {
        [SerializeField] private GameObject _player;
        [SerializeField] private GameObject _levelPrefab;

        private PlayerStats _playerStats;
        private GameObject _actualLevel;
        private LevelController _levelController;
        private GameObject _startPoint;
        private GameObject _finishTrigger;
        private IObservable<float> _positionY;
        private IObservable<bool> _isDead;
        private IObservable<bool> _isEnterFinishTrigger;
        private CompositeDisposable _disposables;

        public void Awake()
        {
            _playerStats = _player.GetComponent<PlayerStats>();
            _isDead = _playerStats.IsDead;
            _positionY = Observable.EveryUpdate()
                .Select(_ => { return _player.transform.position.y; });

            _disposables = new CompositeDisposable();
        }
        public void Start()
        {
            LoadLevel();
        }
        private void OnDestroy()
        {
            _disposables.Dispose();
        }
        private void LoadLevel()
        {
            if (_actualLevel!=null)
            {
                Destroy(_actualLevel);
            }
            _actualLevel = Instantiate(_levelPrefab, Vector3.zero, Quaternion.Euler(Vector3.zero));
            Transform startTransform = _actualLevel.GetComponentInChildren<StartPoint>().transform;
            _player.transform.position = startTransform.position;
            _player.transform.rotation = startTransform.rotation;

            _levelController = _actualLevel.GetComponent<LevelController>();

            Collider colliderFinishTrigger = _actualLevel.GetComponentInChildren<Finish>().GetComponent<Collider>();
            _isEnterFinishTrigger = _player.OnTriggerEnterAsObservable()
                .Select(collider => { return collider == colliderFinishTrigger; });

            ReSubscribeAllObservable();
        }
        private void ReSubscribeAllObservable()
        {
            _disposables.Clear();

            _positionY
                .Where(y => { return y < -5; })
                .Subscribe(_ => LoseLogic().Forget())
                .AddTo(_disposables);
            _isDead
                .Where(isDead => { return isDead; })
                .Subscribe(_ => LoseLogic().Forget())
                .AddTo(_disposables);
            _isEnterFinishTrigger
                .Where(isEnter => { return isEnter; })
                .Subscribe(_ => WinLogic().Forget())
                .AddTo(_disposables);

        }
        private async UniTask CheckKills()
        {
            GameObject[] enemies = _levelController.Enemies;
            bool isWin = true;
            foreach (GameObject go in enemies)
            {
                if (go != null)
                {
                    Promt.PrintMessage(PromptType.DontKill);
                    await UniTask.Delay(1000);
                    LoseLogic().Forget();
                    isWin = false;
                }
            }
            if (isWin)
            {
                WinLogic().Forget();
            }
        }
        private async UniTask WinLogic()
        {
            Promt.PrintMessage(PromptType.Win);
            await UniTask.Delay(1000);
            Promt.PrintMessage(PromptType.None);
            _playerStats.ResetHelth();
            LoadLevel();
        }
        private async UniTask LoseLogic()
        {
            Promt.PrintMessage(PromptType.Lose);
            await UniTask.Delay(1000);
            Promt.PrintMessage(PromptType.None);
            _playerStats.ResetHelth();
            LoadLevel();
        }
    }
}
