using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using YungChingProgram.Models;

namespace YungChingProgram.ViewModels
{
    public class SingleCRUDViewModel
    {
        public SelectList DTypeSelectList { get; set; } = new SelectList(new List<SelectListItem>());
        public string ErrorMessage { get; set; }

    }

    public class SingleCRUDPartialViewModel 
    {
        public List<SingleCRUDModel> SingleCRUDModelList { get; set; } = new List<SingleCRUDModel>();

        public string ErrorMessage { get; set; }
    }

    public class SingleCRUDInfoViewModel : SingleCRUDViewModel
    {
        public SingleCRUDModel SingleCRUDModel { get; set; } = new SingleCRUDModel();

        public string Mode { get; set; }
    }
}