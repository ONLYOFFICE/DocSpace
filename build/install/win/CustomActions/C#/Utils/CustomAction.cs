using System;
using System.Net.Sockets;
using Microsoft.Deployment.WindowsInstaller;

namespace Utils
{
    public class CustomActions
    {
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
