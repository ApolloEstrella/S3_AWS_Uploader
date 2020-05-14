using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using Amazon.S3;
using Amazon.S3.Model;
using System.Web;


namespace AwsWrapper
{
    public class AwsApi
    {
        private static string AwsAccessKey = ConfigurationManager.AppSettings["AWSAccessKey"].ToString();
        private static string AwsSecretKey = ConfigurationManager.AppSettings["AWSSecretKey"].ToString();
        //private static string AwsBucketName = ConfigurationManager.AppSettings["AWSBucketName"].ToString();

        //static string keyName = "*** key name when object is created ***";
        //static string filePath = "*** absolute path to a sample file to upload ***";

        private static IAmazonS3 client;

        public static void UploadFile(string bucketName, string keyName, string filePath)
        {
            //keyName = @"media\articles\thumb\2.jpeg";
            client = new AmazonS3Client(AwsAccessKey, AwsSecretKey, Amazon.RegionEndpoint.USEast1);
            PutObjectRequest request = new PutObjectRequest()
            {
                BucketName = bucketName,
                Key = keyName,
                FilePath = filePath
            };
            PutObjectResponse response2 = client.PutObject(request);
        }

        public static List<ListObjectsResponse> GetBucketLisOfObjects(string bucketName, string prefix)
        {
            client = new AmazonS3Client(AwsAccessKey, AwsSecretKey, Amazon.RegionEndpoint.USEast1);
            List<ListObjectsResponse> responses = new List<ListObjectsResponse>();
            ListObjectsRequest request = new ListObjectsRequest
            {
                BucketName = bucketName,

                Prefix = prefix
               
                //MaxKeys = 2,
                
            };
            do
            {
                ListObjectsResponse response = client.ListObjects(request);

                responses.Add(response);
                // Process response.


                // If response is truncated, set the marker to get the next 
                // set of keys.
                if (response.IsTruncated)
                {
                    request.Marker = response.NextMarker;
                }
                else
                {
                    request = null;
                }
            } while (request != null);
            return responses;
        }

        public static void DownloadFile(string bucketName, string keyName,string fileName, string fullKeyName)
        {
            using (client = new AmazonS3Client(AwsAccessKey, AwsSecretKey, Amazon.RegionEndpoint.USEast1))
            {
                GetObjectRequest request = new GetObjectRequest
                {
                    BucketName = bucketName,
                    Key = fullKeyName  //string.Concat(keyName, fileName)
                };

                using (GetObjectResponse response = client.GetObject(request))
                {
                    //string dest = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), keyName);
                    string dest = Path.Combine(HttpContext.Current.Server.MapPath("~/AwsTemp"), fileName);
                    if (!File.Exists(dest))
                    {
                        response.WriteResponseStreamToFile(dest);
                    }
                }
            }
        }
    }
}