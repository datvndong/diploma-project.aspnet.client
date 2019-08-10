using CentralizedDataSystem.Repositories.Interfaces;
using CentralizedDataSystem.Resources;
using CentralizedDataSystem.Services.Interfaces;
using MongoDB.Bson;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NPOI.SS.UserModel;
using NPOI.SS.Util;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

namespace CentralizedDataSystem.Services.Implements {
    public class SurveyService : ISurveyService {
        private readonly IMongoRepository _mongoRepository;
        private int _numberAddedRow;

        public SurveyService(IMongoRepository mongoRepository) {
            _mongoRepository = mongoRepository;
            _mongoRepository.InitCollection(Collections.SURVEYS);
            _numberAddedRow = 0;
        }

        private string GetCellStringValue(ISheet sheet, int row, int column) {
            return sheet.GetRow(row).GetCell(column).StringCellValue;
        }

        private int GetMaxMergedRegionRowLength(ISheet sheet, List<CellRangeAddress> listMergedRegion) {
            int max = 0;
            int mergedRegionsSize = sheet.NumMergedRegions;
            int rowRange = 0;
            CellRangeAddress cellRangeAddress = null;
            for (int i = 0; i < mergedRegionsSize; ++i) {
                cellRangeAddress = sheet.GetMergedRegion(i);
                listMergedRegion.Add(cellRangeAddress);
                rowRange = cellRangeAddress.LastRow - cellRangeAddress.FirstRow + 1;
                if (rowRange > max) {
                    max = rowRange;
                }
            }
            return max;
        }

        private static void Swap<T>(IList<T> list, int indexA, int indexB) {
            T tmp = list[indexA];
            list[indexA] = list[indexB];
            list[indexB] = tmp;
        }

        private void SortMergedRegions(List<CellRangeAddress> listMergedRegion, bool isSortByRow) {
            int size = listMergedRegion.Count;
            for (int i = 0; i < size - 2; i++) {
                for (int j = size - 1; j > i; j--) {
                    if (isSortByRow) {
                        if (listMergedRegion[j].FirstRow < listMergedRegion[j - 1].FirstRow) {
                            Swap(listMergedRegion, j, j - 1);
                        }
                    } else {
                        if (listMergedRegion[j].FirstColumn < listMergedRegion[j - 1].FirstColumn) {
                            Swap(listMergedRegion, j, j - 1);
                        }
                    }
                }
            }
        }

        private void AddMergeRegionToJSON(ISheet sheet, int maxMergedRegionRowLength, List<CellRangeAddress> listMergedRegion,
            JObject jsonObject, int rowStartIndex, string parentStringCellValue) {
            List<CellRangeAddress> listMergedRegionTemp = new List<CellRangeAddress>();
            CellRangeAddress range = null;
            ICell cell = null;

            int regionsSize = listMergedRegion.Count;
            if (regionsSize == 0) {
                _numberAddedRow = rowStartIndex;
            }
            bool isNotFirstRow = rowStartIndex != 0;

            for (int j = 0; j < regionsSize; j++) {
                range = listMergedRegion[j];
                int firstRow = range.FirstRow;
                int firstColumn = range.FirstColumn;

                // If row index != 0 -> must find parent cell
                if (isNotFirstRow) {
                    cell = GetParentCell(sheet, firstRow, firstColumn);
                    if (cell.CellType != CellType.Blank) {
                        parentStringCellValue = cell.StringCellValue;
                    }
                }

                if (firstRow == rowStartIndex) {
                    if (range.LastRow == maxMergedRegionRowLength - 1) {
                        // If lastRow == maxMergedRegionRowLength -> no have child cell
                        if (isNotFirstRow) {
                            AddKeyToJsonObject(jsonObject, parentStringCellValue,
                                    GetCellStringValue(sheet, firstRow, firstColumn), false);
                        } else {
                            jsonObject.Add(GetCellStringValue(sheet, firstRow, firstColumn), string.Empty);
                        }
                    } else {
                        if (isNotFirstRow) {
                            AddKeyToJsonObject(jsonObject, parentStringCellValue,
                                    GetCellStringValue(sheet, firstRow, firstColumn), true);
                        } else {
                            jsonObject.Add(GetCellStringValue(sheet, firstRow, range.FirstColumn), new JObject());
                        }
                    }
                } else {
                    listMergedRegionTemp.Add(range);
                }
            }

            rowStartIndex++;
            if (rowStartIndex < maxMergedRegionRowLength) {
                AddMergeRegionToJSON(sheet, maxMergedRegionRowLength, listMergedRegionTemp, jsonObject, rowStartIndex,
                        parentStringCellValue);
            }
        }

        private ICell GetParentCell(ISheet sheet, int currRowIndex, int currColIndex) {
            currRowIndex = currRowIndex - 1;
            ICell cell = sheet.GetRow(currRowIndex).GetCell(currColIndex);
            if (cell.CellType == CellType.Blank && currRowIndex > 0) {
                return GetParentCell(sheet, currRowIndex, currColIndex);
            }
            return cell;
        }

        private void AddKeyToJsonObject(JObject jsonObject, string parentKey, string currKey, bool isAddJsonObject) {
            if (string.Empty.Equals(parentKey)) {
                if (isAddJsonObject) {
                    jsonObject.Add(currKey, new JObject());
                } else {
                    jsonObject.Add(currKey, string.Empty);
                }
            } else {
                foreach (KeyValuePair<string, JToken> entry in jsonObject) {
                    string entryKey = entry.Key;
                    JToken entryValue = entry.Value;

                    if (entryKey.Equals(parentKey)) {
                        if (isAddJsonObject) {
                            ((JObject)jsonObject.GetValue(entryKey)).Add(currKey, new JObject());
                        } else {
                            ((JObject)jsonObject.GetValue(entryKey)).Add(currKey, string.Empty);
                        }
                        break;
                    }

                    if (entryValue is JObject) {
                        AddKeyToJsonObject((JObject)jsonObject.GetValue(entryKey), parentKey, currKey, isAddJsonObject);
                    }
                }
            }
        }

        private bool SetValueToJSONBluePrint(JObject jsonObjectTemp, string value, int recursiveSteps) {
            foreach (KeyValuePair<string, JToken> entry in jsonObjectTemp) {
                string entryKey = entry.Key;
                JToken entryValue = entry.Value;

                if (entryValue is JObject) {
                    if (!SetValueToJSONBluePrint((JObject)jsonObjectTemp.GetValue(entryKey), value, ++recursiveSteps)) {
                        continue;
                    } else if (--recursiveSteps == 0) {
                        break;
                    } else {
                        return true;
                    }
                } else {
                    if (string.Empty.Equals(entryValue.ToString())) {
                        jsonObjectTemp[entryKey] = value;
                        return true;
                    }
                }
            }
            return false;
        }

        public List<string> GetListDataFromFile(string pathFile, string fileName, string importer) {
            FileStream fs = new FileStream(pathFile, FileMode.Open);

            // Create Workbook instance holding reference to .xlsx file
            XSSFWorkbook workbook = new XSSFWorkbook(fs);

            // Get first/desired sheet from the workbook
            ISheet sheet = workbook.GetSheetAt(0);

            // Prepare variable
            List<string> result = new List<string>();
            DataFormatter formatter = new DataFormatter();
            JObject jsonObject = new JObject();
            JObject jsonObjectTemp = null;
            JObject finalObj = null;
            List<CellRangeAddress> listMergedRegion = new List<CellRangeAddress>();
            int maxMergedRegionRowLength = GetMaxMergedRegionRowLength(sheet, listMergedRegion);
            ICell parentCell = null;
            string parentStringCellValue = string.Empty;
            string currStringCellValue = string.Empty;
            int currRowIndex = 0;
            int currColIndex = 0;
            bool isReadHeader = true;
            string jsonBluePrint = string.Empty; // Use to set value by key when read data after read header

            SortMergedRegions(listMergedRegion, true);
            SortMergedRegions(listMergedRegion, false);

            AddMergeRegionToJSON(sheet, maxMergedRegionRowLength, listMergedRegion, jsonObject, 0, string.Empty);

            // Start reading
            int lastRowNum = sheet.LastRowNum;
            for (int i = 0; i <= lastRowNum; i++) {
                IRow row = sheet.GetRow(i);
                if (row == null) {
                    continue;
                }

                // For each row, iterate through all the columns
                List<ICell> cells = row.Cells;

                if (!string.Empty.Equals(jsonBluePrint)) {
                    jsonObjectTemp = JObject.Parse(jsonBluePrint);
                }

                foreach (ICell cell in cells) {
                    if (isReadHeader) {
                        if (cell.CellType != CellType.Blank) {
                            // Reading header -> ICell always string
                            currStringCellValue = cell.StringCellValue;
                            if ("(1)".Equals(currStringCellValue)) {
                                isReadHeader = false;

                                finalObj = new JObject {
                                    { Keywords.NAME_SURVEY, fileName },
                                    { Keywords.IMPORTER, importer },
                                    { Keywords.CREATED_AT, DateTime.Now.ToString(Configs.DATETIME_FORMAT, CultureInfo.InvariantCulture) },
                                    { Keywords.DATA, jsonObject }
                                };

                                jsonBluePrint = JsonConvert.SerializeObject(finalObj);
                                break;
                            }

                            currRowIndex = cell.RowIndex;
                            currColIndex = cell.ColumnIndex;
                            if (currRowIndex < _numberAddedRow) {
                                // These cell in row was added
                                continue;
                            }
                            if (currRowIndex != 0) {
                                parentCell = GetParentCell(sheet, currRowIndex, currColIndex);
                            }

                            if (!jsonObject.ContainsKey(currStringCellValue)) {
                                if (parentCell != null && parentCell.CellType != CellType.Blank) {
                                    parentStringCellValue = parentCell.StringCellValue;
                                }
                                AddKeyToJsonObject(jsonObject, parentStringCellValue, currStringCellValue, false);
                            }
                        }
                    } else {
                        SetValueToJSONBluePrint(jsonObjectTemp, formatter.FormatCellValue(cell), 0);
                    }
                }

                if (jsonObjectTemp != null) {
                    result.Add(JsonConvert.SerializeObject(jsonObjectTemp));
                }
            }
            fs.Close();

            return result;
        }

        public bool Insert(string jsonData) {
            BsonDocument document = BsonDocument.Parse(jsonData);

            return _mongoRepository.Insert(document);
        }
    }
}