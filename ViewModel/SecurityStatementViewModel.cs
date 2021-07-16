using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Update;
using MSN.Model;
using MSN.ModelContext;
using MSN.ViewModel.Util;

namespace MSN.ViewModel
{
    public class SecurityStatementViewModel
    {
        public string OldSecurityStatement { get; set; }
        public string NewSecurityStatement { get; set; }
        public string ConfirmNewSecurityStatement { get; set; }
    }
}