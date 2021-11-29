using System;
using UnityEngine;

using UnityEngine.UI;

namespace Scripts.Core.UI
{
    public class Promt : MonoBehaviour
    {
        [SerializeField] private Text _promt;
        private static Promt _instance;
        public static Promt Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindObjectOfType<Promt>();
                    if (_instance == null)
                    {
                        throw new ArgumentNullException("there is no active GO with Promt");
                    }
                }
                return _instance;
            }
        }
        public static void PrintMessage(PromptType promptType)
        {
            switch (promptType)
            {
                case PromptType.None:
                    Instance._promt.text = "";
                    break;
                case PromptType.Win:
                    Instance._promt.text = "You win";
                    break;
                case PromptType.Lose:
                    Instance._promt.text = "You lose";
                    break;
                case PromptType.DontKill:
                    Instance._promt.text = "You don't kill all enemies";
                    break;
                default:
                    break;
            }
        }
    }

    public enum PromptType
    {
        None,
        Win,
        Lose,
        DontKill
    }
}
