using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MenuError : MonoBehaviour
{
    [SerializeField]private GameObject errorMessageOverlay;
    [SerializeField] private Button continueButton;
    [SerializeField] private TMP_Text text;
    
    [SerializeField] private float messagedisplayTime = 4f;
    private Coroutine displayMessageCo;

    private void Start()
    {
        if (continueButton!= null)
            continueButton.onClick.AddListener(ContinueButtonPressed);
        else
            Debug.Log("Button Not referenced");
    }

    private void OnDisable()
    {
        if (continueButton != null)
            continueButton.onClick.RemoveListener(ContinueButtonPressed);        
    }

    public void DisplayErrorMessage(string errorCode)
    {
        text.text = errorCode;

        displayMessageCo = StartCoroutine("DisplayErrorMesageCoroutine");

    }

    private IEnumerator DisplayErrorMesageCoroutine()
    {
        errorMessageOverlay.SetActive(true);
        yield return new WaitForSeconds(messagedisplayTime);
        errorMessageOverlay.SetActive(false);
    }

    private void ContinueButtonPressed()
    {
        StopCoroutine(displayMessageCo);
        errorMessageOverlay.SetActive(false);
    }
}
