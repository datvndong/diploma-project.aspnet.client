namespace CentralizedDataSystem.Models {
    public class Role {
        public string Id { get; set; }
        public string Title { get; set; }
        public string GroupCode { get; set; }
        public string ParentCode { get; set; }
        public Role(string _id, string title, string groupCode, string parentCode) {
            this.Id = _id;
            this.Title = title;
            this.GroupCode = groupCode;
            this.ParentCode = parentCode;
        }

        public Role() { }
    }
}