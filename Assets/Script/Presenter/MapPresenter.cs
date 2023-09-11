using System;
using LitJson;
using Niantic.ARDK.LocationService;
using Niantic.ARDK.VPSCoverage;
using Script.Model;
using Script.Presenter.Interface;
using UniRx;
using UnityEngine;
using RuntimeEnvironment = Niantic.ARDK.RuntimeEnvironment;
using System.Runtime.InteropServices;
using Script.Model.Interface;


namespace Script.Presenter
{
    public class MapPresenter : MonoBehaviour, IMapPresentable
    {
        [DllImport("__Internal")]
        private static extern void delete_map_view();
        private IWayspotRepository _wayspotRepository;
        public IObservable<string> geoJsonObservable => geoJsonSubject;
        private Subject<string> geoJsonSubject = new Subject<string>();
      
        public IObservable<bool> _reloadImageHiddenObservable => _reloadButtonHiddenSubject;
        private Subject<bool> _reloadButtonHiddenSubject = new Subject<bool>();

        public void Start()
        {
            var locationService = LocationServiceFactory.Create(RuntimeEnvironment.Default);
            locationService.Start();
            _wayspotRepository = new WayspotRepository(
                CoverageClientFactory.Create(),
                locationService);  
        }

        public async void OnTapReloadButton(string message)
        {
            var areas = await _wayspotRepository.GetCoverageAreas(2000);
            if (CheckAreaResult(areas))
            {
                var areaList = await _wayspotRepository.GetAreaList(areas); 
                var json = JsonMapper.ToJson(areaList);
                Debug.Log(json + "OnTapReloadButton");
                geoJsonSubject.OnNext(json);
            }
        }
        
        public void OnTapDismissButton(string message)
        {
#if UNITY_IOS
            Debug.Log("OnTapDismissButton" + message);
            delete_map_view(); 
#endif
        }
        private bool CheckAreaResult(CoverageAreasResult areasResult)
        {
            if (areasResult.Status != ResponseStatus.Success) 
            {
                Debug.LogWarning("CoverageAreas request failed with status: " + areasResult.Status);
                return false;
            }

            if (areasResult.Areas.Length == 0)
            {
                Debug.LogWarning("CoverageAreas request succeeded, but no areas were found.");
                return false;
            }
            return true;
        } 
    }
}
