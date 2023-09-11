using System.Runtime.InteropServices;
using Script.Presenter;
using UniRx;
using UnityEngine;

namespace Script.View
{
    public class MapView : MonoBehaviour
    {
        [DllImport("__Internal")]
        private static extern void receive_map_info(string geoJson);
        [DllImport("__Internal")]
        private static extern void present_map_view();
    
#if UNITY_ANDROID
    private const string PackageName = "com.example.mylibrary";
    private const string ClassName = "UnityInterface";
    private AndroidJavaObject _pluginInstance;
#endif

        [SerializeField]
        private MapPresenter _mapPresenter;
        private void Start()
        {
#if UNITY_ANDROID
       _pluginInstance = new AndroidJavaObject($"{PackageName}.{ClassName}");
#endif

            _mapPresenter.geoJsonObservable
                .Subscribe(jsonString =>
                {
                    PresentMapView(jsonString); 
                }).AddTo(this);
        }
    
        public void PresentMapView(string geoJson)
        {
#if UNITY_ANDROID
        Debug.Log("PresentMapView" + geoJson);
        _pluginInstance.Call("presentMapView", geoJson); 
#endif

#if UNITY_IOS
            Debug.Log("PresentMapView" + geoJson);
            receive_map_info(geoJson);
#endif
        }
    
        public void OnTapMapButton()
        {
#if UNITY_IOS
            Debug.Log("OnTapMapButton" + "iOS");
            present_map_view();
#endif
        
#if UNITY_ANDROID
        Debug.Log("OnTapMapButton" + "Android");
        _pluginInstance.Call("presentMapView"); 
#endif
        }


        public void CallBackMapAndroid(string message)
        {
#if UNITY_ANDROID
        Debug.Log("CallBackMapAndroid" + message);
        _pluginInstance.Call("deleteMapView");
#endif
        } 
    
        private void OnDestroy()
        {
#if UNITY_ANDROID
       _pluginInstance.Dispose();
       _pluginInstance = null;
#endif
        }
    }
}
