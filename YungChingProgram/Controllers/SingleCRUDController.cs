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
    public class SingleCRUDController : Controller
    {
        private readonly SingleCRUDService _service = new SingleCRUDService();
        private static readonly LogManagement Log = new LogManagement();
        private string userName = "admin";
        // GET: SingleCRUD
        public ActionResult Index()
        {
            try
            {
                Log.LogInfoWriter(userName, LogManagement.LogType.OP, LogManagement.EventLevel.Info, LogManagement.LogAction.USER_Trace_EnterPage, LogManagement.SystemName.基本作業, "使用者取得SingleCRUD維護畫面", null, null);
                Log.LogInfoWriter(userName, LogManagement.LogType.AP, LogManagement.EventLevel.Info, LogManagement.LogAction.SYS_FunctionStart, LogManagement.SystemName.基本作業, "開始取得SingleCRUD類別選單", null, null);
                SingleCRUDViewModel singleCRUDViewModel = new SingleCRUDViewModel();
                var selectList = _service.GetDtypeSelecList();
                selectList.Insert(0, new SelectListItem { Text = "請選擇", Value = "" });
                singleCRUDViewModel.DTypeSelectList = new SelectList(selectList, "Value", "Text");
                Log.LogInfoWriter(userName, LogManagement.LogType.AP, LogManagement.EventLevel.Info, LogManagement.LogAction.SYS_FunctionEnd, LogManagement.SystemName.基本作業, "取得SingleCRUD類別選單結束", singleCRUDViewModel, null);
                return View(singleCRUDViewModel);
            }
            catch (Exception ex)
            {
                Log.LogInfoWriter(userName, LogManagement.LogType.AP, LogManagement.EventLevel.Error, LogManagement.LogAction.Error_FunctionError, LogManagement.SystemName.基本作業, "取得SingleCRUD類別選單發生錯誤", null, ex);
                return View();
            }
        }

        public PartialViewResult SingleCRUDPartialView(string name, string type)
        {
            try
            {
                Log.LogInfoWriter(userName, LogManagement.LogType.OP, LogManagement.EventLevel.Info, LogManagement.LogAction.USER_Action_Query, LogManagement.SystemName.基本作業, "使用者取得SingleCRUD查詢資料", null, null);
                Log.LogInfoWriter(userName, LogManagement.LogType.AP, LogManagement.EventLevel.Info, LogManagement.LogAction.SYS_FunctionStart, LogManagement.SystemName.基本作業, "開始取得SingleCRUD查詢資料", null, null);
                SingleCRUDPartialViewModel singleCRUDPartialViewModel = new SingleCRUDPartialViewModel();
                singleCRUDPartialViewModel.SingleCRUDModelList = _service.GetSingleCRUDDataList(name, type);
                if (singleCRUDPartialViewModel.SingleCRUDModelList == null)
                {
                    singleCRUDPartialViewModel.ErrorMessage = "查詢發生錯誤";
                    return PartialView(singleCRUDPartialViewModel);
                }
                Log.LogInfoWriter(userName, LogManagement.LogType.AP, LogManagement.EventLevel.Info, LogManagement.LogAction.SYS_FunctionEnd, LogManagement.SystemName.基本作業, "取得SingleCRUD查詢資料結束", null, null);
                return PartialView(singleCRUDPartialViewModel);
            }
            catch (Exception ex)
            {
                Log.LogInfoWriter(userName, LogManagement.LogType.AP, LogManagement.EventLevel.Error, LogManagement.LogAction.Error_FunctionError, LogManagement.SystemName.基本作業, "取得SingleCRUD查詢資料發生錯誤", null, ex);
                return PartialView();
            }
        }

        /// <summary>
        ///BKMBInfo維護畫面建置
        /// </summary>
        /// <param name="bkmbModel"></param>
        /// <param name="insertOrUpdate"></param>
        /// <returns></returns>
        public ActionResult SingleCRUDInfo(string id, string Mode)
        {
            try
            {
                Log.LogInfoWriter(userName, LogManagement.LogType.OP, LogManagement.EventLevel.Info, LogManagement.LogAction.USER_Trace_EnterPage, LogManagement.SystemName.基本作業, "使用者進入SingleCRUD維護畫面", null, null);
                Log.LogInfoWriter(userName, LogManagement.LogType.AP, LogManagement.EventLevel.Info, LogManagement.LogAction.SYS_FunctionStart, LogManagement.SystemName.基本作業, "開始取得SingleCRUD維護畫面", new { id, Mode }, null);
                SingleCRUDInfoViewModel singleCRUDInfoModel = new SingleCRUDInfoViewModel();
                var selectList = _service.GetDtypeSelecList();
                selectList.Insert(0, new SelectListItem { Text = "請選擇", Value = "" });
                singleCRUDInfoModel.DTypeSelectList = new SelectList(selectList, "Value", "Text");
                singleCRUDInfoModel.Mode = Mode;
                if (Mode == "Update")
                {
                    if (string.IsNullOrEmpty(id))
                    {
                        Log.LogInfoWriter(userName, LogManagement.LogType.AP, LogManagement.EventLevel.Error, LogManagement.LogAction.Error_NullException, LogManagement.SystemName.基本作業, "修改資料的Pkey為空值，修改失敗", new { id, Mode }, null);
                        singleCRUDInfoModel.ErrorMessage = "需要修改資料的代碼為空值，修改動作失敗";
                        return View(singleCRUDInfoModel);
                    }
                    singleCRUDInfoModel.SingleCRUDModel = _service.GetSingleCRUDData(id);
                    if (singleCRUDInfoModel.SingleCRUDModel == null)
                    {
                        Log.LogInfoWriter(userName, LogManagement.LogType.AP, LogManagement.EventLevel.Error, LogManagement.LogAction.Error_NullException, LogManagement.SystemName.基本作業, "查無該筆修改資料，修改失敗", new { id, Mode }, null);
                        singleCRUDInfoModel.ErrorMessage = "查無該筆修改資料，修改動作失敗";
                        return View(singleCRUDInfoModel);
                    }
                    Log.LogInfoWriter(userName, LogManagement.LogType.AP, LogManagement.EventLevel.Info, LogManagement.LogAction.SYS_FunctionEnd, LogManagement.SystemName.基本作業, "取得SingleCRUD維護畫面結束", singleCRUDInfoModel, null);
                    return View(singleCRUDInfoModel);
                }
                Log.LogInfoWriter(userName, LogManagement.LogType.AP, LogManagement.EventLevel.Info, LogManagement.LogAction.SYS_FunctionEnd, LogManagement.SystemName.基本作業, "取得SingleCRUD維護畫面結束", singleCRUDInfoModel, null);
                return View(singleCRUDInfoModel);
            }
            catch (Exception ex)
            {
                Log.LogInfoWriter(userName, LogManagement.LogType.AP, LogManagement.EventLevel.Error, LogManagement.LogAction.Error_FunctionError, LogManagement.SystemName.基本作業, "取得SingleCRUD維護畫面，發生錯誤", new { id, Mode }, ex);
                return View(new SingleCRUDInfoViewModel());
            }
        }

        /// <summary>
        /// 新增資料
        /// </summary>
        /// <returns></returns>
        public JsonResult InsertColumnCRUDData(SingleCRUD singleCRUDData)
        {
            try
            {
                //判斷PKEY是否有填寫
                if (string.IsNullOrEmpty(singleCRUDData.Id))
                {
                    Log.LogInfoWriter(userName, LogManagement.LogType.AP, LogManagement.EventLevel.Error, LogManagement.LogAction.Error_NullException, LogManagement.SystemName.基本作業, "傳入新增參數的Pkey為Null", singleCRUDData.Id, null);
                    return Json(new { result = "人員代號為必填欄位" });
                }
                Log.LogInfoWriter(userName, LogManagement.LogType.OP, LogManagement.EventLevel.Info, LogManagement.LogAction.USER_Action_Insert, LogManagement.SystemName.基本作業, "使用者新增一筆SingleCRUD資料", singleCRUDData, null);
                Log.LogInfoWriter(userName, LogManagement.LogType.AP, LogManagement.EventLevel.Info, LogManagement.LogAction.SYS_FunctionStart, LogManagement.SystemName.基本作業, "開始新增一筆SingleCRUD資料", singleCRUDData, null);
                var result = _service.InsertSingleCRUDData(singleCRUDData);
                Log.LogInfoWriter(userName, LogManagement.LogType.AP, LogManagement.EventLevel.Info, LogManagement.LogAction.SYS_FunctionEnd, LogManagement.SystemName.基本作業, "新增一筆資料SingleCRUD結束", singleCRUDData, null);
                return Json(new { result = result });
            }
            catch (Exception ex)
            {
                Log.LogInfoWriter(userName, LogManagement.LogType.AP, LogManagement.EventLevel.Error, LogManagement.LogAction.Error_FunctionError, LogManagement.SystemName.基本作業, "新增一筆資料SingleCRUD時，發生錯誤", singleCRUDData, ex);
                return Json(new { result = "false" });
            }
        }

        /// <summary>
        /// 修改資料
        /// </summary>
        /// <returns></returns>
        public JsonResult UpdateSingleCRUDData(SingleCRUDModel singleCRUDDataModel)
        {
            try
            {
                //判斷PKEY是否有填寫
                if (string.IsNullOrEmpty(singleCRUDDataModel.Id))
                {
                    Log.LogInfoWriter(userName, LogManagement.LogType.AP, LogManagement.EventLevel.Error, LogManagement.LogAction.Error_NullException, LogManagement.SystemName.基本作業, "傳入修改參數的Pkey為Null", singleCRUDDataModel, null);
                    return Json(new { result = "false" });
                }
                Log.LogInfoWriter(userName, LogManagement.LogType.OP, LogManagement.EventLevel.Info, LogManagement.LogAction.USER_Action_Update, LogManagement.SystemName.基本作業, "使用者修改一筆資料", singleCRUDDataModel, null);
                Log.LogInfoWriter(userName, LogManagement.LogType.AP, LogManagement.EventLevel.Info, LogManagement.LogAction.SYS_FunctionStart, LogManagement.SystemName.基本作業, "開始修改一筆資料", singleCRUDDataModel, null);
                var result = _service.UpdateSingleCRUDData(singleCRUDDataModel);
                Log.LogInfoWriter(userName, LogManagement.LogType.AP, LogManagement.EventLevel.Info, LogManagement.LogAction.SYS_FunctionEnd, LogManagement.SystemName.基本作業, "修改一筆資料結束", singleCRUDDataModel, null);
                return Json(new { result = result });
            }
            catch (Exception ex)
            {
                Log.LogInfoWriter(userName, LogManagement.LogType.AP, LogManagement.EventLevel.Error, LogManagement.LogAction.Error_FunctionError, LogManagement.SystemName.基本作業, "修改一筆資料發生錯誤", singleCRUDDataModel, ex);
                return Json(new { result = "false" });
            }
        }

        /// <summary>
        /// 刪除資料
        /// </summary>
        /// <returns></returns>
        public ActionResult DeleteSingleCRUDDataList(List<string> didList)
        {
            try
            {
                //判斷PKEY是否有填寫
                foreach (var item in didList)
                {
                    if (string.IsNullOrEmpty(item))
                    {
                        Log.LogInfoWriter(userName, LogManagement.LogType.AP, LogManagement.EventLevel.Error, LogManagement.LogAction.Error_NullException, LogManagement.SystemName.基本作業, "傳入刪除參數的Pkey為Null", didList, null);
                        return Json(new { result = "人員Id為空值" });
                    }
                }
                Log.LogInfoWriter(userName, LogManagement.LogType.OP, LogManagement.EventLevel.Info, LogManagement.LogAction.USER_Action_Delete, LogManagement.SystemName.基本作業, "使用者刪除一筆SingleCRUD資料", didList, null);
                Log.LogInfoWriter(userName, LogManagement.LogType.AP, LogManagement.EventLevel.Info, LogManagement.LogAction.SYS_FunctionStart, LogManagement.SystemName.基本作業, "開始刪除一筆SingleCRUD資料", didList, null);
                var result = _service.DeleteSingleCRUDDataList(didList);
                Log.LogInfoWriter(userName, LogManagement.LogType.AP, LogManagement.EventLevel.Info, LogManagement.LogAction.SYS_FunctionEnd, LogManagement.SystemName.基本作業, "刪除一筆SingleCRUD資料結束", didList, null);
                return Json(new { result = result });
            }
            catch (Exception ex)
            {
                Log.LogInfoWriter(userName, LogManagement.LogType.AP, LogManagement.EventLevel.Error, LogManagement.LogAction.Error_FunctionError, LogManagement.SystemName.基本作業, "刪除一筆SingleCRUD資料發生錯誤", didList, ex);
                return Json(new { result = "false" });
            }
        }
    }
}