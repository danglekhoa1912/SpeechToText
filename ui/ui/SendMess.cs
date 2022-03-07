using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ui
{
    class SendMess
    {
        public void send(String s)
        {
            SendKeys.SendWait(s);
        }
    }
}
