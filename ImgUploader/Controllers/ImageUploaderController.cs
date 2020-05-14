using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using System.IO;
using AwsWrapper;
using ImgUploader.Models;
using ImageUtilityWrapper;
using Amazon.S3;
using Amazon.S3.Model;
using ImageResizer;

namespace ImgUploader.Controllers
{
    public class ImageUploaderController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public string ImageUpload(ImageParameterModel data)
        {
            if (string.IsNullOrEmpty(data.FileName))
                return string.Empty;
            string fileName = string.Concat(Path.GetFileNameWithoutExtension(data.FileName), ".jpeg");
            string base64 = data.Source.Substring(data.Source.IndexOf(',') + 1);
            base64 = base64.Trim('\0');
            byte[] imgData = Convert.FromBase64String(base64);
            Image img = byteArrayToImage(imgData);
            string path = Path.Combine(Server.MapPath("~/AwsTemp"), fileName);

            img.Save(path);
            var versions = new Dictionary<string, string>();

            //Define the versions to generate
            versions.Add("_thumb", string.Concat("maxwidth=", data.ThumbWidth, "&maxheight=", data.ThumbHeight, "&format=jpeg&speed=0&quality=90"));
            versions.Add("_main", string.Concat("maxwidth=", data.MainWidth, "&maxheight=", data.MainHeight, "&format=jpeg&speed=0&quality=90"));
            versions.Add("_pop", string.Concat("maxwidth=", data.PopWidth, "&maxheight=", data.PopHeight, "&format=jpeg&speed=0&quality=90"));

            //Generate each version
            foreach (var suffix in versions.Keys)
            {
                //Let the image builder add the correct extension based on the output file type
                ImageBuilder.Current.Build(
                    new ImageJob(
                        path,
                        path + suffix,
                        new Instructions(versions[suffix]),
                        false,
                        true));
            }


            if (System.IO.File.Exists(path))
                System.IO.File.Delete(path);


            if (data.ThumbRequired)
            {
                // Create thumbnail
                AwsApi.UploadFile(data.AWSBucketName, string.Concat(data.AWSThumbPath, fileName), string.Concat(path, "_thumb.jpg"));
                if (System.IO.File.Exists(string.Concat(path, "_thumb.jpg")))
                    System.IO.File.Delete(string.Concat(path, "_thumb.jpg"));
            }

            // Create Main
            AwsApi.UploadFile(data.AWSBucketName, string.Concat(data.AWSMainPath, fileName), string.Concat(path, "_main.jpg"));
            if (System.IO.File.Exists(string.Concat(path, "_main.jpg")))
                System.IO.File.Delete(string.Concat(path, "_main.jpg"));

            if (data.PopRequired)
            {
                // Create Pop
                AwsApi.UploadFile(data.AWSBucketName, string.Concat(data.AWSPopPath, fileName), String.Concat(path, "_pop.jpg"));
                if (System.IO.File.Exists(String.Concat(path, "_pop.jpg")))
                    System.IO.File.Delete(String.Concat(path, "_pop.jpg"));
            }

            return fileName;
        }

        private Image byteArrayToImage(byte[] bytesArr)
        {
            MemoryStream memstr = new MemoryStream(bytesArr);
            Image img = Image.FromStream(memstr);
            return img;
        }

        public ActionResult GetBucketObjectList(string bucketName, string path, int pageIndex, int pageSize, string searchString)
        {
            List<ListObjectsResponse> responses = null;
            IList<S3Object> s3Objects = new List<S3Object>();
            if (pageIndex == 0)
            {
                responses = AwsApi.GetBucketLisOfObjects(bucketName, path);
                for (var i = 0; i < responses.Count; i++)
                {
                    var s3ObjectsTemp = responses[i].S3Objects;

                    IList<S3Object> s3ObjectsTempList = s3ObjectsTemp.Where(x => Path.HasExtension(x.Key) && Path.GetFileName(x.Key).ToLower().Contains(searchString.Trim().ToLower())).ToList();
                    foreach(var s3Temp in s3ObjectsTempList)
                    {
                        s3Objects.Add(s3Temp);
                    }
                }
                Session["S3objects"] = s3Objects;
            }
            else
                s3Objects = (IList<S3Object>)Session["S3objects"];

            return Json(s3Objects.Where(x => Path.GetFileName(x.Key).ToLower().Contains(searchString.Trim().ToLower())).Skip(pageSize * pageIndex).Take(pageSize).ToList(), JsonRequestBehavior.AllowGet);
        }

        public ActionResult ImageDownload(string bucketName, string keyName, string fileName, string fullKeyName)
        {
            AwsApi.DownloadFile(bucketName, keyName, fileName, fullKeyName);
            return Json(fileName, JsonRequestBehavior.AllowGet);
        }
    }
}
