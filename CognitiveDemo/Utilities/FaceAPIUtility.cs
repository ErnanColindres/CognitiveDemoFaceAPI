using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using CognitiveDemo.Models;
using Microsoft.ProjectOxford.Face;
using Microsoft.ProjectOxford.Face.Contract;

namespace CognitiveDemo.Utilities
{
    public class FaceAPIUtility
    {
        private readonly IFaceServiceClient fsClient = new FaceServiceClient("68ab1b78c0304e2a84e990d6bbef75a0", "https://southeastasia.api.cognitive.microsoft.com/face/v1.0");
        
        public async Task<string> DetectEmotion(string filePath)
        {
            var emotion = string.Empty;

            try
            {
                using (var imgStream = File.OpenRead(filePath))
                {
                    var faceattrbutes = new List<FaceAttributeType>();
                    faceattrbutes.Add(FaceAttributeType.Emotion);

                    var face = await fsClient.DetectAsync(imgStream, true, true, faceattrbutes);
                    var emotionresult = face.Select(f => new {
                        f.FaceAttributes.Emotion
                    }).ToList().FirstOrDefault();
                    
                    IEnumerable<KeyValuePair<string, float>> emotionrating = new List<KeyValuePair<string, float>>();
                    emotionrating = emotionresult.Emotion.ToRankedList().OrderByDescending(f => f.Value);
                    emotion = emotionrating.FirstOrDefault().Key;
                }
            }
            catch (Exception e)
            {
                emotion = "error";
            }

            return emotion;
        }

        public async Task RegisterAnImage(string groupId, string groupName, string personName, string imageData)
        {
            await CreatePersonGroup(groupName, groupId);
            await CreatePerson(personName, groupId, imageData);
            await TrainPersonGroup(groupId);
        }

        private async Task CreatePersonGroup(string groupName, string groupId)
        {
            var group = await fsClient.GetPersonGroupAsync(groupId);

            if (group == null)
                await fsClient.CreatePersonGroupAsync(groupId, groupName);
        }

        private async Task CreatePerson(string personname, string groupId, string imagePath)
        {
            CreatePersonResult person = await fsClient.CreatePersonInPersonGroupAsync(groupId, personname);

            Stream s = File.OpenRead(imagePath);
            await fsClient.AddPersonFaceInPersonGroupAsync(groupId, person.PersonId, s);
        }

        private async Task TrainPersonGroup(string groupId)
        {
            await fsClient.TrainPersonGroupAsync(groupId);

            TrainingStatus trainingstatus = null;
            while (true)
            {
                trainingstatus = await fsClient.GetPersonGroupTrainingStatusAsync(groupId);

                if (trainingstatus.Status != Status.Running)
                    break;

                await Task.Delay(1000);
            }
        }

        public async Task<string> IdentifyImage(string imagePath, string groupId)
        {
            var faces = await UploadAndDetectFaces(imagePath);
            var faceIds = faces.Select(f => f.FaceId).ToArray();

            foreach (var result in await fsClient.IdentifyAsync(faceIds, personGroupId: groupId))
            {
                if (result.Candidates.Length != 0)
                {
                    var personId = result.Candidates[0].PersonId;
                    var person = await fsClient.GetPersonInPersonGroupAsync(groupId, personId);

                    return $"Image was identified as { person.Name }";
                }
            }

            return "Image not recognize.";
        }

        private async Task<Face[]> UploadAndDetectFaces(string imagePath)
        {
            try
            {
                using (Stream imageFileStream = File.OpenRead(imagePath))
                {
                    var faces = await fsClient.DetectAsync(imageFileStream,
                        true,
                        true,
                        new FaceAttributeType[] {
                            FaceAttributeType.Gender,
                            FaceAttributeType.Age,
                            FaceAttributeType.Emotion
                        });
                    return faces.ToArray();
                }
            }
            catch (Exception ex)
            {
                return new Face[0];
            }
        }

        public async Task<List<FaceRectangleInfo>> GetFaceRectangle(string imagePath)
        {
            try
            {
                using (Stream imageFileStream = File.OpenRead(imagePath))
                {
                    var faces = await fsClient.DetectAsync(imageFileStream, true, true, new FaceAttributeType[] {
                            FaceAttributeType.Gender,
                            FaceAttributeType.Age,
                            FaceAttributeType.Emotion
                    });

                    var results = new List<FaceRectangleInfo>();                    

                    foreach (var face in faces)
                    {
                        var result = new FaceRectangleInfo();
                        result.Age = (int)face.FaceAttributes.Age;
                        result.Emotion = face.FaceAttributes.Emotion.ToRankedList().OrderByDescending(e => e.Value).Select(e => e.Key).FirstOrDefault();
                        result.Gender = face.FaceAttributes.Gender;
                        result.Left = face.FaceRectangle.Left;
                        result.Top = face.FaceRectangle.Top;
                        result.Width = face.FaceRectangle.Width;
                        result.Height = face.FaceRectangle.Height;

                        results.Add(result);
                    }

                    return results;
                }
            }
            catch (Exception e)
            { 
                return null;
            }
        }

        public async Task<bool> HasFace(string imagePath)
        {
            try
            {
                using (Stream imageFileStream = File.OpenRead(imagePath))
                {
                    var faces = await fsClient.DetectAsync(imageFileStream, true, true, new FaceAttributeType[] {
                            FaceAttributeType.Gender,
                            FaceAttributeType.Age,
                            FaceAttributeType.Emotion
                    });

                    return faces.Count() > 0;
                }
            }
            catch
            {
                return false;
            }
        }
    }
}