using System.Collections.Generic;
using System.Threading.Tasks;
using Firebase.Firestore;
using Script.Model.Interface;

namespace Script.Model
{
    public class FirebaseRepository: IFirebaseRepository 
    {
        // private Firebase
        // WaySportがつくられた＆＆ローカライズに成功していればそのデータ情報を保存する
        public async void PostLetterData(LetterData letter)
        {
            Dictionary<string, object> data = new Dictionary<string, object>()
            {
                {"wayspotId", letter.wayspotId},
                {"message", letter.message},
                {"payload", letter.payLoad}, 
                {"positionX",letter.positionX },
                {"positionY",letter.positionY },
                {"positionZ",letter.positionZ },
            };
            await FirebaseFirestore.DefaultInstance.Collection("letter").Document().SetAsync(data);
        }

        public async Task<List<LetterData>> GetLetterData(string wayPointId)
        {
            var list = new List<LetterData>();
            var instance = FirebaseFirestore.DefaultInstance;
            var path = instance.Collection("letter");
            var query = path.WhereEqualTo("wayspotId", wayPointId);
            var snapShot = await query.GetSnapshotAsync();
            foreach (var document in snapShot.Documents)
            {
                var doc = document.ToDictionary();
                var wayspotId = doc["wayspotId"].ToString();
                var message = doc["message"].ToString();
                var payload = doc["payload"].ToString();
                var positionX = float.Parse(doc["positionX"].ToString());
                var positionY = float.Parse(doc["positionY"].ToString());
                var positionZ = float.Parse(doc["positionZ"].ToString());
                var letter = new LetterData(wayspotId, message, payload, positionX, positionY, positionZ);
                list.Add(letter);
            }
            return list;
        }
    }
}
