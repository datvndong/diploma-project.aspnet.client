namespace CentralizedDataSystem.Models {
    public class User {
        public string Email { get; set; }
        public string Name { get; set; }
        public string Password { get; set; }
        public string Token { get; set; }
        public string IdGroup { get; set; }
        public string Gender { get; set; }
        public string PhoneNumber { get; set; }
        public string Address { get; set; }
        public int Status { get; set; }
        public string NameGroup { get; set; }
        public string Id { get; set; }
        public int ReportsNumber { get; set; }
        public int SubmittedNumber { get; set; }
        public bool IsAdmin { get; set; }

        public User(string email, string name, string token, string idGroup, string gender, string phoneNumber,
            string address, string id, bool isAdmin) {
            this.Email = email;
            this.Name = name;
            this.Token = token;
            this.IdGroup = idGroup;
            this.Gender = gender;
            this.PhoneNumber = phoneNumber;
            this.Address = address;
            this.Id = id;
            this.IsAdmin = isAdmin;
        }

        public User(string id, string email, string name, string nameGroup, string gender, string phoneNumber, string address) {
            this.Id = id;
            this.Email = email;
            this.Name = name;
            this.NameGroup = nameGroup;
            this.Gender = gender;
            this.PhoneNumber = phoneNumber;
            this.Address = address;
        }

        public User(string email, string name, string token, string idGroup) {
            this.Email = email;
            this.Name = name;
            this.Token = token;
            this.IdGroup = idGroup;
        }

        public User(string email, string name, string token, bool isAdmin) {
            this.Email = email;
            this.Name = name;
            this.Token = token;
            this.IsAdmin = isAdmin;
        }

        public User(string email) {
            this.Email = email;
        }

        public User() { }
    }
}