using UnityEngine;
using System.Collections;

public class EnemyAnimation : MonoBehaviour
{
    [SerializeField] float _jumpHeight = 0.5f;
    [SerializeField] float _jumpDuration = 0.2f;

    public void AnimateJump(Transform target)
    {
        StopAllCoroutines();
        StartCoroutine(JumpCycle(target.position));
    }

    IEnumerator JumpCycle(Vector3 targetPos)
    {
        Vector3 originalPos = transform.position;
        yield return StartCoroutine(JumpRoutine(originalPos, targetPos));
        yield return StartCoroutine(SpringEffect(targetPos));
        yield return StartCoroutine(JumpRoutine(targetPos, originalPos));
        yield return StartCoroutine(SpringEffect(originalPos));
    }

    IEnumerator JumpRoutine(Vector3 from, Vector3 to)
    {
        float t = 0f;

        while (t < 1f)
        {
            t += Time.deltaTime / _jumpDuration;
            float height = Mathf.Sin(t * Mathf.PI) * _jumpHeight;
            Vector3 pos = Vector3.Lerp(from, to, t);
            pos.y += height;
            transform.position = pos;
            yield return null;
        }
    }

    IEnumerator SpringEffect(Vector3 basePos)
    {
        float springDuration = 0.1f;
        float t = 0f;

        Vector3 originalScale = transform.localScale;
        Vector3 squashedScale = new(originalScale.x * 1.3f, originalScale.y * 0.6f, originalScale.z); // widen and squash

        while (t < 1f)
        {
            t += Time.deltaTime / springDuration;
            float eased = Mathf.Sin(t * Mathf.PI); // smooth in/out
            transform.localScale = Vector3.Lerp(originalScale, squashedScale, eased);
            transform.position = basePos; // ensure stable landing
            yield return null;
        }

        transform.localScale = originalScale;
    }
}
