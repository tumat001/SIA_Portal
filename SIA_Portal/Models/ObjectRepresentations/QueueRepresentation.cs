using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SIA_Portal.Models.ObjectRepresentations
{
    public class QueueRepresentation
    {
        private const int PREVIEW_LIMIT = 50;


        public int Id { set; get; }


        private string _descriptionPreview;
        public string DescriptionPreview
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

                _descriptionPreview = subtext;
            }

            get
            {
                return _descriptionPreview;
            }
        }

        public int Status { set; get; }

        public string StatusAsText { set; get; }

        public bool IsSelected { set; get; }


        public string FullDescription { set; get; }

        //

        public string AssociatedReqDocuName { set; get; }


    }
}