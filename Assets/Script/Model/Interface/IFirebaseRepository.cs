using System.Collections.Generic;
using System.Threading.Tasks;

namespace Script.Model.Interface
{
    public interface IFirebaseRepository
    {
        public void PostLetterData(LetterData letter);
        public Task<List<LetterData>> GetLetterData(string wayPointId);
    }
}