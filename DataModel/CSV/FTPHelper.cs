using System;
using System.IO;
using System.Net;

namespace DataModel.CSV
{

    /// <summary>
    /// FTP操作
    /// </summary>
    public class FTPHelper
    {
        #region FTPConfig
        string ftpURI;
        string ftpUserID;
        string ftpServerIP;
        string ftpPassword;
        string ftpRemotePath;
        #endregion

        /// <summary>  
        /// 连接FTP服务器
        /// </summary>  
        /// <param name="FtpServerIP">FTP连接地址</param>  
        /// <param name="FtpRemotePath">指定FTP连接成功后的当前目录, 如果不指定即默认为根目录</param>  
        /// <param name="FtpUserID">用户名</param>  
        /// <param name="FtpPassword">密码</param>  
        public FTPHelper(string FtpServerIP, string FtpRemotePath, string FtpUserID, string FtpPassword)
        {
            ftpServerIP = FtpServerIP;
            ftpRemotePath = FtpRemotePath;
            ftpUserID = FtpUserID;
            ftpPassword = FtpPassword;
            ftpURI = "ftp://" + ftpServerIP + "/" + ftpRemotePath + "/";
        }

        public bool CheckFtp()
        {
            try
            {
                FtpWebRequest ftprequest = (FtpWebRequest)FtpWebRequest.Create(new Uri(ftpURI));
                // ftp用户名和密码
                ftprequest.Credentials = new NetworkCredential(ftpUserID, ftpPassword);
                ftprequest.Method = WebRequestMethods.Ftp.ListDirectory;
                ftprequest.Timeout = 3000;
                FtpWebResponse ftpResponse = (FtpWebResponse)ftprequest.GetResponse();

                ftpResponse.Close();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        /// <summary>
        /// FTP文件上传
        /// </summary>
        /// <param name="localfile"></param>
        /// <param name="ftpfile"></param>
        /// <param name="pb"></param>
        /// <exception cref="Exception"></exception>
        public void Upload(string localfile, string ftpfile, System.Windows.Forms.ProgressBar pb)
        {
            FileInfo fileInf = new FileInfo(localfile);
            FtpWebRequest reqFTP;
            reqFTP = (FtpWebRequest)FtpWebRequest.Create(new Uri(ftpURI + ftpfile));
            reqFTP.Credentials = new NetworkCredential(ftpUserID, ftpPassword);
            reqFTP.Method = WebRequestMethods.Ftp.UploadFile;
            reqFTP.KeepAlive = false;
            reqFTP.UseBinary = true;
            reqFTP.ContentLength = fileInf.Length;
            if (pb != null)
            {
                pb.Maximum = Convert.ToInt32(reqFTP.ContentLength / 2048);
                pb.Maximum = pb.Maximum + 1;
                pb.Minimum = 0;
                pb.Value = 0;
            }
            int buffLength = 2048;
            byte[] buff = new byte[buffLength];
            int contentLen;
            FileStream fs = fileInf.OpenRead();
            try
            {
                Stream strm = reqFTP.GetRequestStream();
                contentLen = fs.Read(buff, 0, buffLength);
                while (contentLen != 0)
                {
                    strm.Write(buff, 0, contentLen);
                    if (pb != null)
                    {
                        if (pb.Value != pb.Maximum)
                            pb.Value = pb.Value + 1;
                    }
                    contentLen = fs.Read(buff, 0, buffLength);
                    System.Windows.Forms.Application.DoEvents();
                }
                if (pb != null)
                    pb.Value = pb.Maximum;
                System.Windows.Forms.Application.DoEvents();
                strm.Close();
                fs.Close();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// 从FTP中下载图片
        /// </summary>
        /// <param name="localfilename"></param>
        /// <param name="ftpfileName"></param>
        /// <param name="pb"></param>
        /// <exception cref="Exception"></exception>

        public void Download(string localfilename, string ftpfileName, System.Windows.Forms.ProgressBar pb = null)
        {
            long fileSize = GetFileSize(ftpfileName);
            if (fileSize > 0)
            {
                if (pb != null)
                {
                    pb.Maximum = Convert.ToInt32(fileSize / 2048);
                    pb.Maximum = pb.Maximum + 1;
                    pb.Minimum = 0;
                    pb.Value = 0;
                }
                try
                {
                    string directoryPath = Path.GetDirectoryName(localfilename);
                    if (!Directory.Exists(directoryPath))
                    {
                        Directory.CreateDirectory(directoryPath);
                    }

                    FileStream outputStream = new FileStream(localfilename, FileMode.Create);
                    FtpWebRequest reqFTP;
                    reqFTP = (FtpWebRequest)FtpWebRequest.Create(new Uri(ftpURI + ftpfileName));
                    reqFTP.Credentials = new NetworkCredential(ftpUserID, ftpPassword);
                    reqFTP.Method = WebRequestMethods.Ftp.DownloadFile;
                    reqFTP.UseBinary = true;
                    FtpWebResponse response = (FtpWebResponse)reqFTP.GetResponse();
                    Stream ftpStream = response.GetResponseStream();
                    int bufferSize = 2048;

                    int readCount;
                    byte[] buffer = new byte[bufferSize];
                    readCount = ftpStream.Read(buffer, 0, bufferSize);
                    while (readCount > 0)
                    {
                        outputStream.Write(buffer, 0, readCount);
                        if (pb != null)
                        {
                            if (pb.Value != pb.Maximum)
                                pb.Value = pb.Value + 1;
                        }
                        readCount = ftpStream.Read(buffer, 0, bufferSize);
                        System.Windows.Forms.Application.DoEvents();
                    }
                    if (pb != null)
                        pb.Value = pb.Maximum;
                    System.Windows.Forms.Application.DoEvents();
                    ftpStream.Close();
                    outputStream.Close();
                    response.Close();
                }
                catch (Exception ex)
                {
                    //File.Delete(localfilename);
                    throw new Exception(ex.Message);
                }
            }
        }
        /// <summary>
        /// 获取指定FTP文件大小

        /// </summary>
        /// <param name="ftpfileName"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public long GetFileSize(string ftpfileName)
        {
            long fileSize = 0;
            try
            {
                FtpWebRequest reqFTP;
                reqFTP = (FtpWebRequest)FtpWebRequest.Create(new Uri(ftpURI + ftpfileName));
                reqFTP.Credentials = new NetworkCredential(ftpUserID, ftpPassword);
                reqFTP.Method = WebRequestMethods.Ftp.GetFileSize;
                reqFTP.UseBinary = true;
                FtpWebResponse response = (FtpWebResponse)reqFTP.GetResponse();
                Stream ftpStream = response.GetResponseStream();
                fileSize = response.ContentLength;
                ftpStream.Close();
                response.Close();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return fileSize;
        }
    }
}

