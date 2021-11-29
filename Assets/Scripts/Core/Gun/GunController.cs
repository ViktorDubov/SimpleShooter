using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Scripts.Core.Gun
{
    public class GunController : MonoBehaviour
    {
        private Transform _startTransform;
        public void Awake()
        {
            _startTransform = GetComponentInChildren<StartPointForBullet>().transform;
        }
        public void Shoot(float damage, Vector3 direction, float speed = 10f, float randomDeviation = 0.05f, float lifeTimeSeconds = 10f)
        {
            GameObject bullet = BulletPool.Instance.GetNextBullet();
            bullet.transform.position = _startTransform.position;
            bullet.transform.rotation = _startTransform.rotation;

            BulletController bulletController = bullet.GetComponent<BulletController>();
            bulletController.InstantiateBullet(damage, speed, direction, randomDeviation, lifeTimeSeconds);
            bulletController.DoShoot();
        }
    }
}
