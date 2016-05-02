using FreshMvvm;
using System;
using System.Collections.Generic;
using Xamarin.Forms;

namespace MycoDemo
{
    public class MycoDemoPageModel : FreshBasePageModel
    {
        #region Constructors

        public MycoDemoPageModel()
        {
            Random r = new Random(12345);

            for (int i = 0; i < 100; i++)
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

        #endregion Constructors

        #region Properties

        public Command FolderCommand
        {
            get
            {
                return new Command(() =>
                {
                });
            }
        }

        public List<ListItem> Items { get; } = new List<ListItem>();

        #endregion Properties

        #region Classes

        public class LabelItem
        {
            #region Properties

            public bool Enabled { get; set; }
            public string Header { get; set; }
            public string Value { get; set; }

            #endregion Properties
        }

        public class ListItem
        {
            #region Properties

            public List<LabelItem> Labels { get; set; } = new List<LabelItem>();
            public string Text { get; set; }

            #endregion Properties
        }

        #endregion Classes
    }
}