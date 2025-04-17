using System.Text;
using UnityEngine;

namespace Structures
{
    public static class Utilities
    {
        public static float Remap(float from, float fromMin, float fromMax, float toMin, float toMax)
        {
            var fromAbs = from - fromMin;
            var fromMaxAbs = fromMax - fromMin;

            var normal = fromAbs / fromMaxAbs;

            var toMaxAbs = toMax - toMin;
            var toAbs = toMaxAbs * normal;

            var to = toAbs + toMin;

            return to;
        }

        public static void DebugPlayerDeck(this int[] array,string playerName)
        {
            StringBuilder newStringBuilder = new StringBuilder();
            newStringBuilder.Append(playerName + " : ");
            foreach (var val in array)
            {
                newStringBuilder.Append(val + " , ");
            }
            Debug.Log(newStringBuilder.ToString());
        }
    }
    public abstract class StaticInstance<T> : MonoBehaviour where T : MonoBehaviour 
    {
        public static T Instance { get; private set; }
        protected virtual void Awake() => Instance = this as T;
        protected virtual void OnApplicationQuit() 
        {
            Instance = null;
            Destroy(gameObject);
        }
    }
    public abstract class Singleton<T> : StaticInstance<T> where T : MonoBehaviour 
    {
        protected override void Awake() 
        {
            if (Instance != null)
            {
                Destroy(gameObject);
            }
            else
            {
                base.Awake();    
            }
        }
    }
    public abstract class PersistentSingleton<T> : Singleton<T> where T : MonoBehaviour
    {
        protected override void Awake() 
        {
            base.Awake();
            DontDestroyOnLoad(gameObject);
        }
    }
}