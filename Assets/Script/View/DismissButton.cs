using System;
using UnityEngine;

namespace Script.View
{
    public class DismissButton : MonoBehaviour
    {

        public Action ClickDismissButton;
        public void OnTapDismissButton()
        {
            Debug.Log("OnTapDismissButton" + "Android");
            ClickDismissButton?.Invoke();
        }
    } 
}

