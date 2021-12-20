#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using System;
using System.Collections;
using System.IO;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Net;
using System.Security.Cryptography;
using System.Text;

public class AssetBundleBuilder : EditorWindow
{
    private static string rootUrl = string.Empty;
    private static string userId = string.Empty;
    private static string password = string.Empty;

    [MenuItem("Tools/Asset Bundle Builder")]
    static void ShowWindow()
    {
        var window = EditorWindow.GetWindow(typeof(AssetBundleBuilder));
        window.maxSize = new Vector2(500f, 500f);
        window.minSize = window.maxSize;
    }

    void OnGUI()
    {

        if (GUILayout.Button("Build All"))
        {
            string outputPath = GetBundleBuildDir();

            var manifest = BuildPipeline.BuildAssetBundles(outputPath, BuildAssetBundleOptions.ChunkBasedCompression, EditorUserBuildSettings.activeBuildTarget);

            var bundleHashMap = new Dictionary<string, string>();
            var assetBundles = manifest.GetAllAssetBundles();
            for(int i=0; i < assetBundles.Length; i++)
            {
                bundleHashMap[assetBundles[i]] = manifest.GetAssetBundleHash(assetBundles[i]).ToString();
            }

            File.WriteAllText(GetBundleBuildDir() + "/bundleHashes.json", JsonConvert.SerializeObject(bundleHashMap) );
        }

        rootUrl = EditorGUILayout.TextField("Root Url: ", rootUrl);
        userId = EditorGUILayout.TextField("User Id: ", userId);
        password = EditorGUILayout.TextField("Password: ", password);

        if (GUILayout.Button("Upload All"))
        {
            S3Uploader s3Uploader = new S3Uploader(rootUrl, userId, password);

            var filesToUpload = Directory.GetFiles(GetBundleBuildDir());
            for(int i=0; i < filesToUpload.Length; i++)
            {
                string name = Path.GetFileName(filesToUpload[i]);
                s3Uploader.UploadFileToAWS3(name, filesToUpload[i]);
            }
        }
    }

    private string GetBundleBuildDir()
    {
        string outputPath = $"Build/AssetBundles/{ AssetBundleManager.GetRuntimePlatformFromBuildTarget(EditorUserBuildSettings.activeBuildTarget) }";

        if (!Directory.Exists(outputPath))
        {
            Directory.CreateDirectory(outputPath);
        }

        return outputPath;
    }
}

public class S3Uploader
{
    private string awsBucketName = "MyTestBucket";
    private string awsAccessKey = "XXXxxxXXXxxx";
    private string awsSecretKey = "XXXXxxxxxxXXXxxxXXX";
    private string awsURLBaseVirtual = "";

    public S3Uploader(string bucketName, string accessKey, string secretKey)
    {
        awsAccessKey = accessKey;
        awsSecretKey = secretKey;
        awsBucketName = bucketName;

        awsURLBaseVirtual = "http://" +
           awsBucketName +
           ".s3.amazonaws.com/";
    }
    public void UploadFileToAWS3(string FileName, string FilePath)
    {
        string region = "us-east-2";
        string longDate = System.DateTime.UtcNow.ToString("yyyyMMdd'T'HHmmss'Z'");
        string shortDate = System.DateTime.UtcNow.ToString("yyyyMMdd");
        byte[] signingKey = getSignatureKey(awsSecretKey, shortDate, region, "s3");

        string canonicalString = $"PUT\n" +
                                 $"/{FileName}\n\n" +
                                 $"host:{awsBucketName}.s3.amazonaws.com\n" +
                                 $"x-amz-content-sha256:UNSIGNED-PAYLOAD\n" +
                                 $"x-amz-date:{longDate}\n\n" +
                                 $"host;x-amz-content-sha256;x-amz-date\n"+
                                 $"UNSIGNED-PAYLOAD";

        
        
        string hashedCanonicalRequest = ComputeSha256Hash(canonicalString);

        string stringToSign = $"AWS4-HMAC-SHA256\n"+
                              $"{longDate}\n"+
                              $"{shortDate}/{region}/s3/aws4_request\n"+
                              $"{hashedCanonicalRequest}";

        
        string signature = ByteArrayToString( HmacSHA256(stringToSign, signingKey) );


        //string canonicalUtf8Bytes = ByteArrayToString( Encoding.UTF8.GetBytes(canonicalString) );
        //string stringToSignUtf8Bytes = ByteArrayToString(Encoding.UTF8.GetBytes(stringToSign));
        //Debug.Log($"{canonicalString}\n\n{canonicalUtf8Bytes}\n\n{stringToSign}\n\n{stringToSignUtf8Bytes}\n\n{signature}");


        string url = $"http://{awsBucketName}.s3.amazonaws.com/{FileName}";
        string authHeader = $"AWS4-HMAC-SHA256 Credential={awsAccessKey}/{shortDate}/{region}/s3/aws4_request," +
                            $"SignedHeaders=host;x-amz-date;x-amz-content-sha256," +
                            $"Signature={signature}";

        WebRequest webRequest = (HttpWebRequest)WebRequest.Create(url);
        webRequest.Method = "PUT";

        webRequest.Headers.Add("Authorization", authHeader);
        webRequest.Headers.Add("x-amz-content-sha256", "UNSIGNED-PAYLOAD");
        webRequest.Headers.Add("x-amz-date", longDate);

        byte[] fileRawBytes = File.ReadAllBytes(FilePath);
        webRequest.ContentLength = fileRawBytes.Length;
        


        //Send the request
        Stream S3Stream = webRequest.GetRequestStream();
        S3Stream.Write(fileRawBytes, 0, fileRawBytes.Length);
        
        Debug.Log("Sent bytes: " +
            webRequest.ContentLength +
            ", for file: " +
            FileName);

        S3Stream.Close();

        WebResponse response = webRequest.GetResponse();
        
        Debug.Log(response.ToString());
    }

    static byte[] HmacSHA256(String data, byte[] key)
    {
        String algorithm = "HmacSHA256";
        KeyedHashAlgorithm kha = KeyedHashAlgorithm.Create(algorithm);
        kha.Key = key;

        return kha.ComputeHash(Encoding.UTF8.GetBytes(data));
    }

    static byte[] getSignatureKey(String secretKey, String dateStamp, String regionName, String serviceName)
    {
        byte[] kSecret = Encoding.UTF8.GetBytes(("AWS4" + secretKey).ToCharArray());
        byte[] kDate = HmacSHA256(dateStamp, kSecret);
        byte[] kRegion = HmacSHA256(regionName, kDate);
        byte[] kService = HmacSHA256(serviceName, kRegion);
        byte[] kSigning = HmacSHA256("aws4_request", kService);

        return kSigning;
    }

    public static string ByteArrayToString(byte[] ba)
    {
        StringBuilder hex = new StringBuilder(ba.Length * 2);
        foreach (byte b in ba)
            hex.AppendFormat("{0:x2}", b);
        return hex.ToString();
    }

    static string ComputeSha256Hash(string rawData)
    {
        using (SHA256 sha256Hash = SHA256.Create())
        {
            byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(rawData));

            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < bytes.Length; i++)
            {
                builder.Append(bytes[i].ToString("x2"));
            }
            return builder.ToString();
        }
    }
}
#endif