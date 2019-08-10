using System.Collections.Generic;

namespace CentralizedDataSystem.Services.Interfaces {
    public interface ISurveyService {
        List<string> GetListDataFromFile(string pathFile, string fileName, string importer);
        bool Insert(string jsonData);
    }
}
