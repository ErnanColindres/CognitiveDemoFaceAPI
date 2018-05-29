using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.ProjectOxford.Face;

namespace CognitiveDemo.Utilities
{
    public class FaceVerification
    {
        private readonly IFaceServiceClient fsclient = new FaceServiceClient("67721a7f67434297a005f37d4ca0f428", "https://southeastasia.api.cognitive.microsoft.com/face/v1.0");

    }
}