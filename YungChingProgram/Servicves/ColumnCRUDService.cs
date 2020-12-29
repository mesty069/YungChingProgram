﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using YungChingProgram.Models;
using YungChingProgram.Models.Database;

namespace YungChingProgram.Servicves
{
    public class ColumnCRUDService
    {
        private readonly TestDBEntities _db = new TestDBEntities();
        /// <summary>
        /// 取得人員類別下拉式選單
        /// </summary>
        /// <returns></returns>
        public List<SelectListItem> GetTypeSelectList()
        {
            try
            {
                var selectListData = (from category in _db.Category
                                      where category.Class_group == "ptype" && category.Flag == "Y"
                                      select new SelectListItem { Text = category.Name, Value = category.Code }).ToList();
                return selectListData;
            }
            catch (Exception ex)
            {
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
                var typeSelectList = GetTypeSelectList();
                var columnCRUDDataList = (from columnData in _db.ColumnCRUD
                                          where (columnData.Name == null || (columnData.Name.Contains(name)) &&
                                          (string.IsNullOrEmpty(type) || columnData.Type == type))
                                          select new ColumnCRUDDataModel
                                          {
                                              Sex = columnData.Sex,
                                              Id = columnData.Id,
                                              Name = columnData.Name,
                                              Tel = columnData.Tel,
                                              Type = columnData.Type,
                                              Address = columnData.Address
                                          }).ToList();
                return columnCRUDDataList;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }
}