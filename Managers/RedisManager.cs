using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServerShenkar.Managers
{
    internal class RedisManager
    {
        private static string endPoint = "redis-10360.c55.eu-central-1-1.ec2.cloud.redislabs.com";
        private static string password = "Xg25akGPbWK8z0jQeCHDO3X8N73KLwqb";
        private static int port = 10360;

        private static Lazy<ConnectionMultiplexer> lazyConnectionMultiplexer;
        static RedisManager()
        {
            RedisManager.lazyConnectionMultiplexer = new Lazy<ConnectionMultiplexer>(() =>
            {
                var configurationOptions = new ConfigurationOptions
                {
                    AbortOnConnectFail = false,
                    EndPoints = { { endPoint, port } }
                    ,
                    Password = password
                    ,
                    ConnectTimeout = 10000
                };
                return ConnectionMultiplexer.Connect(configurationOptions);
            });
        }

        public static ConnectionMultiplexer connection
        {
            get
            {
                return lazyConnectionMultiplexer.Value;
            }
        }
    }
}
