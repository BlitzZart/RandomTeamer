using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;

namespace RandomTeamer {
    class MemberViewModel : INotifyPropertyChanged{
        #region construction
        public MemberViewModel() {
            _allMembers = new ObservableCollection<Member>();
            //_allMembers = new ObservableCollection<Member> { new Member("Bob"), new Member("Gerti"), new Member("Derpo"), new Member("Gill") };
            _selectedItems = new ObservableCollection<Member>();
            _invalidGroups = new ObservableCollection<MemberGroup>();
            _teams = new ObservableCollection<MemberGroup>();
        }
        #endregion

        #region members
        ObservableCollection<Member> _allMembers;
        ObservableCollection<Member> _selectedItems;
        ObservableCollection<MemberGroup> _invalidGroups;
        ObservableCollection<MemberGroup> _teams;
        MemberGroup _selectedGroup;
        int _selectedTeamSize;
        string _currentName;
        #endregion

        #region properties
        public string CurrentName {
            get {
                return _currentName;
            }

            set {
                _currentName = value;
            }
        }
        public ObservableCollection<Member> AllMembers {
            get {
                return _allMembers;
            }

            set {
                _allMembers = value;
            }
        }
        public ObservableCollection<Member> SelectedItems {
            get {
                return _selectedItems;
            }

            set {
                _selectedItems = value;
                OnPropertyChanged("SelectedItems");
            }
        }
        public ObservableCollection<MemberGroup> InvalidGroups {
            get {
                return _invalidGroups;
            }

            set {
                _invalidGroups = value;
            }
        }
        public MemberGroup SelectedGroup {
            get {
                return _selectedGroup;
            }

            set {
                _selectedGroup = value;
                OnPropertyChanged("SelectedGroup");
            }
        }
        public int SelectedTeamSize {
            get {
                return _selectedTeamSize;
            }

            set {
                _selectedTeamSize = value + 1;
            }
        }
        public ObservableCollection<MemberGroup> Teams {
            get {
                return _teams;
            }

            set {
                _teams = value;
                OnPropertyChanged("Teams");
            }
        }
        #endregion

        #region INotifyPropertyChanged Members
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string e) {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(e));
        }
        #endregion

        #region commands
        void OnGenerateRandomTeamsExecute() {
            List<MemberGroup> newTeams = RandomTeamGenerator.GetTeams(new List<Member>(AllMembers), SelectedTeamSize);
            Teams.Clear();
            foreach (MemberGroup item in newTeams)
                Teams.Add(item);
        }
        public ICommand GenerateRandomTeamsCommand {
            get { return new RelayCommand(OnGenerateRandomTeamsExecute, () => true); }
        }

        void OnRemoveSelectedInGroupsExecute() {
            if (SelectedGroup == null)
                return;
            // remove invalid users from all users in group
            foreach (Member item in SelectedGroup.Users)
                item.IllegalMembers.Clear();

            _invalidGroups.Remove(SelectedGroup);
        }
        public ICommand RemoveSelectedInGroupsCommand {
            get { return new RelayCommand(OnRemoveSelectedInGroupsExecute, () => true); }
        }

        void OnMakeInvalidGroupExecute() {
            MemberGroup newGroup = new MemberGroup();
            foreach (Member user in _selectedItems) {
                newGroup.Users.Add(user);
            }
            if (newGroup.Users.Count > 1) {
                // store invalid users within users
                for (int o = 0; o < newGroup.Users.Count; o++) {

                    Member m = newGroup.Users[o];

                    for (int i = 0; i < newGroup.Users.Count; i++) {
                        if (newGroup.Users[i] != m)
                            m.IllegalMembers.Add(newGroup.Users[i]);
                    }
                }
                // add to list
                _invalidGroups.Add(newGroup);
            }

        }
        public ICommand MakeInvalidGroupCommand {
            get { return new RelayCommand(OnMakeInvalidGroupExecute, () => true); }
        }

        void OnRemoveSelectedInAllUsersExecute() {
            for (int i = _selectedItems.Count -1; i >= 0; i--) {
                _allMembers.Remove(_selectedItems[i]);
            }
        }
        public ICommand RemoveSelectedInAllUsersCommand {
           get { return new RelayCommand(OnRemoveSelectedInAllUsersExecute, () => true); }
        }

        // check if member already exists
        bool AlreadyExists() {
            if (_allMembers.Any(p => p.Name.ToLower() == _currentName.ToLower()))
                return true;

            return false;
        }

        void AddMemberExecute() {
            if (_currentName == null || _currentName == string.Empty)
                return;

            if (!AlreadyExists())
                _allMembers.Add(new Member(_currentName));
        }
        bool CanAddMemberExecute() {
            return true;
        }
        public ICommand AddMember {
            get { return new RelayCommand(AddMemberExecute, CanAddMemberExecute); }
        }
        #endregion
    }
}