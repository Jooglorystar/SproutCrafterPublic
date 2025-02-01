using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UILog : MonoBehaviour
{
    public TextMeshProUGUI logText; // UI Text 컴포넌트를 연결
    private Queue<string> logQueue = new Queue<string>(); // 로그 메시지 큐
    private int maxLogLines = 5; // 최대 표시 줄 수
    private float logDisplayDuration = 5f; // 로그 유지 시간
    private Coroutine removeLogCoroutine;

    public void AddLog(string message)
    {
        if(!gameObject.activeInHierarchy) return;
        
        // 로그 메시지를 큐에 추가
        logQueue.Enqueue(message);

        // 최대 줄 수를 초과하면 가장 오래된 메시지를 제거
        while (logQueue.Count > maxLogLines)
        {
            logQueue.Dequeue();
        }

        // UI 갱신
        UpdateLogText();

        // 기존 코루틴 중지 후 새 코루틴 시작
        if (removeLogCoroutine != null)
        {
            StopCoroutine(removeLogCoroutine);
        }
        removeLogCoroutine = StartCoroutine(RemoveLogAfterDelay());
    }

    private void UpdateLogText()
    {
        logText.text = string.Join("\n", logQueue.ToArray());
    }

    private IEnumerator RemoveLogAfterDelay()
    {
        // 지정된 시간 동안 대기
        yield return new WaitForSeconds(logDisplayDuration);

        // 큐를 완전히 비움
        logQueue.Clear();
        UpdateLogText();
    }
}