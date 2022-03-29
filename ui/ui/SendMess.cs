using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ui
{
    class SendMess
    {
        public void Run(String s)
        {
            Thread t = new Thread(() => Send(s));
            t.Start();
            if (t.Join(4 * 1000) == false)
            {
                t.Abort();
            }
        }
        public void Send(String s)
        {
            SendKeys.SendWait(s);
        }
    }
}
