using CefSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Floater
{
    class DownloadHandler : IDownloadHandler
    {
        private readonly string _path;
        private Stream _stream;

        public DownloadHandler(string fileName)
        {
            _path = System.IO.Path.Combine(System.IO.Path.GetTempPath(), fileName);
            _stream = File.Create(_path);
        }

        public DownloadHandler()
        {
        }

        public bool ReceivedData(byte[] data)
        {
            _stream.Write(data, 0, data.GetLength(0));
            return true;

        }
        public void Complete()
        {
            _stream.Dispose();
            _stream = null;

            MessageBox.Show("Downloaded: {0}", _path);
            //Console.WriteLine("Downloaded: {0}", _path);
        }

        public void OnBeforeDownload(IWebBrowser chromiumWebBrowser, IBrowser browser, DownloadItem downloadItem, IBeforeDownloadCallback callback)
        {
            //throw new NotImplementedException();
        }

        public void OnDownloadUpdated(IWebBrowser chromiumWebBrowser, IBrowser browser, DownloadItem downloadItem, IDownloadItemCallback callback)
        {
            //throw new NotImplementedException();
        }
    }
}
