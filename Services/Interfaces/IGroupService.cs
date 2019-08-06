using CentralizedDataSystem.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CentralizedDataSystem.Services.Interfaces {
    public interface IGroupService {
        Task<string> FindGroupFieldByIdGroup(string idGroup, string field);
        Task<string> FindGroupDataById(string id);
        Task<Group> FindGroupParent(string condition);
        Task<List<Group>> FindListChildGroupByIdParentWithPage(string idParent, string nameParent, int page);
        Task<int> FindNumberOfChildGroupByIdParent(string idParent);
        Task<string> FindGroupsByIdParentWhenCallAjax(string idParent);
        Task<string> InsertGroup(string data);
    }
}
