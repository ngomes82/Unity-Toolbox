using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class FileIOCoroutines
{
    const float DEFAULT_FILE_IO_MAX_EXECUTION_TIME = 1f / 120f;
    const int   BYTES_PER_TIME_SLICE_QUERY         = 1000;

    public static IEnumerator Read(string path, Action<byte[]> OnComplete, float maxExecutionTime=DEFAULT_FILE_IO_MAX_EXECUTION_TIME)
    {
        return new TimeSlicedCoroutine(maxExecutionTime, ReadFileCoroutine(path, OnComplete));
    }

    public static IEnumerator Write(string path, byte[] bytes, Action OnComplete=null, float maxExecutionTime=DEFAULT_FILE_IO_MAX_EXECUTION_TIME)
    {
        return new TimeSlicedCoroutine(maxExecutionTime, WriteFileCoroutine(path, bytes, OnComplete));
    }

    private static IEnumerator ReadFileCoroutine(string path, Action<byte[]> OnComplete)
    {
        FileStream fileStream   = new FileStream(path, FileMode.Open);
        int totalByteCount      = (int)fileStream.Length;
        int sumBytesRead        = 0;
        byte[] buffer           = new byte[totalByteCount];
        int currenBytesRead     = fileStream.Read(buffer, sumBytesRead, BYTES_PER_TIME_SLICE_QUERY);
        
        yield return null;

        while ( currenBytesRead > 0)
        {
            currenBytesRead = fileStream.Read(buffer, sumBytesRead, BYTES_PER_TIME_SLICE_QUERY);
            sumBytesRead += currenBytesRead;

            yield return null;
        }

        fileStream.Close();
        fileStream.Dispose();

        OnComplete(buffer);
    }

    private static IEnumerator WriteFileCoroutine(string path, byte[] bytes, Action OnComplete)
    {
        FileStream fileStream = new FileStream(path, FileMode.OpenOrCreate);
        int sumBytesWritten = 0;

        while(sumBytesWritten < bytes.Length)
        {
            int bytesToWrite = Mathf.Min(BYTES_PER_TIME_SLICE_QUERY, bytes.Length - sumBytesWritten);
            fileStream.Write(bytes, sumBytesWritten, bytesToWrite);
            sumBytesWritten += bytesToWrite;

            yield return null;
        }

        fileStream.Close();
        fileStream.Dispose();

        OnComplete?.Invoke();
    }
}
