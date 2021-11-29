using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Scripts.Core.Level
{
    public class LevelController : MonoBehaviour
    {
        [SerializeField] private GameObject[] _enemies;
        public GameObject[] Enemies
        {
            get
            {
                return _enemies;
            }
        }
    }
}
