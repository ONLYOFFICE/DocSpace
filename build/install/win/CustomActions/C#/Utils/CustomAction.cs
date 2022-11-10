using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using System.Net.Sockets;
using Microsoft.Deployment.WindowsInstaller;
using RabbitMQ.Client;

namespace Utils
{
    public class CustomActions
    {
        public static string CreateAuthToken(string pkey, string machinekey)
        {
            using (var hasher = new System.Security.Cryptography.HMACSHA1(Encoding.UTF8.GetBytes(machinekey)))
            {
                var now = DateTime.UtcNow.ToString("yyyyMMddHHmmss");
                var hash = System.Web.HttpServerUtility.UrlTokenEncode(hasher.ComputeHash(Encoding.UTF8.GetBytes(string.Join("\n", now, pkey))));
                return string.Format("ASC {0}:{1}:{2}", pkey, now, hash);
            }
        }

        [CustomAction]
        public static ActionResult SetVersionReleaseDateSign(Session session)
        {
            var pkey = Convert.ToString(session["VERSION.RELEASE_DATE"]);
            var machineKey = Convert.ToString(session["MACHINE_KEY"]);

            session.Log("SetVersionReleaseDateSign: pkey {0}, machineKey {1}", pkey, machineKey);

            session["VERSION.RELEASE_DATE.SIGN"] = CreateAuthToken(pkey, machineKey);

            session.Log("SetVersionReleaseDateSign End: {0}", session["VERSION.RELEASE_DATE.SIGN"]);
            
            return ActionResult.Success;           
        }


        [CustomAction]
        public static ActionResult TestRedisServerConnection(Session session)
        {
            try
            {
                using (var redis = new Redis(session["REDIS_HOST"], Convert.ToInt32(session["REDIS_PORT"])))
                {

                    if (!String.IsNullOrEmpty(session["REDIS_PWD"].Trim()))
                        redis.Password = session["REDIS_PWD"];

                    var pong = redis.Ping("ONLYOFFICE");

                    session.Log("Redis Status: IsConnected is {0}", !String.IsNullOrEmpty(pong));
                    session["RedisServerConnectionError"] = !String.IsNullOrEmpty(pong) ? "" : String.Format("Connection Refused HOST:{0},PORT:{1},PASS:{2}", session["REDIS_HOST"], session["REDIS_PORT"], session["REDIS_PWD"]);
                                                       
                }
            }
            catch (Exception ex)
            {
                session.Log("RedisConnectionException '{0}'", ex.Message);
                session["RedisServerConnectionError"] = String.Format("Connection Refused HOST:{0},PORT:{1},PASS:{2}", session["REDIS_HOST"], session["REDIS_PORT"], session["REDIS_PWD"]);
            }

            return ActionResult.Success; 
        }


        [CustomAction]
        public static ActionResult TestRabbitMQConnection(Session session)
        {
            ConnectionFactory factory = new ConnectionFactory();

            factory.HostName = session["AMQP_HOST"];
            factory.Port = Convert.ToInt32(session["AMQP_PORT"]);
            factory.VirtualHost = session["AMQP_VHOST"];
            factory.UserName = session["AMQP_USER"];
            factory.Password = session["AMQP_PWD"];

            try
            {
                using (IConnection conn = factory.CreateConnection())
                {
                    session.Log("RabbitMQ Status: IsConnected is {0}", conn.IsOpen);

                    session["RabbitMQServerConnectionError"] = conn.IsOpen ? "" : String.Format("Connection Refused HOST:{0}, PORT:{1}, VirtualHost:{2}, UserName:{3}, PASS:{4}",
                        session["AMQP_HOST"],
                        session["AMQP_PORT"],
                        session["AMQP_VHOST"],
                        session["AMQP_USER"],
                        session["AMQP_PWD"]
                        );
                }
            }
            catch (Exception ex)
            {

                session.Log("RabbitMQ.Client.Exceptions.BrokerUnreachableException {0}", ex.Message);
                session["RabbitMQServerConnectionError"] = String.Format("Connection Refused HOST:{0}, PORT:{1}, VirtualHost:{2}, UserName:{3}, PASS:{4}",
                        session["AMQP_HOST"],
                        session["AMQP_PORT"],
                        session["AMQP_VHOST"],
                        session["AMQP_USER"],
                        session["AMQP_PWD"]
                        );

            }

            return ActionResult.Success;

        }

        [CustomAction]
        public static ActionResult CheckTCPAvailability(Session session)
        {
            string HOST = session.CustomActionData["HOST"];
            string PORT = session.CustomActionData["PORT"];
            string OUTPUT = session.CustomActionData["OUTPUT"];
            var success = new TcpClient().BeginConnect(HOST, Convert.ToInt32(PORT), null, null).AsyncWaitHandle.WaitOne(TimeSpan.FromSeconds(1));
            session[OUTPUT] = success.ToString();
            return ActionResult.Success; 
        }
    }
}
