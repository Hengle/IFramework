/*********************************************************************************
 *Author:         OnClick
 *Version:        1.0
 *UnityVersion:   2017.2.3p3
 *Date:           2019-08-06
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/
using System;

namespace IFramework.Net
{
    public interface IDownLoader:IDisposable
    {
        float Progress { get; }
        string Url { get; }
        /// ��Դ���ش��·�����������ļ���
        string SaveDir { get; }
        /// �ļ���,��������׺
        string FileNameWithoutExt { get; }
        /// �ļ���׺
        string FileExt { get; }
        /// �����ļ�ȫ·����·��+�ļ���+��׺
        string SaveFilePath { get; }
        /// ԭ�ļ���С
        long FileLength { get; }
        /// ��ǰ���غ��˵Ĵ�С
        long CurrentLength { get; }
        /// �Ƿ�ʼ����
        bool IsDownLoading { get; }
        void DownLoad();
       
    }
}