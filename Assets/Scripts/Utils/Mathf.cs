using UnityEngine;

namespace Utils
{
    public class Mathf
    {
        public static float Vector2Angle(Vector2 vector2)
        {
            return Vector2Angle(vector2.x, vector2.y);
        }

        public static float Vector2Angle(float x, float y)
        {
            if (x < 0.0f)
            {
                return 360.0f - (UnityEngine.Mathf.Atan2(x, y) * UnityEngine.Mathf.Rad2Deg * -1);
            }

            return UnityEngine.Mathf.Atan2(x, y) * UnityEngine.Mathf.Rad2Deg;
        }
    }
}
