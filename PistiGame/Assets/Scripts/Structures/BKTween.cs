using System;
using System.Collections;
using UnityEngine;

namespace Structures
{
    public static class BKTween
    {
        public static void SetAnchoredPositionAndRotation(this Transform transform, Vector2 targetPosition, Vector3 targetRotation, float duration = 1f, Action onComplete = null)
        {
            var rect = transform.GetComponent<RectTransform>();
            if (rect == null)
            {
                Debug.LogWarning($"SetAnchoredPositionAndRotation failed: {transform.name} does not have RectTransform.");
                return;
            }

            TweenManager.Instance.StartCoroutine(MoveAnchoredCoroutine(rect, targetPosition, duration));
            TweenManager.Instance.StartCoroutine(RotateCoroutine(rect, targetRotation, duration, onComplete));
        }
        public static void MoveAnchored(this Transform transform, Vector2 targetPosition, float duration = 1f, Action onComplete = null)
        {
            var rect = transform.GetComponent<RectTransform>();
            if (rect == null)
            {
                Debug.LogWarning($"MoveAnchored failed: {transform.name} does not have RectTransform.");
                return;
            }

            TweenManager.Instance.StartCoroutine(MoveAnchoredCoroutine(rect, targetPosition, duration, onComplete));
        }
        public static void Move(this Transform transform, Transform target, float duration = 1f, Action onComplete = null)
        {
            TweenManager.Instance.StartCoroutine(MoveCoroutine(transform, target, duration, onComplete));
        }
        public static void Rotate(this Transform transform, Vector3 targetRotation, float duration = 1f, Action onComplete = null)
        {
            var rect = transform.GetComponent<RectTransform>();
            if (rect == null)
            {
                Debug.LogWarning($"Rotate failed: {transform.name} does not have RectTransform.");
                return;
            }

            TweenManager.Instance.StartCoroutine(RotateCoroutine(rect, targetRotation, duration, onComplete));
        }
        private static IEnumerator MoveCoroutine(Transform transform, Transform target, float duration, Action onComplete = null)
        {
            float elapsed = 0f;
            Vector2 start = transform.position;
            while (elapsed < duration)
            {
                transform.position = Vector3.Lerp(start, target.position, elapsed / duration);
                elapsed += Time.deltaTime;
                yield return null;
            }
            transform.position = target.position;
            onComplete?.Invoke();
        }
        private static IEnumerator MoveAnchoredCoroutine(RectTransform rectTransform, Vector2 targetPosition, float duration, Action onComplete = null)
        {
            float elapsed = 0f;
            Vector2 start = rectTransform.anchoredPosition;

            while (elapsed < duration)
            {
                rectTransform.anchoredPosition = Vector2.Lerp(start, targetPosition, elapsed / duration);
                elapsed += Time.deltaTime;
                yield return null;
            }

            rectTransform.anchoredPosition = targetPosition;
            onComplete?.Invoke();
        }
        private static IEnumerator RotateCoroutine(RectTransform rectTransform, Vector3 targetAngles, float duration, Action onComplete = null)
        {
            float elapsed = 0f;
            Vector3 startAngles = rectTransform.localEulerAngles;

            while (elapsed < duration)
            {
                rectTransform.localEulerAngles = Vector3.Lerp(startAngles, targetAngles, elapsed / duration);
                elapsed += Time.deltaTime;
                yield return null;
            }

            rectTransform.localEulerAngles = targetAngles;
            onComplete?.Invoke();
        }
    }
}
