using UnityEngine;
using System.Collections; // 為了使用Coroutine

public class MoveObject : MonoBehaviour
{
    public Transform objectToMove; // 需要移動的物件
    public Vector3 targetPositionOffset; // 相對於起始位置的目標距離
    public float moveDuration = 1f; // 移動所需時間（秒）
    public float delayBeforeMove = 2f; // 移動前的延遲時間（秒）

    private Vector3 startPosition; // 起始位置
    private Vector3 targetPosition; // 目標位置
    private float elapsedTime = 0f; // 經過的時間
    private bool isMoving = false; // 是否在移動

    void Start()
    {
        // 初始化起始和目標位置
        startPosition = objectToMove.position;
        targetPosition = startPosition + targetPositionOffset;

        // 在啟動時開始計時，並在延遲後開始移動
        StartCoroutine(StartMovingWithDelay());
    }

    void Update()
    {
        if (isMoving)
        {
            elapsedTime += Time.deltaTime;
            float progress = elapsedTime / moveDuration;

            // 線性插值移動物體
            objectToMove.position = Vector3.Lerp(startPosition, targetPosition, progress);

            // 如果移動完成，停止移動
            if (progress >= 1f)
            {
                isMoving = false;
                elapsedTime = 0f; // 重置時間以便重複移動
            }
        }
    }

    // 協程：在延遲後開始移動物件
    IEnumerator StartMovingWithDelay()
    {
        // 等待設定的延遲時間
        yield return new WaitForSeconds(delayBeforeMove);
        
        // 開始移動物件
        StartMoving();
    }

    // 開始移動物件
    public void StartMoving()
    {
        if (!isMoving)
        {
            startPosition = objectToMove.position; // 重新設置起始位置
            targetPosition = startPosition + targetPositionOffset; // 重新計算目標位置
            elapsedTime = 0f; // 重置時間
            isMoving = true; // 啟動移動
        }
    }
}