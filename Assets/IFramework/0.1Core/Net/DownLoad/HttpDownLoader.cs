/*********************************************************************************
 *Author:         OnClick
 *Version:        1.0
 *UnityVersion:   2017.2.3p3
 *Date:           2019-08-06
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/
using System;
using System.IO;
using System.Net;
using UnityEngine;
namespace IFramework.Net
{
    public class HttpDownLoader : DownLoader
    {
        private FileStream fileStream;
        private Stream stream;
        private HttpWebResponse response;
        private string tempFileExt = ".temp";
        /// ��ʱ�ļ�ȫ·��
        private string tempSaveFilePath;
        public override float Progress
        {
            get
            {
                if (fileLength > 0)
                    return Mathf.Clamp((float)currentLength / fileLength, 0, 1);
                return 0;
            }
        }
        public HttpDownLoader(string url, string SaveDir, Action<float> CallProgress) : base(url, SaveDir, CallProgress)
        {
            tempSaveFilePath = string.Format("{0}/{1}{2}", SaveDir, FileNameWithoutExt, tempFileExt);
        }
        public override void DownLoad()
        {
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(Url);
            request.Method = "GET";
            if (File.Exists(tempSaveFilePath))
            {
                //��֮ǰ��������һ���֣���������
                fileStream = File.OpenWrite(tempSaveFilePath);
                currentLength = fileStream.Length;
                fileStream.Seek(currentLength, SeekOrigin.Current);
                //�������ص��ļ���ȡ����ʼλ��
                request.AddRange((int)currentLength);
                progress = Mathf.Clamp((float)currentLength / fileLength, 0, 1);
            }
            else
            {
                //��һ������
                fileStream = new FileStream(tempSaveFilePath, FileMode.Create, FileAccess.Write);
                currentLength = 0;
                progress = 0;
            }
            if (CallProgress!=null) CallProgress(progress);

            response = (HttpWebResponse)request.GetResponse();
            stream = response.GetResponseStream();
            //�ܵ��ļ���С=��ǰ��Ҫ���ص�+�����ص�
            fileLength = response.ContentLength + currentLength;

            IsDownLoading = true;
            int lengthOnce;
            int bufferMaxLength = 1024 * 20;

            while (currentLength < fileLength)
            {
                progress = Mathf.Clamp((float)currentLength / fileLength, 0, 1);
                byte[] buffer = new byte[bufferMaxLength];
                if (stream.CanRead)
                {
                    //��д����
                    lengthOnce = stream.Read(buffer, 0, buffer.Length);
                    currentLength += lengthOnce;
                    fileStream.Write(buffer, 0, lengthOnce);
                }
                else
                {
                    break;
                }
                if (CallProgress != null) CallProgress(progress);
            }
            IsDownLoading = false;
            response.Close();
            stream.Close();
            fileStream.Close();
            //��ʱ�ļ�תΪ���յ������ļ�
            if (File.Exists(SaveFilePath))
            {
                File.Delete(SaveFilePath);
            }
            File.Move(tempSaveFilePath, SaveFilePath);
            if (CallProgress != null) CallProgress(1);
        }

        public override void Dispose()
        {
            IsDownLoading = false;
            if (response!=null) response.Close();
            if (stream!=null) stream.Close();
            if (fileStream!=null) fileStream.Close();
        }
    }
}