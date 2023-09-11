using System;
using Script.Presenter;
using UnityEngine;
using UnityEngine.UI;
using UniRx;

namespace Script.View
{
    public class SendLetterButton : MonoBehaviour
    {
  
        [SerializeField] private MainARScenePresenter _mainARScenePresenter;
        [SerializeField] private GameObject localizePanel;
        [SerializeField] private GameObject androidPanel;

        private void Start()
        {
            var button = GetComponent<Button>();
            button.onClick.AsObservable()
                .Subscribe(_ =>
                {
                    androidPanel.gameObject.SetActive(false);
                    localizePanel.gameObject.SetActive(true); 
                }).AddTo(this);
            
          
        }
    }
}
