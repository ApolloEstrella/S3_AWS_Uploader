using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ImgUploader.Models
{
    public class ImageParameterModel
    {
        public string Source { get; set; }
        public string Source2 { get; set; }
        public string Source3 { get; set; }
        public string FileName { get; set; }
        public string AWSBucketName { get; set; }
        public string AWSBucketPath { get; set; }
        public string AWSSelectPath { get; set; }
        public bool ThumbRequired { get; set; }
        public bool PopRequired { get; set; }
        public string ThumbWidth { get; set; }
        public string ThumbHeight { get; set; }
        public string MainWidth { get; set; }
        public string MainHeight { get; set; }
        public string PopWidth { get; set; }
        public string PopHeight { get; set; }
        public string AWSThumbPath { get; set; }
        public string AWSMainPath { get; set; }
        public string AWSPopPath { get; set; }
        public string Prefix { get; set; }
    }
}