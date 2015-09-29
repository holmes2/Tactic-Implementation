using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using Bnk.Log.Monitor.Areas.HeartBeat;

namespace Bnk.Log.Monitor
{
    public partial class Service1 : ServiceBase
    {
        HeartBeatReceiver m_receive;
        public Service1()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            m_receive = new HeartBeatReceiver();
        }

        protected override void OnStop()
        {
        }
    }
}
