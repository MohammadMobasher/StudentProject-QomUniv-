using Core.BankCommon.Commons;
using System;
using System.Collections.Generic;
using System.Text;

namespace Core.BankCommon.ViewModels
{
    public class BankPaymentViewModel
    {

        public string TerminalId { get; set; } = "24000615";
        public string MerchantId { get; set; } = "000000140212149";
        public long Amount { get; set; } = 1000;
        public string OrderId { get; set; }
        public string AdditionalData { get; set; }
        public DateTime LocalDateTime { get; set; } = DateTime.Now;
        public string ReturnUrl { get; set; }
        public string SignData { get; set; }
        public bool EnableMultiplexing { get; set; }
        public MultiplexingData MultiplexingData { get; set; }
        public string MerchantKey { get; set; } = "kLheA+FS7MLoLlLVESE3v3/FP07uLaRw";
        public string PurchasePage { get; set; } = "https://sadad.shaparak.ir";

    }
}
