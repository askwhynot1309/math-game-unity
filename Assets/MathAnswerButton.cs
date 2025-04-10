using UnityEngine;

public class MathAnswerButton : MonoBehaviour
{
    public int answerValue;
    public MathGameManager gameManager;
    public AstraInputController inputController;

    private bool isHandOver = false;

    private void Start()
    {
        if (inputController == null)
        {
            inputController = GetComponent<AstraInputController>();
        }

        if (inputController != null)
        {
            inputController.OnClickEvent.AddListener(HandleClick);
        }
    }

    private void HandleClick()
    {
        if (isHandOver)
        {
            Debug.Log("Activate");
            gameManager.SelectAnswer(answerValue);
        }
    }

    private void OnMouseDown()
    {
        gameManager.SelectAnswer(answerValue);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Hand"))
        {
            Debug.Log("Hand entered button");
            isHandOver = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Hand"))
        {
            Debug.Log("Hand exited button");
            isHandOver = false;
        }
    }
}
