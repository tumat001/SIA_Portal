using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SIA_Portal.Models.ObjectRepresentations
{
    public class EmployeeAccountRepresentation
    {
        public string Username { set; get; }

        public int Id { set; get; }

        public bool DisabledFromLogIn { set; get; }

        public bool IsSelected { set; get; }

    }
}