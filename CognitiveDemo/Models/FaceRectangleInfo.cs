using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CognitiveDemo.Models
{
    public class FaceRectangleInfo
    {
        public string Gender { get; set; }
        public int Age { get; set; }
        public string Emotion { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public int Top { get; set; }
        public int Left { get; set; }
    }
}