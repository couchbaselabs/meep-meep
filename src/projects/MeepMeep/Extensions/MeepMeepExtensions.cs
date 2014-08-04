using System;
using Couchbase.Configuration;

namespace MeepMeep.Extensions
{
    public static class MeepMeepExtensions
    {
        public static ICouchbaseClientConfiguration ToClientConfig(this MeepMeepOptions options)
        {
            var config = new CouchbaseClientConfiguration
            {
                Bucket = options.Bucket,
                BucketPassword = options.BucketPassword,
                Username = options.ClusterUsername,
                Password = options.ClusterPassword
            };

            foreach (var node in options.Nodes)
                config.Urls.Add(new Uri(node));

            return config;
        }
    }
}
