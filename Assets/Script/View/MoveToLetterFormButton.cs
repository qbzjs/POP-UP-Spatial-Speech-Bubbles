using UnityEngine;
using UniRx;
using System.Runtime.InteropServices;
using Script.Presenter;

namespace Script.View
{
    public class MoveToLetterFormButton : MonoBehaviour
    {
        [SerializeField] private MainARScenePresenter _mainARScenePresenter;
        [DllImport("__Internal")]
        private static extern void present_letter();
        private void Start()
        {
            _mainARScenePresenter.sendLetterButtonHiddenObservable
                .Subscribe(isActive =>
                {
                    gameObject.SetActive(isActive);
                }).AddTo(this);
            
            
        } 
        
        public void OnTapMoveLetterFormButton()
        {
#if UNITY_IOS
            Debug.Log("OnTapLetterButton" + "iOS");
            present_letter(); 
#endif
        
#if UNITY_ANDROID
        Debug.Log("OnTapLetterButton" + "Android");
        
#endif
        }
    }
}