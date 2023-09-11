using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.UI;

namespace Script.View
{
    public class LetterFormView : MonoBehaviour
    {
        [DllImport("__Internal")]
        private static extern void present_letter();
        [SerializeField] private InputField inputField;
        [SerializeField] private GameObject localizePanel;
    
        public void OnTapLetterButton()
        {
#if UNITY_IOS
            Debug.Log("OnTapLetterButton" + "iOS");
            present_letter(); 
#endif
        }
    }
}