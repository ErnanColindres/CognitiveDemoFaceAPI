using CognitiveDemo.Models;
using CognitiveDemo.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace CognitiveDemo.Controllers
{
    public class HomeController : Controller
    {
        private string groupId = "1";
        private string groupName = "Cognitive Demo";

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        [HttpPost]
        public async Task<JsonResult> SaveImage(string imageData)
        {
            var path = @"C:\ScannedDocs\";
            var issaved = true;
            var emotion = string.Empty;
            try
            {
                string fileNameWitPath = path + DateTime.Now.ToString("ddMMyyyyHHmmss") + ".png";
                using (FileStream fs = new FileStream(fileNameWitPath, FileMode.Create))
                {
                    using (BinaryWriter bw = new BinaryWriter(fs))
                    {
                        byte[] data = Convert.FromBase64String(imageData);
                        bw.Write(data);
                        bw.Close();
                        
                        emotion = await new FaceAPIUtility().DetectEmotion(fileNameWitPath);
                    }
                }
            }
            catch (Exception e)
            {
                issaved = false;
            }
            return Json(new { Saved = issaved, Emotion = emotion }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public async Task<JsonResult> RegisterImage(string imageData, string personName)
        {
            var path = @"C:\ScannedDocs\";
            var issaved = true;
            try
            {
                string fileNameWitPath = path + personName + "_" + DateTime.Now.ToString("ddMMyyyyHHmmss") + ".png";

                if (!System.IO.File.Exists(fileNameWitPath))
                {
                    using (FileStream fs = new FileStream(fileNameWitPath, FileMode.Create))
                    {
                        using (BinaryWriter bw = new BinaryWriter(fs))
                        {
                            byte[] data = Convert.FromBase64String(imageData);
                            bw.Write(data);
                            bw.Close();
                        }
                    }
                }

                await new FaceAPIUtility().RegisterAnImage(groupId, groupName, personName, fileNameWitPath);
            }
            catch (Exception e)
            {
                issaved = false;
            }
            return Json(new { Saved = issaved }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public async Task<JsonResult> IdentifyImage(string imageData)
        {
            var path = @"C:\ScannedDocs\";
            var imagename = string.Empty;

            try
            {
                string fileNameWitPath = path + DateTime.Now.ToString("ddMMyyyyHHmmss") + ".png";

                if (!System.IO.File.Exists(fileNameWitPath))
                {
                    using (FileStream fs = new FileStream(fileNameWitPath, FileMode.Create))
                    {
                        using (BinaryWriter bw = new BinaryWriter(fs))
                        {
                            byte[] data = Convert.FromBase64String(imageData);
                            bw.Write(data);
                            bw.Close();
                        }
                    }
                }

                imagename = await new FaceAPIUtility().IdentifyImage(fileNameWitPath, groupId);
            }
            catch (Exception e)
            {
                imagename = "Error found while identifying the image.";
            }
            return Json(new { ImageName = imagename }, JsonRequestBehavior.AllowGet);

        }

        [HttpPost]
        public async Task<JsonResult> GetRectangle(string imageData)
        {
            var path = @"C:\ScannedDocs\";
            var facerectinfo = new List<FaceRectangleInfo>();

            try
            {
                string fileNameWitPath = path + DateTime.Now.ToString("ddMMyyyyHHmmss") + ".png";

                if (!System.IO.File.Exists(fileNameWitPath))
                {
                    using (FileStream fs = new FileStream(fileNameWitPath, FileMode.Create))
                    {
                        using (BinaryWriter bw = new BinaryWriter(fs))
                        {
                            byte[] data = Convert.FromBase64String(imageData);
                            bw.Write(data);
                            bw.Close();
                        }
                    }
                }

                facerectinfo = await new FaceAPIUtility().GetFaceRectangle(fileNameWitPath);
                System.IO.File.Delete(fileNameWitPath);
            }
            catch (Exception e)
            {

            }
            return Json(facerectinfo, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public async Task<JsonResult> CheckHasFace(string imageData)
        {
            var path = @"C:\ScannedDocs\";
            var hasface = false;

            try
            {
                string fileNameWitPath = path + DateTime.Now.ToString("ddMMyyyyHHmmss") + ".png";

                if (!System.IO.File.Exists(fileNameWitPath))
                {
                    using (FileStream fs = new FileStream(fileNameWitPath, FileMode.Create))
                    {
                        using (BinaryWriter bw = new BinaryWriter(fs))
                        {
                            byte[] data = Convert.FromBase64String(imageData);
                            bw.Write(data);
                            bw.Close();
                        }
                    }
                }

                hasface = await new FaceAPIUtility().HasFace(fileNameWitPath);
                System.IO.File.Delete(fileNameWitPath);
            }
            catch (Exception e)
            {

            }
            return Json(new { HasFace = hasface }, JsonRequestBehavior.AllowGet);
            
        }
    }
}