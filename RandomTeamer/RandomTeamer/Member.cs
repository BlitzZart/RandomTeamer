
using System.Collections.Generic;

namespace RandomTeamer {
    public class Member {
        #region construction
        public Member(string name) {
            _name = name;
            _team = 0;
            _illegalMembers = new List<Member>();
        }
        #endregion

        #region members
        string _name;
        int _team;
        List<Member> _illegalMembers;
        #endregion

        #region properties
        public string Name {
            get {
                return _name;
            }

            set {
                _name = value;
            }
        }

        public int Team {
            get {
                return _team;
            }

            set {
                _team = value;
            }
        }

        public List<Member> IllegalMembers {
            get {
                return _illegalMembers;
            }

            set {
                _illegalMembers = value;
            }
        }
        #endregion
    }
}