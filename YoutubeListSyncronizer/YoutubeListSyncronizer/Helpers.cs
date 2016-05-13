using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace YoutubeListSyncronizer
{
    public static class Helpers
    {
        public static void UpdateAndRedrawForm(Form form)
        {
            form.Invalidate();
            form.Update();
            form.Refresh();
            Application.DoEvents();
        }

        public static void ToggleChecked(this ListView listView)
        {
            for (int i = 0; i < listView.Items.Count; i++)
            {
                listView.Items[i].Checked = !listView.Items[i].Checked;
            }
        }


    }
}
