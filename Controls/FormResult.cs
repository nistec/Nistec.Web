using Nistec.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace Nistec.Web.Controls
{
    public class FormResult
    {
        public FormResult() { Target = "alert"; }

        public FormResult(int status, string lang = "he")
        {
            Target = "alert";
            Status = status;
            Message = GetMessage(status, lang);
        }
        public FormResult(int res, string action, int outputIdentity=0, string lang = "he")
        {
            if (res > 1) res = 1;

            Status = res;
            Message = GetResultMessage(res, action, lang);
            Title = action;
            OutputId = outputIdentity;
        }
        public FormResult(int res, string action, string reason, string lang = "he")
        {
            if (res > 1) res = 1;

            Status = res;
            string prefix = (lang == "he") ? "שגיאה: " : "Error: ";
            Message = GetResultMessage(res, action, lang) + ", " + prefix + reason;
            Title = action;
            OutputId = 0;
        }

        public static FormResult GetFormResult(int res, string action, int outputIdentity = 0, string lang = "he")
        {
            return new FormResult(res, action, outputIdentity , lang);
        }
        public static FormResult GetFormResult(int res, string action, string reason, string lang = "he")
        {
            return new FormResult(res, action, reason, lang);
        }

        public static string GetMessage(int status, string lang = "he")
        {

            if (lang == "he")
            {
                if (status == 401)
                    return "משתמש אינו מורשה";
                else if (status == 0)
                    return "לא עודכנו נתונים";
                else if (status > 0)
                    return "עודכן בהצלחה";
                else if (status < 0)
                    return "אירעה שגיאה, הנתונים לא עודכנו";
                else
                    return "לא נמצאו נתונים";
            }
            else
            {
                if (status == 401)
                    return "Unauthorized";
                else if (status == 0)
                    return "Data not updated";
                else if (status > 0)
                    return "Updated successfully";
                else if (status < 0)
                    return "Error occurred, Data not update";
                else
                    return "Data not found";
            }
        }

        public static string GetResultMessage(int status, string action, string lang = "he")
        {
            string supix = "";

            if (lang == "he")
            {

                if (status == 401)
                    supix= "משתמש אינו מורשה";
                else if (status == 0)
                    supix = "לא בוצע";
                else if (status > 0)
                    supix = "בוצע בהצלחה";
                else if (status < 0)
                    supix = "אירעה שגיאה, לא בוצע";
                else
                    supix = "לא נמצאו נתונים";
            }
            else
            {
                if (status == 401)
                    supix = "Unauthorized";
                else if (status == 0)
                    supix = "Nothing was done";
                else if (status > 0)
                    supix = "Done successfully";
                else if (status < 0)
                    supix = "Error occurred, Done failed";
                else
                    supix = "Data not found";
            }
            return action + " " + supix;
        }
        public static string GetResultMessage(bool result, string action, string lang = "he")
        {
            string supix = "";

            if (lang == "he")
            {
                supix = (result) ? "בוצע בהצלחה" : "לא בוצע";
            }
            else
            {
                supix = (result) ? "Done successfully" : "Done failed";
            }
            return action + " " + supix;
        }

       


        //protected void Load(int status)
        //{
        //    Target = "alert";
        //    Status = status;
        //    if (status == 401)
        //        Message = "משתמש אינו מורשה";
        //    else if (status == 0)
        //        Message = "לא עודכנו נתונים";
        //    else if (status > 0)
        //        Message = "עודכן בהצלחה";
        //    else if (status < 0)
        //        Message = "אירעה שגיאה, הנתונים לא עודכנו";
        //}

        public int Status { get; set; }
        public string Title { get; set; }
        public string Message { get; set; }
        public string Link { get; set; }
        public int OutputId { get; set; }
        public string Args { get; set; }
        public string Target { get; set; }

        public static FormResult GetLang(EntityCommandResult res, string title, string lang = "he")
        {
            var model = new FormResult(res.AffectedRecords > 1 ? 1 : res.AffectedRecords, lang) { Title = title, OutputId = res.GetIdentityValue<int>() };
            return model;
        }

        public static FormResult Get(EntityCommandResult res, string title)
        {
            string message = GetResultMessage(res.Status, title);// GetLang(res, title, "he");
            var model = new FormResult() { Status = res.AffectedRecords > 1 ? 1 : res.AffectedRecords, Title = title, Message = message, Link = null, OutputId = res.GetIdentityValue<int>() };
            return model;
        }

        public static FormResult Get(EntityCommandResult res, string title, string message)
        {
            if (message == null)
                message = GetResultMessage(res.Status, title);// GetLang(res, title, "he");
            var model = new FormResult() { Status = res.AffectedRecords > 1 ? 1 : res.AffectedRecords, Title = title, Message = message, Link = null, OutputId = res.GetIdentityValue<int>() };
            return model;
        }

        public static FormResult Get(int status, string title, int outputIdentity = 0, string lang = "he")
        {
            var model = new FormResult(status, lang) { Title = title, OutputId = outputIdentity };
            return model;
        }
        public static FormResult Get(int status, string title, string message, int outputIdentity = 0)
        {
            if (message == null)
                message = GetResultMessage(status, title);
                var model = new FormResult() { Status = status, Title = title, Message = message, Link = null, OutputId = outputIdentity };
            return model;
        }
        public static FormResult Get(int status, string title, string success, string failed, int outputIdentity = 0)
        {
            string message = status > 0 ? success : failed;
            var model = new FormResult() { Status = status, Title = title, Message = message, Link = null, OutputId = outputIdentity };
            return model;
        }
        public static FormResult GetError(string action, string reason)
        {
            if (reason == null)
                reason = GetResultMessage(-1, action);
            var model = new FormResult() { Status = -1, Title = action, Message = reason };
            return model;
        }
        public static FormResult GetTrace<Dbc>(Exception ex, string action, HttpRequestBase request) where Dbc : IDbContext
        {
            string   reason = GetMessage(-1);
            TraceHelper<Dbc>.LogAsync("App", action, ex.Message, request);
            var model = new FormResult() { Status = -1, Title = action, Message = reason };
            return model;
        }
        public static FormResult GetTrace<Dbc>(Exception ex, string action, string reason, HttpRequestBase request) where Dbc : IDbContext
        {
            if (reason == null)
                reason = GetMessage(-1);
            TraceHelper<Dbc>.LogAsync("App", action, ex.Message, request);
            var model = new FormResult() { Status = -1, Title = action, Message = reason };
            return model;
        }

        //public static EntityResultModel Get(int status, string method, string action, int outputIdentity = 0)
        //{
        //    string title = action ?? "";
        //    string message = ResultStatusModel.GetResult(status, method, action);
        //    //string buttonTrigger = "<input type=\"button\" id=\"btnTrigger\" value=\"המשך\"/>";
        //    string link = null;
        //    //if (status > 0)
        //    //{
        //    //    link = buttonTrigger;
        //    //}
        //    var model = new EntityResultModel() { Status = status, Title = title, Message = message, Link = link, OutputId = outputIdentity };
        //    return model;
        //}
        //public static EntityResultModel GetFormResult(int status, string action, string reason, int outputIdentity = 0)
        //{
        //    string title = action ?? "";
        //    string message = ResultStatusModel.GetResult(status, null, action);
        //    if (reason != null)
        //        message = message + " - " + reason;
        //    //string buttonTrigger = "<input type=\"button\" id=\"btnTrigger\" value=\"המשך\"/>";
        //    string link = null;
        //    //if (status > 0)
        //    //{
        //    //    link = buttonTrigger;
        //    //}
        //    var model = new EntityResultModel() { Status = status, Title = title, Message = message, Link = link, OutputId = outputIdentity };
        //    return model;
        //}
    }
}
