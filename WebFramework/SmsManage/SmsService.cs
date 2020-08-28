//using Core;
//using Core.Utilities;
//using DataLayer.SSOT;
//using Microsoft.Extensions.Configuration;
//using PayamakPanel.Core;
//using System;
//using System.Collections.Generic;

//namespace WebFramework.SmsManage
//{
//    public class SmsService
//    {
//        private readonly FaraApi _faraApi;
//        private readonly SmsSettings _siteSetting;

//        public SmsService(FaraApi faraApi, IConfiguration configuration)
//        {
//            _faraApi = faraApi;
//            _siteSetting = configuration.GetSection(nameof(SmsSettings)).Get<SmsSettings>();
//        }

//        /// <summary>
//        /// متد ارسال پیامک
//        /// </summary>
//        /// <param name="to">دریافت کننده</param>
//        /// <param name="message">متن پیامک</param>
//        /// <returns></returns>
//        public SweetAlertExtenstion SendSms(string to, string message)
//        {
//            try
//            {
//                var model = _faraApi.SendSms
//                            (_siteSetting.UserName, _siteSetting.Password
//                            , _siteSetting.Number, to, message);

//                return model.RetStatus == 35 ? SweetAlertExtenstion.Error("اطلاعات وارد شده نادرست است") : SweetAlertExtenstion.Ok();
//            }
//            catch (Exception)
//            {
//                return SweetAlertExtenstion.Error("پیامک ارسال نشد!! خطای غیرمنتظره ای رخ داد لطفا پس از چند لحظه دوباره امتحان کنید و در صورت برطرف نشدن مشکل با پشتیبانی تماس بگیرید");
//            }
//        }

//        /// <summary>
//        /// ارسال پیامک به صورت گروهی
//        /// </summary>
//        /// <param name="phoneNumbers">لیستی از شماره تلفن ها</param>
//        /// <param name="message">متن پیامک</param>
//        /// <returns></returns>
//        public SweetAlertExtenstion SendSmsRange(List<string> phoneNumbers, string message)
//        {
//            try
//            {
//                foreach (var to in phoneNumbers)
//                {
//                    var model = _faraApi.SendSms
//                                (_siteSetting.UserName, _siteSetting.Password
//                                , _siteSetting.Number, to, message);
//                }

//                return SweetAlertExtenstion.Ok();
//            }
//            catch (Exception)
//            {
//                return SweetAlertExtenstion.Error("پیامک ارسال نشد!! خطای غیرمنتظره ای رخ داد لطفا پس از چند لحظه دوباره امتحان کنید و در صورت برطرف نشدن مشکل با پشتیبانی تماس بگیرید");
//            }
//        }

//        /// <summary>
//        /// اعتبار باقی مانده
//        /// </summary>
//        /// <returns></returns>
//        public string Credit()
//        {
//            var model = _faraApi.GetMyCredit(_siteSetting.UserName, _siteSetting.Password);

//            return model.Value;
//        }

//        /// <summary>
//        /// مشاهده تمامی پیام های ارسالی/دریافتی
//        /// </summary>
//        /// <param name="type">دریافت/ارسال</param>
//        /// <returns></returns>
//        public MessageList AllSms(SmsTypeSSOT type = SmsTypeSSOT.Send)
//        {
//            return _faraApi.GetMyMessageList(_siteSetting.UserName, _siteSetting.Password, (int)type, 0, 100);
//        }


//        /// <summary>
//        /// مبلغ باقی مانده
//        /// </summary>
//        /// <returns></returns>
//        public string PriceRemain()
//        {
//            var model = _faraApi.GetBasePrice(_siteSetting.UserName, _siteSetting.Password);

//            return model.Value;
//        }

//        public void sendTest()
//        {
            
//        }
//    }
//}
