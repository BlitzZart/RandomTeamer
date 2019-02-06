using System;
using System.Collections.Generic;
using System.Linq;

namespace RandomTeamer {
    public static class RandomTeamGenerator {
        static List<MemberGroup> _teams;
        static int _teamSize;
        public static List<MemberGroup> GetTeams(List<Member> users, int teamSize) {
            _teams = new List<MemberGroup>();
            _teamSize = teamSize;

            MakeTeam(users);

            return _teams;
        }

        // recursive find teams
        public static void MakeTeam(List<Member> users) {
            // return - all users are mapped
            if (users.Count == 0)
                return;

            int teamCounter = 1;

            Random random = new Random();
            // get first user of the new team
            Member m1 = users[random.Next(0, users.Count - 1)];
            // remove this user from the pool of all users
            users.Remove(m1);
            // get all valid users left in the pool
            var tUsers = users.Except(m1.IllegalMembers);
            List<Member> validUsers = new List<Member>();
            foreach (Member item in tUsers)
                validUsers.Add(item);

            // create new team
            MemberGroup team = new MemberGroup();
            team.Number = teamCounter++;

            // add firts user
            team.Users.Add(m1);

            // !!! not enough users left !!!
            if (users.Count <= _teamSize - 1) {
                foreach (Member item in users) {
                    team.Users.Add(item);
                }
                _teams.Add(team);
                return; // 
            }

            // take x (according to team size) random users from remaining valid users
            for (int i = 0; i < _teamSize - 1; i++) {
                int rndIndex = 0;

                if (validUsers.Count > 1)
                    rndIndex = random.Next(0, validUsers.Count - 1);

                if (validUsers.Count > rndIndex)
                {
                    team.Users.Add(validUsers[rndIndex]);
                    users.Remove(validUsers[rndIndex]);
                    validUsers.Remove(validUsers[rndIndex]);
                } 
            }
            // finally add the new team
            _teams.Add(team);

            MakeTeam(users);
        }
    }
}
