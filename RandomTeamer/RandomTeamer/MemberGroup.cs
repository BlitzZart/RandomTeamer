using System.Collections.ObjectModel;

namespace RandomTeamer {
    public class MemberGroup {
        #region static
        public static int count = 0;
        #endregion

        #region members
        private int _number = 0;
        private ObservableCollection<Member> _users;
        #endregion

        #region property
        public int Number {
            get {
                return _number;
            }

            set {
                _number = value;
            }
        }
        public ObservableCollection<Member> Users {
            get {
                return _users;
            }

            set {
                _users = value;
            }
        }
        #endregion

        #region construction
        public MemberGroup() {
            _number = count++;
            Users = new ObservableCollection<Member>();
        }
        #endregion
    }
}