using CentralizedDataSystem.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CentralizedDataSystem.Services.Interfaces {
    public interface IFormControlService {
        Task<FormControl> FindByPathForm(string pathForm);
        Task<List<FormControl>> FindByOwner(string email);
        Task<List<FormControl>> FindByAssign(string assign);
        bool Insert(FormControl formControl);
        Task<long> Update(FormControl formControl, string oldPath);
        Task<bool> DeleteByPathForm(string pathForm);
    }
}
