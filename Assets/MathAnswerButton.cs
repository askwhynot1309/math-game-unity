using UnityEngine;

public class MathAnswerButton : MonoBehaviour
{
    public int answerValue;
    public MathGameManager gameManager;
    public AstraInputController inputController;

    private bool isFootOver = false;
    private bool hasClicked = false;

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
        if (isFootOver && !hasClicked)
        {
            hasClicked = true;
            gameManager.SelectAnswer(answerValue);
        }
    }

    private void OnMouseDown()
    {
        gameManager.SelectAnswer(answerValue);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Foot"))
        {
            isFootOver = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Foot"))
        {
            isFootOver = false;
            hasClicked = false;
        }
    }
}
