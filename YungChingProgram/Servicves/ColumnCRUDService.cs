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
    public class ColumnCRUDService
    {
        private readonly TestDBEntities _db = new TestDBEntities();
        private static readonly LogManagement Log = new LogManagement();
        private string userName = "admin";

        /// <summary>
        /// 取得人員類別下拉式選單
        /// </summary>
        /// <returns></returns>
        public List<SelectListItem> GetTypeSelectList()
        {
            try
            {
                Log.LogInfoWriter(userName, LogManagement.LogType.AP, LogManagement.EventLevel.Info, LogManagement.LogAction.SYS_FunctionStart, LogManagement.SystemName.基本作業, "開始取得Type下拉式選單", null, null);
                var selectListData = (from category in _db.Category
                                      where category.Class_group == "ptype" && category.Flag == "Y"
                                      select new SelectListItem { Text = category.Name, Value = category.Code }).ToList();
                Log.LogInfoWriter(userName, LogManagement.LogType.AP, LogManagement.EventLevel.Info, LogManagement.LogAction.SYS_FunctionEnd, LogManagement.SystemName.基本作業, "取得Type下拉式選單結束", selectListData, null);
                return selectListData;
            }
            catch (Exception ex)
            {
                Log.LogInfoWriter(userName, LogManagement.LogType.AP, LogManagement.EventLevel.Info, LogManagement.LogAction.Error_FunctionError, LogManagement.SystemName.基本作業, "取得Type下拉式選單，發生錯誤", null, ex);
                return new List<SelectListItem>();
            }
        }

        /// <summary>
        /// 取得多筆人員資料
        /// </summary>
        /// <returns></returns>
        public List<ColumnCRUDDataModel> GetColumnCRUDDataList(string name, string type)
        {
            try
            {
                Log.LogInfoWriter(userName, LogManagement.LogType.AP, LogManagement.EventLevel.Info, LogManagement.LogAction.SYS_FunctionStart, LogManagement.SystemName.基本作業, "開始取得ColumnCRUD多筆資料", new { name, type }, null);
                var typeSelectList = GetTypeSelectList();
                var columnCRUDDataList = (from columnData in _db.ColumnCRUD
                                          where (string.IsNullOrEmpty(name) || columnData.Name.Contains(name)) &&
                                          (string.IsNullOrEmpty(type) || columnData.Type == type)
                                          select new ColumnCRUDDataModel
                                          {
                                              Sex = columnData.Sex,
                                              Id = columnData.Id,
                                              Name = columnData.Name,
                                              Tel = columnData.Tel,
                                              Type = columnData.Type,
                                              Address = columnData.Address
                                          }).ToList();
                Log.LogInfoWriter(userName, LogManagement.LogType.AP, LogManagement.EventLevel.Info, LogManagement.LogAction.SYS_FunctionEnd, LogManagement.SystemName.基本作業, "取得ColumnCRUD多筆資料結束", columnCRUDDataList, null);
                return columnCRUDDataList;
            }
            catch (Exception ex)
            {
                Log.LogInfoWriter(userName, LogManagement.LogType.AP, LogManagement.EventLevel.Info, LogManagement.LogAction.Error_FunctionError, LogManagement.SystemName.基本作業, "取得ColumnCRUD多筆資料，發生錯誤", new { name, type }, ex);
                return null;
            }
        }

        public string InsertColumnCRUDData(ColumnCRUD columnCRUDData)
        {
            try
            {
                Log.LogInfoWriter(userName, LogManagement.LogType.AP, LogManagement.EventLevel.Info, LogManagement.LogAction.SYS_FunctionStart, LogManagement.SystemName.基本作業, "開始新增一筆ColumnCRUD資料", columnCRUDData, null);
                ColumnCRUD columnCRUD = (from column in _db.ColumnCRUD
                                         where column.Id == columnCRUDData.Id
                                         select column).FirstOrDefault();
                //搜尋出重複資料，新增失敗
                if (columnCRUD != null)
                {
                    Log.LogInfoWriter(userName, LogManagement.LogType.AP, LogManagement.EventLevel.Error, LogManagement.LogAction.Error_FunctionError, LogManagement.SystemName.基本作業, "新增到重複PKey的ColumnCRUD資料，取消新增動作", columnCRUDData.Id, null);
                    return "新增到重複ColumnCRUD資料，新增失敗";
                }
                columnCRUDData.Upuser = "admin";
                columnCRUDData.Updatetime = DateTime.Now;
                columnCRUDData.Cruser = "admin";
                columnCRUDData.Crdatetime = DateTime.Now;
                _db.ColumnCRUD.Add(columnCRUDData);
                _db.SaveChanges();
                Log.LogInfoWriter(userName, LogManagement.LogType.AP, LogManagement.EventLevel.Error, LogManagement.LogAction.SYS_FunctionEnd, LogManagement.SystemName.基本作業, "新增一筆ColumnCRUD資料結束", columnCRUDData.Id, null);
                return "true";
            }
            catch (Exception ex)
            {
                Log.LogInfoWriter(userName, LogManagement.LogType.AP, LogManagement.EventLevel.Info, LogManagement.LogAction.Error_FunctionError, LogManagement.SystemName.基本作業, "新增一筆ColumnCRUD資料，發生錯誤", columnCRUDData, ex);
                return "false";
            }
        }

        public string UpdateColumnCRUDData(ColumnCRUDDataModel columnCRUDDataModel)
        {
            try
            {
                Log.LogInfoWriter(userName, LogManagement.LogType.AP, LogManagement.EventLevel.Info, LogManagement.LogAction.SYS_FunctionStart, LogManagement.SystemName.基本作業, "開始修改一筆ColumnCRUD資料", columnCRUDDataModel, null);
                ColumnCRUD columnCRUD = (from column in _db.ColumnCRUD
                                         where column.Id == columnCRUDDataModel.Id
                                         select column).FirstOrDefault();
                //查無資料，修改失敗
                if (columnCRUD == null)
                {
                    Log.LogInfoWriter(userName, LogManagement.LogType.AP, LogManagement.EventLevel.Error, LogManagement.LogAction.Error_FunctionError, LogManagement.SystemName.基本作業, "查無無該筆資料需要修改的資料", columnCRUDDataModel.Id, null);
                    return "查無此修改資料，請確認人員編號是否異動或刪除";
                }
                _db.ColumnCRUD.Attach(columnCRUD);
                columnCRUD.Address = columnCRUDDataModel.Address;
                columnCRUD.Name = columnCRUDDataModel.Name;
                columnCRUD.Sex = columnCRUDDataModel.Sex;
                columnCRUD.Tel = columnCRUDDataModel.Tel;
                columnCRUD.Type = columnCRUDDataModel.Type;
                columnCRUD.Upuser = "admin";
                columnCRUD.Updatetime = DateTime.Now;
                _db.SaveChanges();
                Log.LogInfoWriter(userName, LogManagement.LogType.AP, LogManagement.EventLevel.Error, LogManagement.LogAction.SYS_FunctionEnd, LogManagement.SystemName.基本作業, "修改一筆ColumnCRUD資料結束", columnCRUDDataModel.Id, null);
                return "true";
            }
            catch (Exception ex)
            {
                return "false";
            }
        }

        public string DeleteColumnCRUDData(string pid)
        {
            try
            {
                Log.LogInfoWriter(userName, LogManagement.LogType.AP, LogManagement.EventLevel.Info, LogManagement.LogAction.SYS_FunctionStart, LogManagement.SystemName.基本作業, "開始刪除一筆ColumnCRUD資料", pid, null);
                ColumnCRUD columnCRUD = (from column in _db.ColumnCRUD
                                         where column.Id == pid
                                         select column).FirstOrDefault();
                //查無資料，修改失敗
                if (columnCRUD == null)
                {
                    Log.LogInfoWriter(userName, LogManagement.LogType.AP, LogManagement.EventLevel.Error, LogManagement.LogAction.Error_FunctionError, LogManagement.SystemName.基本作業, "查無該筆資料需要修改的資料", pid, null);
                    return "查無此刪除資料，請確認人員編號是否異動或已刪除";
                }
                _db.ColumnCRUD.Remove(columnCRUD);
                _db.SaveChanges();
                Log.LogInfoWriter(userName, LogManagement.LogType.AP, LogManagement.EventLevel.Error, LogManagement.LogAction.SYS_FunctionEnd, LogManagement.SystemName.基本作業, "刪除一筆ColumnCRUD資料結束", pid, null);
                return "true";
            }
            catch (Exception ex)
            {
                Log.LogInfoWriter(userName, LogManagement.LogType.AP, LogManagement.EventLevel.Info, LogManagement.LogAction.Error_FunctionError, LogManagement.SystemName.基本作業, "刪除一筆ColumnCRUD資料，發生錯誤", pid, ex);
                return "false";
            }
        }
    }
}