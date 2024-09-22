using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SlimDX;
using Newtonsoft.Json;

namespace AirHockeyGame
{
    public class Status
    {
        public Vector2 PuckPosition { get; set; }
        public Vector2 PaddlePosition { get; set; }
        public int PlayerOneScore { get; set; }
        public int PlayerTwoScore { get; set; }
        
        public Status(Vector2 puckPos, Vector2 p1Pos, int p1Score, int p2Score)
        {
            PuckPosition = puckPos;
            PuckPosition = p1Pos;
            PlayerOneScore = p1Score;
            PlayerTwoScore = p2Score;
        }

        public string ToJson()
        {
            return JsonConvert.SerializeObject(this);
        }

        public static Status FromJson(string jsonData)
        {
            return JsonConvert.DeserializeObject<Status>(jsonData);
        }
    }
}