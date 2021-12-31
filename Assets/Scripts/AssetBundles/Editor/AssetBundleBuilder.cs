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
    private static string awsBucketName = string.Empty;
    private static string awsAccessKey = string.Empty;
    private static string awsSecretKey = string.Empty;
    private static AssetBundleManager.AssetServerEnvironment environment = AssetBundleManager.AssetServerEnvironment.Dev;

    [MenuItem("Tools/Asset Bundles/Build Window", priority = -1)]
    static void ShowWindow()
    {
        var window = EditorWindow.GetWindow(typeof(AssetBundleBuilder));
        window.maxSize = new Vector2(500f, 500f);
        window.minSize = window.maxSize;
    }


    [MenuItem("Tools/Asset Bundles/Bundle Load Mode/Simulation")]
    static void SwitchModeSimulation()
    {
        SetBundleLoadMode(AssetBundleManager.EditorBundleLoadMode.Simulation);
    }

    [MenuItem("Tools/Asset Bundles/Bundle Load Mode/Simulation", true)]
    private static bool SwitchModeSimulationValidate()
    {
        SetCheckMark((AssetBundleManager.EditorBundleLoadMode)EditorPrefs.GetInt(AssetBundleManager.KEY_EDITOR_BUNDLE_LOAD_MODE));
        return true;
    }

    [MenuItem("Tools/Asset Bundles/Bundle Load Mode/Local")]
    static void SwitchModeLocal()
    {
        SetBundleLoadMode(AssetBundleManager.EditorBundleLoadMode.Local);
    }

    [MenuItem("Tools/Asset Bundles/Bundle Load Mode/Server")]
    static void SwitchModeServer()
    {
        SetBundleLoadMode(AssetBundleManager.EditorBundleLoadMode.Server);
    }

    private static void SetBundleLoadMode(AssetBundleManager.EditorBundleLoadMode newMode)
    {
        EditorPrefs.SetInt(AssetBundleManager.KEY_EDITOR_BUNDLE_LOAD_MODE, (int)newMode);
        SetCheckMark(newMode);
    }

    private static void SetCheckMark(AssetBundleManager.EditorBundleLoadMode newMode)
    {
        Menu.SetChecked("Tools/Asset Bundles/Bundle Load Mode/Simulation", newMode == AssetBundleManager.EditorBundleLoadMode.Simulation);
        Menu.SetChecked("Tools/Asset Bundles/Bundle Load Mode/Local", newMode == AssetBundleManager.EditorBundleLoadMode.Local);
        Menu.SetChecked("Tools/Asset Bundles/Bundle Load Mode/Server", newMode == AssetBundleManager.EditorBundleLoadMode.Server);
    }


    void OnGUI()
    {

        if (GUILayout.Button("Build All"))
        {
            string outputPath = AssetBundleManager.GetBundleBuildDir();

            var manifest = BuildPipeline.BuildAssetBundles(outputPath, BuildAssetBundleOptions.ChunkBasedCompression, EditorUserBuildSettings.activeBuildTarget);

            var bundleHashMap = new Dictionary<string, string>();
            var assetBundles = manifest.GetAllAssetBundles();
            for(int i=0; i < assetBundles.Length; i++)
            {
                bundleHashMap[assetBundles[i]] = manifest.GetAssetBundleHash(assetBundles[i]).ToString();
            }

            File.WriteAllText(AssetBundleManager.GetBundleBuildDir() + "/bundleHashes.json", JsonConvert.SerializeObject(bundleHashMap) );
        }

        awsBucketName = EditorGUILayout.TextField("Aws S3 Bucket Name: ", awsBucketName);
        awsAccessKey = EditorGUILayout.TextField("Aws Access Key: ", awsAccessKey);
        awsSecretKey = EditorGUILayout.PasswordField("Aws Secret Key: ", awsSecretKey);
        environment = (AssetBundleManager.AssetServerEnvironment) EditorGUILayout.EnumPopup("Environment: ", environment);

        if (GUILayout.Button("Upload All"))
        {
            S3Uploader s3Uploader = new S3Uploader(awsBucketName, awsAccessKey, awsSecretKey);

            string rootRemotePath = $"{environment}/{Application.version}/{AssetBundleManager.GetRuntimePlatformFromBuildTarget(EditorUserBuildSettings.activeBuildTarget)}";

            bool isConfirmed = true;
            if (environment == AssetBundleManager.AssetServerEnvironment.Prod)
            {
                isConfirmed = EditorUtility.DisplayDialog("Upload to Prod?", $"This will upload bundles to production server at path: \n\n http://{awsBucketName}.s3.amazonaws.com/{rootRemotePath}", "Upload to Production", "Cancel");
            }

            var filesToUpload = Directory.GetFiles(AssetBundleManager.GetBundleBuildDir());
            for(int i=0; i < filesToUpload.Length; i++)
            {
                if (isConfirmed)
                {
                    //TODO: Check server hashes against client hashes. Alert User to files changed and total MB changed. Confirm yes,no (type production for prod environment)
                    string localFilePath = filesToUpload[i];
                    string fileName = Path.GetFileName(localFilePath);
                    string remoteFilePath = $"{rootRemotePath}/{fileName}";
                    s3Uploader.UploadFileToAWS3(remoteFilePath, localFilePath);
                }
            }
        }
    }
}

public class S3Uploader
{
    private string awsBucketName = "MyTestBucket";
    private string awsAccessKey = "XXXxxxXXXxxx";
    private string awsSecretKey = "XXXXxxxxxxXXXxxxXXX";
    
    public S3Uploader(string bucketName, string accessKey, string secretKey)
    {
        awsAccessKey = accessKey;
        awsSecretKey = secretKey;
        awsBucketName = bucketName;
    }

    //NOTE: For more info on steps needed to sign Amazon REST requests follow this link:
    //https://docs.aws.amazon.com/general/latest/gr/sigv4-create-canonical-request.html
    public void UploadFileToAWS3(string remoteFilePath, string localFilePath)
    {
        string region = "us-east-2";
        string longDate = System.DateTime.UtcNow.ToString("yyyyMMdd'T'HHmmss'Z'");
        string shortDate = System.DateTime.UtcNow.ToString("yyyyMMdd");
        byte[] signingKey = getSignatureKey(awsSecretKey, shortDate, region, "s3");

        string canonicalString = $"PUT\n" +
                                 $"/{remoteFilePath}\n\n" +
                                 $"host:{awsBucketName}.s3.amazonaws.com\n" +
                                 $"x-amz-acl:public-read\n" +
                                 $"x-amz-content-sha256:UNSIGNED-PAYLOAD\n" +
                                 $"x-amz-date:{longDate}\n\n" +
                                 $"host;x-amz-acl;x-amz-content-sha256;x-amz-date\n" +
                                 $"UNSIGNED-PAYLOAD";

        
        
        string hashedCanonicalRequest = ComputeSha256Hash(canonicalString);

        string stringToSign = $"AWS4-HMAC-SHA256\n"+
                              $"{longDate}\n"+
                              $"{shortDate}/{region}/s3/aws4_request\n"+
                              $"{hashedCanonicalRequest}";

        
        string signature = ByteArrayToString( HmacSHA256(stringToSign, signingKey) );


        //DEBUG: Use the debug logs below alongside amazon error text to debug auth issues with signing.
        //string canonicalUtf8Bytes = ByteArrayToString( Encoding.UTF8.GetBytes(canonicalString) );
        //string stringToSignUtf8Bytes = ByteArrayToString(Encoding.UTF8.GetBytes(stringToSign));
        //Debug.Log($"{canonicalString}\n\n{canonicalUtf8Bytes}\n\n{stringToSign}\n\n{stringToSignUtf8Bytes}\n\n{signature}");


        string url = $"http://{awsBucketName}.s3.amazonaws.com/{remoteFilePath}";
        string authHeader = $"AWS4-HMAC-SHA256 Credential={awsAccessKey}/{shortDate}/{region}/s3/aws4_request," +
                            $"SignedHeaders=host;x-amz-acl;x-amz-content-sha256;x-amz-date," +
                            $"Signature={signature}";

        WebRequest webRequest = (HttpWebRequest)WebRequest.Create(url);
        webRequest.Method = "PUT";

        webRequest.Headers.Add("Authorization", authHeader);
        webRequest.Headers.Add("x-amz-acl", "public-read");
        webRequest.Headers.Add("x-amz-date", longDate);
        webRequest.Headers.Add("x-amz-content-sha256", "UNSIGNED-PAYLOAD");
        

        byte[] fileRawBytes = File.ReadAllBytes(localFilePath);
        webRequest.ContentLength = fileRawBytes.Length;
        


        //Send the request
        Stream S3Stream = webRequest.GetRequestStream();
        S3Stream.Write(fileRawBytes, 0, fileRawBytes.Length);
        
        Debug.Log("Sent bytes: " +
            webRequest.ContentLength +
            ", for file: " +
            remoteFilePath);

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