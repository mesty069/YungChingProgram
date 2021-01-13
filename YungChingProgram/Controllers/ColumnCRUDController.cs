using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using YungChingProgram._GeneralLibrary;
using YungChingProgram.Models;
using YungChingProgram.Models.Database;
using YungChingProgram.Servicves;
using YungChingProgram.ViewModels;

namespace YungChingProgram.Controllers
{
    public class ColumnCRUDController : Controller
    {
        private static readonly LogManagement Log = new LogManagement();
        private readonly ColumnCRUDService _service = new ColumnCRUDService();
        private string userName = "admin";
        public ActionResult Index()
        {
            try
            {
                Log.LogInfoWriter(userName, LogManagement.LogType.OP, LogManagement.EventLevel.Info, LogManagement.LogAction.USER_Trace_EnterPage, LogManagement.SystemName.基本作業, "使用者取得ColumnCRUD人員維護畫面", null, null);
                Log.LogInfoWriter(userName, LogManagement.LogType.AP, LogManagement.EventLevel.Info, LogManagement.LogAction.SYS_FunctionStart, LogManagement.SystemName.基本作業, "開始取得ColumnCRUD人員類別選單", null, null);
                ColumnCRUDViewModel columnCRUDViewModel = new ColumnCRUDViewModel();
                var selectList = _service.GetTypeSelectList();
                selectList.Insert(0, new SelectListItem { Value = "", Text = "請選擇" });
                columnCRUDViewModel.TypeSelectList = new SelectList(selectList, "Value", "Text");
                Log.LogInfoWriter(userName, LogManagement.LogType.AP, LogManagement.EventLevel.Info, LogManagement.LogAction.SYS_FunctionEnd, LogManagement.SystemName.基本作業, "取得ColumnCRUD人員類別選單結束", null, null);
                return View(columnCRUDViewModel);
            }
            catch (Exception ex)
            {
                Log.LogInfoWriter(userName, LogManagement.LogType.AP, LogManagement.EventLevel.Error, LogManagement.LogAction.Error_FunctionError, LogManagement.SystemName.基本作業, "取得ColumnCRUD人員類別選單發生錯誤", null, ex);
                return View();
            }
        }

        public PartialViewResult ColumnCRUDPartialView(string name, string type)
        {
            try
            {
                Log.LogInfoWriter(userName, LogManagement.LogType.OP, LogManagement.EventLevel.Info, LogManagement.LogAction.USER_Action_Query, LogManagement.SystemName.基本作業, "使用者取得ColumnCRUD人員維護畫面", new { name, type }, null);
                Log.LogInfoWriter(userName, LogManagement.LogType.AP, LogManagement.EventLevel.Info, LogManagement.LogAction.SYS_FunctionStart, LogManagement.SystemName.基本作業, "開始取得ColumnCRUD人員類別選單", new { name, type }, null);
                ColumnCRUDPartialViewModel columnCRUDPartialViewModel = new ColumnCRUDPartialViewModel();
                columnCRUDPartialViewModel.ColumnCRUDDataModelList = _service.GetColumnCRUDDataList(name, type);
                var selectList = _service.GetTypeSelectList();
                selectList.Insert(0, new SelectListItem { Value = "", Text = "請選擇" });
                columnCRUDPartialViewModel.TypeSelectList = new SelectList(selectList, "Value", "Text");
                if (columnCRUDPartialViewModel.ColumnCRUDDataModelList == null)
                {
                    Log.LogInfoWriter(userName, LogManagement.LogType.AP, LogManagement.EventLevel.Error, LogManagement.LogAction.Error_FunctionError, LogManagement.SystemName.基本作業, "取得ColumnCRUD人員類別選單時，取得資料為Null", new { name, type }, null);
                    return PartialView(new ColumnCRUDPartialViewModel());
                }
                Log.LogInfoWriter(userName, LogManagement.LogType.AP, LogManagement.EventLevel.Info, LogManagement.LogAction.SYS_FunctionEnd, LogManagement.SystemName.基本作業, "開始取得ColumnCRUD人員類別選單", columnCRUDPartialViewModel, null);
                return PartialView(columnCRUDPartialViewModel);
            }
            catch (Exception ex)
            {
                return PartialView(new ColumnCRUDPartialViewModel());
            }
        }

        /// <summary>
        /// 新增或資料
        /// </summary>
        /// <returns></returns>
        public JsonResult InsertColumnCRUDData(ColumnCRUD columnCRUDData)
        {
            try
            {
                //判斷PKEY是否有填寫
                if (string.IsNullOrEmpty(columnCRUDData.Id))
                {
                    Log.LogInfoWriter(userName, LogManagement.LogType.AP, LogManagement.EventLevel.Error, LogManagement.LogAction.Error_NullException, LogManagement.SystemName.基本作業, "傳入新增ColumnCRUD參數的Pkey為Null", columnCRUDData.Id, null);
                    return Json(new { result = "人員代號為必填欄位" });
                }
                Log.LogInfoWriter(userName, LogManagement.LogType.OP, LogManagement.EventLevel.Info, LogManagement.LogAction.USER_Action_Insert, LogManagement.SystemName.基本作業, "使用者新增一筆ColumnCRUD資料", columnCRUDData, null);
                Log.LogInfoWriter(userName, LogManagement.LogType.AP, LogManagement.EventLevel.Info, LogManagement.LogAction.SYS_FunctionStart, LogManagement.SystemName.基本作業, "開始新增一筆ColumnCRUD資料", columnCRUDData, null);
                var result = _service.InsertColumnCRUDData(columnCRUDData);
                Log.LogInfoWriter(userName, LogManagement.LogType.AP, LogManagement.EventLevel.Info, LogManagement.LogAction.SYS_FunctionEnd, LogManagement.SystemName.基本作業, "新增一筆ColumnCRUD資料結束", columnCRUDData, null);
                return Json(new { result = result });
            }
            catch (Exception ex)
            {
                return Json(new { result = "false" });
            }
        }

        /// <summary>
        /// 新增或修改資料
        /// </summary>
        /// <returns></returns>
        public JsonResult UpdateColumnCRUDData(ColumnCRUDDataModel columnCRUDDataModel)
        {
            try
            {
                //判斷PKEY是否有填寫
                if (string.IsNullOrEmpty(columnCRUDDataModel.Id))
                {
                    Log.LogInfoWriter(userName, LogManagement.LogType.AP, LogManagement.EventLevel.Error, LogManagement.LogAction.Error_NullException, LogManagement.SystemName.基本作業, "傳入修改ColumnCRUD參數的Pkey為Null", columnCRUDDataModel, null);
                    return Json(new { result = "false" });
                }
                Log.LogInfoWriter(userName, LogManagement.LogType.OP, LogManagement.EventLevel.Info, LogManagement.LogAction.USER_Action_Update, LogManagement.SystemName.基本作業, "使用者修改一筆ColumnCRUD資料", columnCRUDDataModel, null);
                Log.LogInfoWriter(userName, LogManagement.LogType.AP, LogManagement.EventLevel.Info, LogManagement.LogAction.SYS_FunctionStart, LogManagement.SystemName.基本作業, "開始修改一筆ColumnCRUD資料", columnCRUDDataModel, null);
                var result = _service.UpdateColumnCRUDData(columnCRUDDataModel);
                Log.LogInfoWriter(userName, LogManagement.LogType.AP, LogManagement.EventLevel.Info, LogManagement.LogAction.SYS_FunctionEnd, LogManagement.SystemName.基本作業, "修改一筆ColumnCRUD資料結束", columnCRUDDataModel, null);
                return Json(new { result = result });
            }
            catch (Exception ex)
            {
                Log.LogInfoWriter(userName, LogManagement.LogType.AP, LogManagement.EventLevel.Error, LogManagement.LogAction.Error_FunctionError, LogManagement.SystemName.基本作業, "修改一筆ColumnCRUD資料發生錯誤", columnCRUDDataModel, ex);
                return Json(new { result = "false" });
            }
        }

        /// <summary>
        /// 刪除資料
        /// </summary>
        /// <returns></returns>
        public ActionResult DeleteColumnCRUDData(string pid)
        {
            try
            {
                //判斷PKEY是否有填寫
                if (string.IsNullOrEmpty(pid))
                {
                    Log.LogInfoWriter(userName, LogManagement.LogType.AP, LogManagement.EventLevel.Error, LogManagement.LogAction.Error_NullException, LogManagement.SystemName.基本作業, "傳入刪除ColumnCRUD參數的Pkey為Null", pid, null);
                    return Json(new { result = "人員Id為空值，刪除失敗" });
                }
                Log.LogInfoWriter(userName, LogManagement.LogType.OP, LogManagement.EventLevel.Info, LogManagement.LogAction.USER_Action_Delete, LogManagement.SystemName.基本作業, "使用者刪除一筆ColumnCRUD資料", pid, null);
                Log.LogInfoWriter(userName, LogManagement.LogType.AP, LogManagement.EventLevel.Info, LogManagement.LogAction.SYS_FunctionStart, LogManagement.SystemName.基本作業, "開始刪除一筆ColumnCRUD資料", pid, null);
                var result = _service.DeleteColumnCRUDData(pid);
                Log.LogInfoWriter(userName, LogManagement.LogType.AP, LogManagement.EventLevel.Info, LogManagement.LogAction.SYS_FunctionEnd, LogManagement.SystemName.基本作業, "刪除一筆ColumnCRUD資料結束", pid, null);
                return Json(new { result = result });
            }
            catch (Exception ex)
            {
                Log.LogInfoWriter(userName, LogManagement.LogType.AP, LogManagement.EventLevel.Error, LogManagement.LogAction.Error_FunctionError, LogManagement.SystemName.基本作業, "刪除一筆ColumnCRUD資料發生錯誤", pid, ex);
                return Json(new { result = "false" });
            }
        }
    }
}