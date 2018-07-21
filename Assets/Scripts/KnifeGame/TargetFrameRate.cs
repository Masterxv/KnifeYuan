using UnityEngine;

namespace KnifeGame
{
    public class TargetFrameRate : MonoBehaviour
    {
        private void Awake()
        {
            if (Application.isMobilePlatform)
                Application.targetFrameRate = 30;
            else
                Application.targetFrameRate = 60;
        }
    }
}