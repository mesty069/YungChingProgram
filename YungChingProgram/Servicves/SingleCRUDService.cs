using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using YungChingProgram._GeneralLibrary;
using YungChingProgram.Models;
using YungChingProgram.Models.Database;

namespace YungChingProgram.Servicves
{
    public class SingleCRUDService
    {
        private readonly TestDBEntities _db = new TestDBEntities();
        private static readonly LogManagement Log = new LogManagement();
        private string userName = "admin";
        public List<SelectListItem> GetDtypeSelecList()
        {
            try
            {
                Log.LogInfoWriter(userName, LogManagement.LogType.AP, LogManagement.EventLevel.Info, LogManagement.LogAction.SYS_FunctionStart, LogManagement.SystemName.基本作業, "開始取得DType下拉式選單", null, null);
                var selectListData = (from category in _db.Category
                                      where (category.Class_group == "dtype" && category.Flag == "Y")
                                      select new SelectListItem { Text = category.Name, Value = category.Code }).ToList();
                Log.LogInfoWriter(userName, LogManagement.LogType.AP, LogManagement.EventLevel.Info, LogManagement.LogAction.SYS_FunctionEnd, LogManagement.SystemName.基本作業, "取得DType下拉式選單結束", null, null);
                return selectListData;
            }
            catch (Exception ex)
            {
                Log.LogInfoWriter(userName, LogManagement.LogType.AP, LogManagement.EventLevel.Error, LogManagement.LogAction.Error_FunctionError, LogManagement.SystemName.基本作業, "取得DType下拉式選單，發生錯誤", null, ex);
                return new List<SelectListItem>();
            }
        }

        /// <summary>
        /// 取得多筆人員資料
        /// </summary>
        /// <returns></returns>
        public List<SingleCRUDModel> GetSingleCRUDDataList(string name, string type)
        {
            try
            {
                Log.LogInfoWriter(userName, LogManagement.LogType.AP, LogManagement.EventLevel.Info, LogManagement.LogAction.SYS_FunctionStart, LogManagement.SystemName.基本作業, "開始取得SingleCRUD多筆資料", new { name, type }, null);
                var typeNameList = (from dtype in _db.Category
                                    where (dtype.Class_group.Equals("dtype", StringComparison.OrdinalIgnoreCase) && dtype.Flag == "Y")
                                    select dtype).ToList();


                var singleCRUDDataList = (from singleData in _db.SingleCRUD
                                          where (string.IsNullOrEmpty(name) || singleData.Name.Contains(name)) &&
                                          (string.IsNullOrEmpty(type) || singleData.Dtype == type)
                                          select new SingleCRUDModel
                                          {
                                              Dtype = singleData.Dtype,
                                              Id = singleData.Id,
                                              Name = singleData.Name,
                                              HeadCount = singleData.Headcount
                                          }).ToList();
                foreach (var singleCRUDData in singleCRUDDataList)
                {
                    var typeNameItem = typeNameList.Where(x => x.Class_group.Equals("dtype", StringComparison.OrdinalIgnoreCase) && x.Code == singleCRUDData.Dtype).FirstOrDefault();
                    if (typeNameItem != null)
                    {
                        singleCRUDData.DtypeName = typeNameItem.Name;
                    };
                }
                Log.LogInfoWriter(userName, LogManagement.LogType.AP, LogManagement.EventLevel.Info, LogManagement.LogAction.SYS_FunctionEnd, LogManagement.SystemName.基本作業, "取得SingleCRUD多筆資料結束", singleCRUDDataList, null);
                return singleCRUDDataList;
            }
            catch (Exception ex)
            {
                Log.LogInfoWriter(userName, LogManagement.LogType.AP, LogManagement.EventLevel.Info, LogManagement.LogAction.Error_FunctionError, LogManagement.SystemName.基本作業, "取得SingleCRUD多筆資料，發生錯誤", new { name, type }, ex);
                return null;
            }
        }

        public string InsertSingleCRUDData(SingleCRUD singleCRUDData)
        {
            try
            {
                Log.LogInfoWriter(userName, LogManagement.LogType.AP, LogManagement.EventLevel.Info, LogManagement.LogAction.SYS_FunctionStart, LogManagement.SystemName.基本作業, "開始新增一筆SingleCRUD資料", singleCRUDData, null);
                SingleCRUD singleCRUD = (from singleData in _db.SingleCRUD
                                         where singleData.Id == singleCRUDData.Id
                                         select singleData).FirstOrDefault();
                //搜尋出重複資料，新增失敗
                if (singleCRUD != null)
                {
                    Log.LogInfoWriter(userName, LogManagement.LogType.AP, LogManagement.EventLevel.Error, LogManagement.LogAction.Error_FunctionError, LogManagement.SystemName.基本作業, "新增到重複PKey的SingleCRUD資料，取消新增動作", singleCRUDData.Id, null);
                    return "新增到重複SingleCRUD資料，新增失敗";
                }
                singleCRUDData.Upuser = "admin";
                singleCRUDData.Updatetime = DateTime.Now;
                singleCRUDData.Cruser = "admin";
                singleCRUDData.Crdatetime = DateTime.Now;
                _db.SingleCRUD.Add(singleCRUDData);
                _db.SaveChanges();
                Log.LogInfoWriter(userName, LogManagement.LogType.AP, LogManagement.EventLevel.Error, LogManagement.LogAction.SYS_FunctionEnd, LogManagement.SystemName.基本作業, "新增一筆SingleCRUD資料結束", singleCRUDData.Id, null);
                return "true";
            }
            catch (Exception ex)
            {
                Log.LogInfoWriter(userName, LogManagement.LogType.AP, LogManagement.EventLevel.Info, LogManagement.LogAction.Error_FunctionError, LogManagement.SystemName.基本作業, "新增一筆SingleCRUD資料，發生錯誤", singleCRUDData, ex);
                return "false";
            }
        }

        public string UpdateSingleCRUDData(SingleCRUDModel singleCRUDDataModel)
        {
            try
            {
                Log.LogInfoWriter(userName, LogManagement.LogType.AP, LogManagement.EventLevel.Info, LogManagement.LogAction.SYS_FunctionStart, LogManagement.SystemName.基本作業, "開始修改一筆SingleCRUD資料", singleCRUDDataModel, null);
                SingleCRUD singleCRUD = (from single in _db.SingleCRUD
                                         where single.Id == singleCRUDDataModel.Id
                                         select single).FirstOrDefault();
                //查無資料，修改失敗
                if (singleCRUD == null)
                {
                    return "查無此修改資料，請確認人員編號是否異動或刪除";
                }
                _db.SingleCRUD.Attach(singleCRUD);
                singleCRUD.Name = singleCRUDDataModel.Name;
                singleCRUD.Headcount = singleCRUDDataModel.HeadCount;
                singleCRUD.Dtype = singleCRUDDataModel.Dtype;
                singleCRUD.Upuser = "admin";
                singleCRUD.Updatetime = DateTime.Now;
                singleCRUD.Cruser = "admin";
                singleCRUD.Crdatetime = DateTime.Now;
                _db.SaveChanges();
                Log.LogInfoWriter(userName, LogManagement.LogType.AP, LogManagement.EventLevel.Error, LogManagement.LogAction.SYS_FunctionEnd, LogManagement.SystemName.基本作業, "新增一筆SingleCRUD資料結束", singleCRUDDataModel.Id, null);
                return "true";
            }
            catch (Exception ex)
            {
                Log.LogInfoWriter(userName, LogManagement.LogType.AP, LogManagement.EventLevel.Info, LogManagement.LogAction.Error_FunctionError, LogManagement.SystemName.基本作業, "新增一筆SingleCRUD資料，發生錯誤", singleCRUDDataModel, ex);
                return "false";
            }
        }

        /// <summary>
        /// 取得多筆人員資料
        /// </summary>
        /// <returns></returns>
        public SingleCRUDModel GetSingleCRUDData(string dId)
        {
            try
            {
                Log.LogInfoWriter(userName, LogManagement.LogType.AP, LogManagement.EventLevel.Info, LogManagement.LogAction.SYS_FunctionStart, LogManagement.SystemName.基本作業, "開始取得SingleCRUD單筆資料", dId, null);
                var singleCRUDDataList = (from singleData in _db.SingleCRUD
                                          where singleData.Id == dId
                                          select new SingleCRUDModel
                                          {
                                              Dtype = singleData.Dtype,
                                              Id = singleData.Id,
                                              Name = singleData.Name,
                                              HeadCount = singleData.Headcount
                                          }).FirstOrDefault();
                Log.LogInfoWriter(userName, LogManagement.LogType.AP, LogManagement.EventLevel.Info, LogManagement.LogAction.SYS_FunctionEnd, LogManagement.SystemName.基本作業, "取得SingleCRUD單筆資料結束", singleCRUDDataList, null);
                return singleCRUDDataList;
            }
            catch (Exception ex)
            {
                Log.LogInfoWriter(userName, LogManagement.LogType.AP, LogManagement.EventLevel.Info, LogManagement.LogAction.Error_FunctionError, LogManagement.SystemName.基本作業, "取得SingleCRUD單筆資料，發生錯誤", dId, ex);
                return null;
            }
        }

        /// <summary>
        /// 刪除多筆資料
        /// </summary>
        /// <param name="didList"></param>
        /// <returns></returns>
        public string DeleteSingleCRUDDataList(List<string> didList)
        {
            try
            {
                Log.LogInfoWriter(userName, LogManagement.LogType.AP, LogManagement.EventLevel.Info, LogManagement.LogAction.SYS_FunctionStart, LogManagement.SystemName.基本作業, "開始刪除一筆ColumnCRUD資料", didList, null);
                foreach (var item in didList)
                {
                    SingleCRUD singleCRUD = (from single in _db.SingleCRUD
                                             where single.Id == item
                                             select single).FirstOrDefault();
                    //查無資料刪除失敗
                    if (singleCRUD == null)
                    {
                        Log.LogInfoWriter(userName, LogManagement.LogType.AP, LogManagement.EventLevel.Error, LogManagement.LogAction.Error_FunctionError, LogManagement.SystemName.基本作業, "查無該筆資料需要刪除的資料", didList, null);
                        return "查無此刪除資料，請確認人員編號是否異動或已刪除";
                    }
                    _db.SingleCRUD.Remove(singleCRUD);
                }
                _db.SaveChanges();
                Log.LogInfoWriter(userName, LogManagement.LogType.AP, LogManagement.EventLevel.Error, LogManagement.LogAction.SYS_FunctionEnd, LogManagement.SystemName.基本作業, "刪除一筆ColumnCRUD資料結束", didList, null);
                return "true";
            }
            catch (Exception ex)
            {
                Log.LogInfoWriter(userName, LogManagement.LogType.AP, LogManagement.EventLevel.Info, LogManagement.LogAction.Error_FunctionError, LogManagement.SystemName.基本作業, "刪除一筆ColumnCRUD資料，發生錯誤", didList, ex);
                return "false";
            }
        }
    }
}