using System.Threading.Tasks;
using Niantic.ARDK.VPSCoverage;

namespace Script.Model.Interface
{
    public interface IWayspotRepository
    {
        public Task<LocalizationTarget> GetNearestPlace();
        public Task<LocalizationTargetsResult> GetLocalizationTargetsResult(string[] targetIds);
        public Task<AreaList> GetAreaList(CoverageAreasResult areasResult);
        public Task<CoverageAreasResult> GetCoverageAreas(int queryRadius);
    }
}