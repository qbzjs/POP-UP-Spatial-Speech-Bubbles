using Script.Presenter;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace Script.View
{
    public class LoadingImageCircle : MonoBehaviour
    {
        [SerializeField] Image circleLoadigImage;
        [SerializeField] private MapPresenter _mapPresenter;

        private void Start()
        {
            _mapPresenter._reloadImageHiddenObservable
                .Subscribe(isActive =>
                {
                    gameObject.SetActive(isActive);
                    enabled = isActive;  
                }).AddTo(this);
            
            
        }

        private void Update()
        {
            if (circleLoadigImage.enabled)
            {
                if (circleLoadigImage.fillAmount >= 1f) circleLoadigImage.fillAmount = 0f;
                circleLoadigImage.fillAmount += 0.05f; 
            }   
        }
    }
}
