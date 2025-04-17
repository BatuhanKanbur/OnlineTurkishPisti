using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using static Structures.Utilities;

namespace Structures
{
    public class AutoLayout : MonoBehaviour
    {
        public void AlignChildren()
        {
            Transform parentTransform = transform;
            int childCount = parentTransform.childCount;
            if(childCount<2) return;
            for (int i = 0; i < childCount; i++)
            {
                Transform child = parentTransform.GetChild(i);
                Vector3 childPosition = new Vector3(
                    Remap(i, 0, childCount - 1, -Constants.Offset, Constants.Offset),
                    i == 0 || i == childCount -1 ? 3f : 3.5f, 0);
                Vector3 childRotation = new Vector3(0, 0, Remap(i, 0, childCount-1, Constants.MaxAngle, -Constants.MaxAngle));
                StartCoroutine(SetTransform(child, childPosition, childRotation));
            }
        }

        IEnumerator SetTransform(Transform child, Vector3 childPosition, Vector3 childRotation, float duration=0.5f)
        {
            float elapsedTime = 0f;
            Vector3 startingAngles = child.localEulerAngles;
            Vector3 startingPosition = child.localPosition;
            while (elapsedTime < duration)
            {
                child.localPosition = Vector3.Lerp(startingPosition, childPosition, elapsedTime / duration);
                child.eulerAngles = Vector3.Lerp(startingAngles, childRotation, elapsedTime / duration);
                elapsedTime += Time.deltaTime;
                yield return null;
            }
            child.localPosition = childPosition;
            child.localEulerAngles = childRotation;
        }  
    }

}