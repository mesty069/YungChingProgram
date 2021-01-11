using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using YungChingProgram.Models;
using YungChingProgram.Models.Database;

namespace YungChingProgram.Servicves
{
    public class SingleCRUDService
    {
        private readonly TestDBEntities _db = new TestDBEntities();

        public List<SelectListItem> GetDtypeSelecList()
        {
            try
            {
                var selectListData = (from category in _db.Category
                                      where (category.Class_group == "dtype" && category.Flag == "Y")
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
        public List<SingleCRUDModel> GetSingleCRUDDataList(string name, string type)
        {
            try
            {

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
                return singleCRUDDataList;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public string InsertSingleCRUDData(SingleCRUD singleCRUDData)
        {
            try
            {
                SingleCRUD singleCRUD = (from singleData in _db.SingleCRUD
                                         where singleData.Id == singleCRUDData.Id
                                         select singleData).FirstOrDefault();
                //搜尋出重複資料，新增失敗
                if (singleCRUD != null)
                {
                    return "新增到重複SingleCRUD資料，新增失敗";
                }
                singleCRUDData.Upuser = "admin";
                singleCRUDData.Updatetime = DateTime.Now;
                singleCRUDData.Cruser = "admin";
                singleCRUDData.Crdatetime = DateTime.Now;
                _db.SingleCRUD.Add(singleCRUDData);
                _db.SaveChanges();
                return "true";
            }
            catch (Exception ex)
            {
                return "false";
            }
        }

        public string UpdateSingleCRUDData(SingleCRUDModel singleCRUDDataModel)
        {
            try
            {
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
                return "true";
            }
            catch (Exception ex)
            {
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
                var singleCRUDDataList = (from singleData in _db.SingleCRUD
                                          where singleData.Id == dId
                                          select new SingleCRUDModel
                                          {
                                              Dtype = singleData.Dtype,
                                              Id = singleData.Id,
                                              Name = singleData.Name,
                                              HeadCount = singleData.Headcount
                                          }).FirstOrDefault();
                return singleCRUDDataList;
            }
            catch (Exception ex)
            {
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
                foreach (var item in didList)
                {
                    SingleCRUD singleCRUD = (from single in _db.SingleCRUD
                                             where single.Id == item
                                             select single).FirstOrDefault();
                    //查無資料刪除失敗
                    if (singleCRUD == null)
                    {
                        return "查無此刪除資料，請確認人員編號是否異動或已刪除";
                    }
                    _db.SingleCRUD.Remove(singleCRUD);
                }
                _db.SaveChanges();
                return "true";
            }
            catch (Exception ex)
            {
                return "false";
            }
        }
    }
}