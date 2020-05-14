using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ImgUploader.Models;
using Amazon.S3;
using Amazon.S3.Model;

namespace ImgUploader.Controllers
{
    public class AdminController : Controller
    {
        public ActionResult Index()
        {
            ImageParameterModel imageParameter = new ImageParameterModel
            {
                AWSBucketName = "worldpipelines", //"testbucketpp",
                AWSBucketPath = "media/articles/",
                AWSSelectPath = "media/articles/thumb/",
                ThumbRequired = true,
                PopRequired = true,
                ThumbWidth = "165",
                ThumbHeight = "110",
                MainWidth = "240",
                MainHeight = "160",
                PopWidth = "450",
                PopHeight = "300",
                AWSThumbPath = "media/articles/thumb/",
                AWSMainPath = "media/articles/main/",
                AWSPopPath = "media/articles/pop/"
            };

            return View(imageParameter);
        }

    }
}
