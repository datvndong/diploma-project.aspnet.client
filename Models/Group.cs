namespace CentralizedDataSystem.Models {
    public class Group {
        public string Id { get; set; }
        public string IdGroup { get; set; }
        public string Name { get; set; }
        public string IdParent { get; set; }
        public int Status { get; set; }
        public string NameParent { get; set; }
        public int NumberOfChildrenGroup { get; set; }
        public Group(string id, string idGroup, string name, string idParent, string nameParent,
            int numberOfChildrenGroup) {
            this.Id = id;
            this.IdGroup = idGroup;
            this.Name = name;
            this.IdParent = idParent;
            this.NameParent = nameParent;
            this.NumberOfChildrenGroup = numberOfChildrenGroup;
        }

        public Group(string id, string idGroup, string name, string idParent, string nameParent) {
            this.Id = id;
            this.IdGroup = idGroup;
            this.Name = name;
            this.IdParent = idParent;
            this.NameParent = nameParent;
        }

        public Group() { }
    }
}