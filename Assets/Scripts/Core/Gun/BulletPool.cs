using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Scripts.Core.Gun
{
    public class BulletPool : MonoBehaviour
    {
        
        [SerializeField] private GameObject _bulletPrefab;
        [SerializeField] private int _startbulletCount = 5;
        private static List<GameObject> _bulletPool;

        private static BulletPool _instance;
        public static BulletPool Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindObjectOfType<BulletPool>();
                    if (_instance == null)
                    {
                        throw new ArgumentNullException("there is no active GO with BulletPool");
                    }
                }
                return _instance;
            }
        }
        private void Awake()
        {
            _bulletPool = new List<GameObject>();
            for (int i = 0; i < _startbulletCount; i++)
            {
                _bulletPool.Add(Instantiate(_bulletPrefab, this.transform));
                _bulletPool[i].transform.localPosition = Vector3.zero;
                _bulletPool[i].transform.localRotation = Quaternion.Euler(Vector3.zero);
                _bulletPool[i].SetActive(false);
            }
        }
        public GameObject GetNextBullet()
        {
            if (_bulletPool[0].activeSelf)
            {
                _bulletPool.Insert(0, (Instantiate(_bulletPrefab, this.transform)));
                _bulletPool[0].transform.localPosition = Vector3.zero;
                _bulletPool[0].transform.localRotation = Quaternion.Euler(Vector3.zero);
                _bulletPool[0].SetActive(false);
            }
            GameObject nextBullet = _bulletPool[0];
            _bulletPool.RemoveAt(0);
            _bulletPool.Add(nextBullet);
            nextBullet.SetActive(true);
            return nextBullet;
        }
    }
}
