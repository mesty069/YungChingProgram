using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using YungChingProgram.Models;
using YungChingProgram.Models.Database;
using YungChingProgram.Servicves;
using YungChingProgram.ViewModels;

namespace YungChingProgram.Controllers
{
    public class SingleCRUDController : Controller
    {
        private readonly SingleCRUDService _service = new SingleCRUDService();
        // GET: SingleCRUD
        public ActionResult Index()
        {
            try
            {
                SingleCRUDViewModel singleCRUDViewModel = new SingleCRUDViewModel();
                var selectList = _service.GetDtypeSelecList();
                selectList.Insert(0, new SelectListItem { Text = "請選擇", Value = "" });
                singleCRUDViewModel.DTypeSelectList = new SelectList(selectList, "Value", "Text");
                return View(singleCRUDViewModel);
            }
            catch (Exception ex)
            {
                return View();
            }
        }

        public PartialViewResult SingleCRUDPartialView(string name, string type)
        {
            try
            {
                SingleCRUDPartialViewModel singleCRUDPartialViewModel = new SingleCRUDPartialViewModel();
                singleCRUDPartialViewModel.SingleCRUDModelList = _service.GetSingleCRUDDataList(name, type);
                if (singleCRUDPartialViewModel.SingleCRUDModelList == null)
                {
                    singleCRUDPartialViewModel.ErrorMessage = "查詢發生錯誤";
                    return PartialView(singleCRUDPartialViewModel);
                }
                return PartialView(singleCRUDPartialViewModel);
            }
            catch (Exception ex)
            {
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
                SingleCRUDInfoViewModel singleCRUDInfoModel = new SingleCRUDInfoViewModel();
                var selectList = _service.GetDtypeSelecList();
                selectList.Insert(0, new SelectListItem { Text = "請選擇", Value = "" });
                singleCRUDInfoModel.DTypeSelectList = new SelectList(selectList, "Value", "Text");
                singleCRUDInfoModel.Mode = Mode;
                if (Mode == "Update")
                {
                    if (string.IsNullOrEmpty(id))
                    {
                        singleCRUDInfoModel.ErrorMessage = "需要修改資料的代碼為空值，修改動作失敗";
                        return View(singleCRUDInfoModel);
                    }
                    singleCRUDInfoModel.SingleCRUDModel = _service.GetSingleCRUDData(id);
                    if (singleCRUDInfoModel.SingleCRUDModel == null)
                    {
                        singleCRUDInfoModel.ErrorMessage = "查無該筆修改資料，修改動作失敗";
                        return View(singleCRUDInfoModel);
                    }
                    return View(singleCRUDInfoModel);
                }
                return View(singleCRUDInfoModel);
            }
            catch (Exception ex)
            {
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
                    return Json(new { result = "人員代號為必填欄位" });
                }
                var result = _service.InsertSingleCRUDData(singleCRUDData);
                return Json(new { result = result });
            }
            catch (Exception ex)
            {
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
                    return Json(new { result = "false" });
                }
                var result = _service.UpdateSingleCRUDData(singleCRUDDataModel);
                return Json(new { result = result });
            }
            catch (Exception ex)
            {
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
                        return Json(new { result = "人員Id為空值" });
                    }
                }
                var result = _service.DeleteSingleCRUDDataList(didList);
                return Json(new { result = result });
            }
            catch (Exception ex)
            {
                return Json(new { result = "false" });
            }
        }
    }
}