using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Cryptography;

namespace Api.Managers
{
    public abstract class DownloadManager
    {

        private static List<DownloadResource> resources = new List<DownloadResource>();
        
        public static string AddFile(string fileName, IPAddress userIp)
        {
            var entry = new DownloadResource(fileName, userIp);
            resources.Add(entry);
            return entry.HashValue;
        }

        public static DownloadResource GetResource(string hashValue)
        {
            DownloadResource result = null;
            List<DownloadResource> outdatedResources = new List<DownloadResource>();

            foreach (DownloadResource entry in resources)
            {
                // expired?
                if (DateTime.Now > entry.Expires)
                    outdatedResources.Add(entry);
                else  if (entry.HashValue.Equals(hashValue))
                    result = entry;
            }
            if (outdatedResources.Count() != 0)
                resources.RemoveAll(r => outdatedResources.Contains(r));
            
            return result;
        }


        public class DownloadResource {

            public DownloadResource(string fileName, IPAddress userIp)
            {
                this.FileName = fileName;
                this.UserIp = userIp;

                using (var md5 = MD5.Create())
                {
                    var hash = md5.ComputeHash(System.Text.Encoding.UTF8.GetBytes(fileName + DateTime.Now));
                    this.HashValue = string.Concat(hash.Select(x => x.ToString("X2")));
                }
                this.Expires = DateTime.Now.AddMinutes(30);
            }

            public string FileName { get; }

            public string HashValue { get; }

            public IPAddress UserIp { get; }

            public bool IsSameUser(IPAddress userIp)
            {
                return UserIp.Equals(userIp);
            }

            public DateTime Expires { get; }

            public bool IsExpired()
            {
                return DateTime.Now > Expires;
            }
        }

    }
}
