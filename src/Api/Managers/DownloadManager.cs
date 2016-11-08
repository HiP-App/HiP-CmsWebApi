using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace Api.Managers
{
    public abstract class DownloadManager
    {

        private static List<DownloadResource> resources = new List<DownloadResource>();
        
        public static string AddFile(string fileName, IPAddress userIp)
        {
            var entry = new DownloadResource(fileName, userIp);
            return entry.HashValue;
        }

        public static DownloadResource GetResource(string hashValue, IPAddress userIp)
        {
            foreach (DownloadResource entry in resources)
            {
                // expired?
                if (DateTime.Now > entry.Expires)
                    resources.Remove(entry);
                else if (entry.HashValue.Equals(hashValue) && entry.UserIp.Equals(userIp))
                    return entry;
            }
            return null;
        }


        public class DownloadResource {

            public DownloadResource(string fileName, IPAddress userIp)
            {
                this.FileName = fileName;
                this.UserIp = userIp;

                using (var md5 = MD5.Create())
                {
                    var hash = md5.ComputeHash(System.Text.Encoding.UTF8.GetBytes(fileName + DateTime.Now));
                    this.HashValue = System.Text.Encoding.UTF8.GetString(hash);
                }
                this.Expires = DateTime.Now.AddMinutes(30);
            }

            public string FileName { get; }

            public string HashValue { get; }

            public IPAddress UserIp { get; }

            public DateTime Expires { get; }
        }

    }
}
