using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LitJson;
using Niantic.ARDK.LocationService;
using Niantic.ARDK.VPSCoverage;
using Script.Model.Interface;
using UnityEngine;
using LocalizationTargetsResult = Niantic.ARDK.VPSCoverage.LocalizationTargetsResult;

namespace Script.Model
{
    public class WayspotRepository: IWayspotRepository 
    {
        private ICoverageClient _coverageClient;
        private ILocationService _locationService;
        public WayspotRepository(
            ICoverageClient coverageClient, 
            ILocationService locationService)
        {
            this._locationService = locationService;
            this._coverageClient = coverageClient;
        }

        public async Task<CoverageAreasResult> GetCoverageAreas(int queryRadius)
        {
            Debug.Log("GetCoverageAreas");
            // 現在の場所を取得
            var requestLocation = new LatLng(_locationService.LastData);
            // カバレッジAPIを叩く
            var coverageAreasResult = await _coverageClient.RequestCoverageAreasAsync(requestLocation, queryRadius);
            return coverageAreasResult;
        }
    
        public async Task<AreaList> GetAreaList(CoverageAreasResult areasResult)
        {
            Debug.Log("SendAreaResult");
        
            List<Task<LocalizationTargetsResult>> taskResults = new List<Task<LocalizationTargetsResult>>();
            List<List<string>> targetIdsList = new List<List<string>>(); 
            List<AreaList.LanLon> lanLons = new List<AreaList.LanLon>();
            foreach (var area in areasResult.Areas)
            {
                var targetIds = area.LocalizationTargetIdentifiers;
                targetIdsList.Add(targetIds.ToList());
                var task = _coverageClient.RequestLocalizationTargetsAsync(targetIds);
                taskResults.Add(task);
                var lanlon = MakeLanLon(area);
                lanLons.Add(lanlon);
            }

            var result = await Task.WhenAll(taskResults.ToArray()).ContinueWith(results =>
            {
                for (int i = 0; i < results.Result.Length; i++)
                {
                    var target = targetIdsList[i];
                    var result = results.Result[i];
                    var title = GetAreaTitle(result, target.ToArray());
                    lanLons[i].SetTitle(title);
                }
                var areaList = new AreaList(lanLons);
                return areaList;
            });
            return result;
        }

        public async Task<LocalizationTargetsResult> GetLocalizationTargetsResult(string[] targetIds)
        {
            var result = await _coverageClient.RequestLocalizationTargetsAsync(targetIds);
            return result;
        }

        private string GetAreaTitle(
            LocalizationTargetsResult targetsResult,
            string[] targetIds)
        {
            foreach (string targetId in targetIds)
            {
                LocalizationTarget target = targetsResult.ActivationTargets[targetId];

                string title = target.Name;
                return title;
            }
            return "";
        }
    
        private AreaList.LanLon MakeLanLon(CoverageArea area)
        {
            var root = JsonMapper.ToObject<Root>(area.ToGeoJson());
            var rootFeature = root.features.FirstOrDefault();
            var feature = rootFeature.geometry.coordinates.FirstOrDefault();
            var point = feature.FirstOrDefault();
            float lat = (float)point[1];
            float lon = (float)point[0]; 
            var lanLon = new AreaList.LanLon(lat,lon);
            return lanLon;
        }
    
        private List<string> MakeTargetIdsList(CoverageAreasResult areasResult)
        {
            List<string> targetIdsList = new List<string>();
            foreach (var area in areasResult.Areas)
            {
                var targetIds = area.LocalizationTargetIdentifiers;
                targetIdsList.AddRange(targetIds);
            }
            return targetIdsList;
        }

        public async Task<LocalizationTarget> GetNearestPlace()
        {
            var coverageAreaList = await GetCoverageAreas(10);
            var targetIds = MakeTargetIdsList(coverageAreaList).ToArray();
            var targetResult = await GetLocalizationTargetsResult(targetIds);
            LocalizationTarget firstTarget = targetResult.ActivationTargets.FirstOrDefault().Value;
            return firstTarget;
        }
    }
}
