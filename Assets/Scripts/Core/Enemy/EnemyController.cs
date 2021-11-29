using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;

using System;
using UniRx;
using Cysharp.Threading.Tasks;

using Scripts.Core;
using Scripts.Core.Extensions;

namespace Scripts.Core.Enemy
{
    public class EnemyController : MonoBehaviour
    {
        [SerializeField] [Range(1, 10)] float _velocity = 1f;
        [SerializeField] [Range(1, 10)] int _maxTimeForMove = 3;

        private ReactiveProperty<MoveState> _moveStateRP;
        private CancellationTokenSource _cts;
        private MoveState[] _moveStates;
        private CompositeDisposable _disposables;
        private bool _isStartMove = false;

        public void Awake()
        {
            _moveStates = (MoveState[])Enum.GetValues(typeof(MoveState));
            _moveStateRP = new ReactiveProperty<MoveState>(MoveState.None);
            _moveStateRP.Value = MoveState.None;
            _disposables = new CompositeDisposable();
        }
        public void OnEnable()
        {
            _moveStateRP
                .Where(moveState => { return moveState != MoveState.None; })
                .Subscribe(moveState => MoveLogic(moveState))
                .AddTo(_disposables);
        }
        public void OnDisable()
        {
            _disposables.Clear();
        }
        public void OnDestroy()
        {
            _disposables.Dispose();
            if (_cts != null)
            {
                _cts.Dispose();
            }
        }
        public void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent<PlayerController>(out PlayerController player) && !_isStartMove)
            {
                _isStartMove = true;
                _moveStateRP.Value = MoveState.None;
                _cts = new CancellationTokenSource();
                SetMoveState().Forget();
            }
        }
        public void OnTriggerExit(Collider other)
        {
            if (other.TryGetComponent<PlayerController>(out PlayerController player))
            {
                _moveStateRP.Value = MoveState.None;
                _cts.Cancel();
            }
        }
        private async UniTask SetMoveState()
        {
            while (!_cts.IsCancellationRequested)
            {
                _moveStateRP.Value = RandomMoveStateValue();
                await UniTask.Delay(UnityEngine.Random.Range(1 * 1000, _maxTimeForMove * 1000));
            }
            _isStartMove = false;
        }
        MoveState RandomMoveStateValue()
        {
            return _moveStates[UnityEngine.Random.Range(0,_moveStates.Length)];
        }
        private void MoveLogic(MoveState moveState)
        {
            Vector3 moveVector = Vector3.zero;
            switch (moveState)
            {
                case MoveState.Right:
                    moveVector = transform.right;
                    break;
                case MoveState.Left:
                    moveVector = -1 * transform.right;
                    break;
            }
            RaycastHit hit, hitDown;
            moveVector *= UnityEngine.Random.Range(1, 4);
            float distance = moveVector.magnitude;
            Vector3 position = this.gameObject.transform.position;
            if (!Physics.Raycast(position, moveVector, out hit, distance)
                &&
                Physics.Raycast(position + moveVector, Vector3.down, out hitDown, 2 * distance))
            {
                this.gameObject.MoveGameObject(position, position + moveVector, _velocity).Forget();
            }
        }
    }

    public enum MoveState
    {
        Right,
        Left,
        None
    }
}
