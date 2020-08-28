using System;
using System.Collections.Generic;
using System.Text;

namespace Core.BankCommon.ViewModels
{
    public class BankVerifyViewModel
    {
        public string OrderId { get; set; }
        public string Token { get; set; }
        public string ResCode { get; set; }
        public BankCallBackResultViewModel VerifyResultData { get; set; }
    }
}
