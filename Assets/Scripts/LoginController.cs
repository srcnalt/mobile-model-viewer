using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;

public class LoginController : MonoBehaviour {
    public InputField nameField;
    public InputField passwordField;
    public Text errorText;
    public UIController uIController;

	public void CheckCredentials()
    {
        if (nameField.text == "")
            errorText.text = "Name field is empty!";
        else if (passwordField.text == "")
            errorText.text = "Password field is empty!";
        else
        {
            uIController.SwitchPanel(uIController.TopPanel);
            uIController.TopPanel.GetComponentsInChildren<Text>()[0].text = nameField.text;

            nameField.text = "";
            passwordField.text = "";
            errorText.text = "";

            uIController.ActivateMainPanel(uIController.ModelSelectPanel);
            uIController.LoadResources();
            uIController.CheckRepository();
        }
    }

    public void CleanErrorMessage()
    {
        errorText.text = "";
    }

    public void HidePassword()
    {
        string str = "";

        for (int i = 0; i < passwordField.text.Length; i++)
        {
            str += "*";
        }

        passwordField.text = str;
    }
}
