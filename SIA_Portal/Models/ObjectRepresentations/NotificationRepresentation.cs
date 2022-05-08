using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SIA_Portal.Models.ObjectRepresentations
{
    public class NotificationRepresentation
    {

        private const int PREVIEW_LIMIT = 50;


        public int Id { set; get; }


        public string Title { set; get; }


        private string _contentPreview;
        public string ContentPreview
        {
            set
            {
                string subtext = null;

                if (value != null)
                {
                    var subtextLength = value.Length;
                    if (subtextLength > PREVIEW_LIMIT)
                    {
                        subtextLength = PREVIEW_LIMIT;
                    }

                    subtext = value.Substring(0, subtextLength);
                }

                _contentPreview = subtext;
            }

            get
            {
                return _contentPreview;
            }
        }

        public string FullContent { set; get; }

        //

        public bool IsSelected { set; get; }



    }
}