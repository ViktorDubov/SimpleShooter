using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;

namespace Scripts.Core.Extensions
{
    public static class GOExtensions
    {
        public static async UniTask MoveGameObject(this GameObject gameObject, Vector3 startPosition, Vector3 finishPosition, float velocity)
        {
            bool isFinish = false;
            Vector3 moveVector = (finishPosition - startPosition).normalized;
            float distance = Vector3.Distance(finishPosition, startPosition);
            while (!isFinish && gameObject != null)
            {
                gameObject.transform.Translate(moveVector * velocity * Time.deltaTime, Space.World);
                if (Vector3.Distance(gameObject.transform.position, startPosition) >= distance)
                {
                    gameObject.transform.position = finishPosition;
                    isFinish = true;
                }
                await UniTask.Yield();
            }
        }
    }
}
