//using Core.Utilities;
//using DataLayer.SSOT;
//using Microsoft.AspNetCore.Authorization;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.AspNetCore.Mvc.Filters;
//using Service.Repos.User;
//using System;
//using System.Collections.Generic;
//using System.Security.Claims;
//using System.Text;
//using WebFramework.Authenticate;

//namespace WebFramework.Base
//{
//    [HasAccess]
//    [Authorize]
//    public class BaseAdminController : Controller
//    {
//        private readonly UsersAccessRepository _usersAccessRepository;

//        public BaseAdminController(UsersAccessRepository usersAccessRepository)
//        {
//            _usersAccessRepository = usersAccessRepository;
//        }

//        #region Fields

//        /// <summary>
//        /// شماره صفحه فعلی
//        /// </summary>
//        public int CurrentPage { get; set; } = 0;


//        /// <summary>
//        /// تعداد آیتم برای هر صفحه
//        /// </summary>
//        public int PageSize { get; set; } = 10;


//        /// <summary>
//        /// تعداد کل آیتم‌ها
//        /// </summary>
//        public int TotalNumber { get; set; }


//        /// <summary>
//        /// تعداد صفحات
//        /// </summary>
//        public int PageCount { get; set; }
//        ///// <summary>
//        ///// مقدار قبلی شماره صفحه
//        ///// از این فیلد برای جستجو استفاده میشود
//        ///// به طوری که اگر مقدار مفعلی شماره صفحه با شماره قبلی آن
//        ///// برابر نباشد، به معنی جستجو جدید است
//        ///// </summary>
//        //public int OldPageCount { get; set; }


//        public bool newSearch { get; set; }


//        public string IndexUrlWithQueryString
//        {
//            get
//            {
//                if (HttpContext.Request.Cookies["IndexUrlWithQueryString"] != null)
//                    return HttpContext.Request.Cookies["IndexUrlWithQueryString"];
//                else
//                    return "/";
//            }
//            set
//            {

//            }
//        }


//        #endregion

//        /// <summary>
//        /// شماره کاربری شخصی که لاگین است
//        /// </summary>
//        public int UserId
//        {
//            get
//            {
//                if (_userId == null)
//                    _userId = User.Identity.FindFirstValue(ClaimTypes.NameIdentifier).ToInt();
//                return _userId.Value;
//            }
//        }

//        private int? _userId { get; set; }

//        /// <summary>
//        /// شماره نقش فردی که لاگین کرده است
//        /// </summary>
//        public string Role
//        {
//            get
//            {
//                if (_roleId == null)
//                    _roleId = User.Identity.FindFirstValue(ClaimTypes.Role);

//                return _roleId;
//            }
//        }
//        private string _roleId { get; set; }

//        public BaseAdminController()
//        {

//        }

//        /// <summary>
//        /// قبل از اجرا هر اکشنی این تابع اجرا میشود تا بتواند اطلاعات مورد نیاز که 
//        /// از صفحه پاس داده شده است اینجا مقداردهی کند
//        /// </summary>
//        /// <param name="context"></param>
//        public override void OnActionExecuting(ActionExecutingContext context)
//        {
//            base.OnActionExecuting(context);

//            if (context.RouteData.Values["action"].ToString() == "Index")
//            {
//                // ================================= Develop by Jadidi ================================= //
//                context.HttpContext.Response.Cookies.Append("IndexUrlWithQueryString", context.HttpContext.Request.Path + context.HttpContext.Request.QueryString);
//            }

//            this.CurrentPage = Request.Query["currentPage"].Count != 0 ? Convert.ToInt32(Request.Query["currentPage"][0]) : 1;
//            this.newSearch = Request.Query["newSearch"].Count != 0 ? Convert.ToBoolean(Request.Query["newSearch"][0]) : false;
//            //this.OldPageCount = Request.Query["oldcurrentPage"].Count != 0 ? Convert.ToInt32(Request.Query["oldcurrentPage"][0]) : 1;

//            if (this.newSearch)
//            {
//                this.CurrentPage = 1;

//            }

//            this.PageSize = Request.Query["pageSize"].Count != 0 ? Convert.ToInt32(Request.Query["pageSize"][0]) : 10;

//        }



//        /// <summary>
//        /// بعد از هر اکشنی این تابع صدا زده می‌شود
//        /// </summary>
//        /// <param name="context"></param>
//        public override void OnActionExecuted(ActionExecutedContext context)
//        {
//            base.OnActionExecuted(context);

//            // لیست دسترسی‌های موجود برای این نقش
//            getListAccess();


//            ViewBag.CurrentPage = this.CurrentPage;
//            ViewBag.pageSize = this.PageSize;


//            ViewBag.pageCount = (int)Math.Floor((decimal)((this.TotalNumber + this.PageSize - 1) / this.PageSize));

//            ViewBag.totalNumber = this.TotalNumber;
//            //ViewBag.oldCurrentPage = this.CurrentPage;


//        }

//        /// <summary>
//        /// با استفاده از این تابع یک لیستی به صفحه پاس داده می‌شود
//        /// در این تابع لیستی از 
//        /// controller 
//        /// و
//        /// action
//        /// هایی که این نقش دارد را به صفحه پاس میدهد
//        /// </summary>
//        private void getListAccess()
//        {
//            ViewBag.ListAccess = _usersAccessRepository.GetAllUserAccesss(this.UserId);
//        }
        
//    }
//}
