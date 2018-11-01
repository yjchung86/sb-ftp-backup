using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace sb_backup
{
    public class FtpSslClient 
    {
        //member variables
        private string _host;
        private string _userName;
        private string _password;

        public FtpSslClient(string host, string userName, string password)
        {
            _host = host;
            _userName = userName;
            _password = password;
        }
        /// <summary>
        /// Upload a PC file to the mainframe in ASCII mode
        /// </summary>
        /// <param name="pcFile">Full path of the file to upload</param>
        /// <param name="mfFile">Mainframe path enclosed by single quote</param>
        public void Upload(string pcFile, string mfFile)
        {
            var request = GetRequest(GetUri(mfFile), WebRequestMethods.Ftp.UploadFile);

            UploadStream(request, pcFile);

            using (var response = (FtpWebResponse)request.GetResponse())
            {
                if (response == null)
                    throw new Exception("No response from the FTP server.");

                response.Close();
            }
        }

        /// <summary>
        /// Download a file from the mainframe in ASCII mode
        /// </summary>
        /// <param name="mfFile">Mainframe path enclosed by single quote</param>
        /// <param name="pcFile">Full path of the file to download</param>
        public void Download(string mfFile, string pcFile)
        {
            var request = GetRequest(GetUri(mfFile), WebRequestMethods.Ftp.DownloadFile);

            using (var response = (FtpWebResponse)request.GetResponse())
            {
                if (response == null)
                    throw new Exception("No response from the FTP server.");

                DownloadStream(response, pcFile);

                response.Close();
            }
        }

        /// <summary>
        /// Delete a file from the mainframe
        /// </summary>
        /// <param name="mfFile">Mainframe path enclosed by single quote</param>
        public void Delete(string mfFile)
        {
            var request = GetRequest(GetUri(mfFile), WebRequestMethods.Ftp.DeleteFile);

            var response = (FtpWebResponse)request.GetResponse();

            if (response == null)
                throw new Exception("No response from the FTP server.");

            response.Close();
        }

        private FtpWebRequest GetRequest(Uri uri, string method)
        {
            var request = (FtpWebRequest)FtpWebRequest.Create(uri);
            request.Credentials = new NetworkCredential(_userName, _password);
            request.EnableSsl = true;
            request.UseBinary = false;
            request.UsePassive = true;
            request.KeepAlive = true;
            request.Method = method;

            ServicePointManager.ServerCertificateValidationCallback = ServerCertificateValidationCallback;

            return request;
        }

        private static bool ServerCertificateValidationCallback(object sender,
            X509Certificate certificate, X509Chain chain, SslPolicyErrors errors)
        {
            if (errors != SslPolicyErrors.None)
            {
                if ((errors & SslPolicyErrors.RemoteCertificateChainErrors) == SslPolicyErrors.RemoteCertificateChainErrors)
                {
                    foreach (X509ChainStatus chainStatus in chain.ChainStatus)
                    {
                        if (chainStatus.Status == X509ChainStatusFlags.Revoked)
                        {
                            return false;
                        }
                    }
                }

                if ((errors & SslPolicyErrors.RemoteCertificateNotAvailable) == SslPolicyErrors.RemoteCertificateNotAvailable)
                {
                    return false;
                }
            }

            return true;
        }

        private Uri GetUri(string mfFile)
        {
            return new Uri(string.Format("ftp://{0}/{1}", _host, mfFile));
        }

        private void DownloadStream(FtpWebResponse response, string pcFile)
        {
            using (var sr = response.GetResponseStream())
            using (var sw = File.Create(pcFile))
            {
                sr.CopyTo(sw);
            }
        }

        private void UploadStream(FtpWebRequest request, string pcFile)
        {
            using (var sr = File.OpenRead(pcFile))
            using (var sw = request.GetRequestStream())
            {
                sr.CopyTo(sw);
            }
        }
    }
}

