using CentralizedDataSystem.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CentralizedDataSystem.Services.Interfaces {
    public interface IGroupService {
        Task<string> FindGroupFieldByIdGroup(string token, string idGroup, string field);
        Task<string> FindGroupDataById(string token, string id);
        Task<Group> FindGroupParent(string token, string condition);
        Task<List<Group>> FindListChildGroupByIdParentWithPage(string token, string idParent, string nameParent, int page);
        Task<int> FindNumberOfChildGroupByIdParent(string token, string idParent);
        Task<string> FindGroupsByIdParentWhenCallAjax(string token, string idParent);
        Task<string> InsertGroup(string token, string data);
    }
}
