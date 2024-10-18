using UnityEngine;
using System.Collections; // 為了使用Coroutine

public class MoveObject : MonoBehaviour
{
    public Transform objectToMove; 
    public Vector3 targetPositionOffset; 
    public float moveDuration = 1f; 
    public float delayBeforeMove = 2f; 

    private Vector3 startPosition; 
    private Vector3 targetPosition;
    private float elapsedTime = 0f; 
    private bool isMoving = false; 

    void Start()
    {
        
        startPosition = objectToMove.localPosition;
        targetPosition = startPosition + targetPositionOffset;
        elapsedTime = 0f; 
        StartCoroutine(StartMovingWithDelay());
    }

    void Update()
    {
        if (isMoving)
        {
            elapsedTime += Time.deltaTime;
            float progress = elapsedTime / moveDuration;

            
            objectToMove.localPosition = Vector3.Lerp(startPosition, targetPosition, progress);

            
            if (progress >= 1f)
            {
                isMoving = false;
                elapsedTime = 0f; 
            }
        }
    }

    
    IEnumerator StartMovingWithDelay()
    {
        
        yield return new WaitForSeconds(delayBeforeMove);
  
        isMoving = true; 
    }

    
}