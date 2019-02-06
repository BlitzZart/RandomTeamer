using Microsoft.Win32;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows.Input;
using System.Xml.Serialization;

namespace RandomTeamer {
    class MemberViewModel : INotifyPropertyChanged{
        #region construction
        public MemberViewModel() {
            _allMembers = new ObservableCollection<Member>();
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
                OnPropertyChanged("CurrentName");
            }
        }
        public ObservableCollection<Member> AllMembers {
            get {
                return _allMembers;
            }

            set {
                _allMembers = value;
                OnPropertyChanged("AllMembers");
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

            int teamNumber = 1;
            foreach (MemberGroup item in newTeams)
            {
                item.Number = teamNumber;
                Teams.Add(item);
                teamNumber++;
            }
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
            if (_allMembers.Any(p => p.Name.ToLower() == _currentName.ToLower())
                || _currentName == "[already exists]")
                return true;

            return false;
        }

        void AddMemberExecute() {
            if (_currentName == null || _currentName == string.Empty)
                return;

            _currentName = _currentName.Replace(";", "");

            if (!AlreadyExists())
            {
                _allMembers.Add(new Member(_currentName));
                CurrentName = string.Empty;
            }
            else
            {
                CurrentName = "[already exists]";
            }
        }
        bool CanAddMemberExecute() {
            return true;
        }
        public ICommand AddMember {
            get { return new RelayCommand(AddMemberExecute, CanAddMemberExecute); }
        }

        // ------------------------------------
        // Save/Load
        const string CSV_FILETYPE = "rnd";
        const char CSV_DELIMITER = ';';
        void SaveNamesExecute()
        {
            string csv = "";
            foreach (Member member in _allMembers)
            {
                csv += member.Name.ToString();
                csv += CSV_DELIMITER;
            }

            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.InitialDirectory = AppDomain.CurrentDomain.BaseDirectory;
            saveFileDialog.Filter += "Name Files (*.rnd)|*.rnd|All Files (*.*)|*.*";       
            saveFileDialog.DefaultExt = "rnd";
            saveFileDialog.AddExtension = true;


            if (saveFileDialog.ShowDialog() == true)
            {
                File.WriteAllText(saveFileDialog.FileName, csv);
            }
        }
        bool CanSaveNamesExecute()
        {
            return true;
        }
        public ICommand SaveNames {
            get { return new RelayCommand(SaveNamesExecute, CanSaveNamesExecute); }
        }

        void LoadNamesExecute()
        {
            OpenFileDialog loadFileDialog = new OpenFileDialog();
            loadFileDialog.InitialDirectory = AppDomain.CurrentDomain.BaseDirectory;
            loadFileDialog.Filter += "Teams Files (*.rnd)|*.rnd|All Files (*.*)|*.*";
            loadFileDialog.DefaultExt = "rnd";
            loadFileDialog.AddExtension = true;

            string csv = "";
            if (loadFileDialog.ShowDialog() == true)
            {
                csv = File.ReadAllText(loadFileDialog.FileName);
            }

            string[] names = csv.Split(CSV_DELIMITER);
            string current = _currentName;

            AllMembers.Clear();
            foreach (string name in names)
            {
                _currentName = name;
                AddMemberExecute();
            }
            _currentName = current;
        }
        bool CanLoadNamesExecute()
        {
            return true;
        }
        public ICommand LoadNames {
            get { return new RelayCommand(LoadNamesExecute, CanLoadNamesExecute); }
        }

        void SaveTeamsExecute()
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.InitialDirectory = AppDomain.CurrentDomain.BaseDirectory;
            saveFileDialog.Filter += "Team Files (*.rndt)|*.rndt|All Files (*.*)|*.*";
            saveFileDialog.DefaultExt = "rnd";
            saveFileDialog.AddExtension = true;

            List<List<string>> teamList = new List<List<string>>();

            foreach(MemberGroup mg in _teams)
            {
                List<string> newTeam = new List<string>();
                teamList.Add(newTeam);

                foreach(Member m in mg.Users)
                {
                    newTeam.Add(m.Name);
                }
            }

            if (saveFileDialog.ShowDialog() == true)
            {
                XmlSerializer xs = new XmlSerializer(typeof(List<List<string>>));
                TextWriter tw = new StreamWriter(saveFileDialog.FileName);
                xs.Serialize(tw, teamList);
            }
        }
        bool CanSaveTeamsExecute()
        {
            return true;
        }
        public ICommand SaveTeams {
            get { return new RelayCommand(SaveTeamsExecute, CanSaveTeamsExecute); }
        }

        void LoadTeamsExecute()
        {
            OpenFileDialog loadFileDialog = new OpenFileDialog();
            loadFileDialog.InitialDirectory = AppDomain.CurrentDomain.BaseDirectory;
            loadFileDialog.Filter += "Name Files (*.rndt)|*.rndt|All Files (*.*)|*.*";
            loadFileDialog.DefaultExt = "rnd";
            loadFileDialog.AddExtension = true;


            List<List<string>> teamList = new List<List<string>>();

            if (loadFileDialog.ShowDialog() == true)
            {
                using (StreamReader sr = new StreamReader(loadFileDialog.FileName))
                {
                    XmlSerializer xs = new XmlSerializer(typeof(List<List<string>>));
                    teamList = (List<List<string>>)xs.Deserialize(sr);
                }
            }

            List<string> names = new List<string>();

            _teams.Clear();
            foreach (List<string> team in teamList)
            {
                MemberGroup newTeam = new MemberGroup();
                _teams.Add(newTeam);
                foreach(string member in team)
                {
                    newTeam.Users.Add(new Member(member));
                    names.Add(member);
                }
            }

            // also load names
            string current = _currentName;
            AllMembers.Clear();
            foreach (string name in names)
            {
                _currentName = name;
                AddMemberExecute();
            }
            _currentName = current;
        }
        bool CanLoadTeamsExecute()
        {
            return true;
        }
        public ICommand LoadTeams {
            get { return new RelayCommand(LoadTeamsExecute, CanLoadTeamsExecute); }
        }
        #endregion
    }
}