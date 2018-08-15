using UnityEngine;

namespace KnifeGame
{
    public class InputTouch : MonoBehaviour
    {
        public delegate void OnTouchDown(Vector3 pos);

        public static event OnTouchDown onTouchDown;

        public bool blockInput = false;

        private void Update()
        {
            if (blockInput)
            {
                return;
            }

            if (Application.isMobilePlatform)
            {
                var nbTouches = Input.touchCount;

                if (nbTouches > 0)
                {
                    for (var i = 0; i < nbTouches; i++)
                    {
                        var touch = Input.GetTouch(i);
                        var phase = touch.phase;

                        if (phase != TouchPhase.Began || onTouchDown == null) continue;
                        onTouchDown(touch.position);
                        break;
                    }
                }
            }

            if (!Application.isMobilePlatform)
            {
                if (Input.GetMouseButtonDown(0)) 
                {
//                    onTouchDown(Input.mousePosition);
                    onTouchDown?.Invoke(Input.mousePosition);
                }
            }
        }
    }
}