using FreshMvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace MycoDemo
{
    public class MycoDemoPageModel : FreshBasePageModel
    {
        public MycoDemoPageModel()
        {
            Random r = new Random(12345);

            for (int i = 0;  i < 100; i++)
            {
                var item = new ListItem { Text = i.ToString() };

                var count = r.Next(8);

                for (int j = 0; j < 8; j++)
                {
                    if (j < count)
                    {
                        item.Labels.Add(new LabelItem { Enabled = true, Header = "Header " + j.ToString(), Value = "Value " + j.ToString() });
                    }
                    else
                    {
                        item.Labels.Add(new LabelItem { Enabled = false });
                    }
                }

                Items.Add(item);
            }
        }

        public class ListItem
        {
            public string Text { get; set; }

            public List<LabelItem> Labels { get; set; } = new List<LabelItem>();
        }

        public class LabelItem
        {
            public bool Enabled { get; set; }
            public string Header { get; set; }
            public string Value { get; set; }
        }

        public List<ListItem> Items { get; } = new List<ListItem>();

        public Command FolderCommand
        {
            get
            {
                return new Command(() =>
                {
                    
                });
            }
        }
    }
}
