using FreshMvvm;
using System.Collections.Generic;

namespace MycoDemo
{
    public class MycoPagingViewPageModel : FreshBasePageModel
    {
        #region Constructors

        public MycoPagingViewPageModel()
        {
            for (int i = 0; i < 5; i++)
            {
                Items.Add(new PageContentItem { Text = i.ToString() });
            }
        }

        #endregion Constructors

        #region Properties

        public List<PageContentItem> Items { get; } = new List<PageContentItem>();

        #endregion Properties

        #region Classes

        public class PageContentItem
        {
            #region Properties

            public string Text { get; set; }

            #endregion Properties
        }

        #endregion Classes
    }
}