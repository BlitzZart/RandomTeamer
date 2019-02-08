using System;
using System.Collections.Generic;
using System.Linq;

namespace RandomTeamer {
    public static class RandomTeamGenerator {
        static List<MemberGroup> _teams;
        static int _teamSize;

        private static Random shuffleRnd = new Random();

        public static List<MemberGroup> GetTeams(List<Member> users, int teamSize) {
            _teams = new List<MemberGroup>();
            _teamSize = teamSize;

            List<Member> workOnList = new List<Member>(users);
            workOnList.Shuffle();

            MakeTeam(workOnList);

            // shuffle teams just prevents that first member found with invalid members
            // appears always first
            _teams.Shuffle();

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
            // first look for members with illegal group
            Member m1 = GetMemberWithIllegal(users);
            // take random if no illegal groups left
            if (m1 == null)
            {
                m1 = users[random.Next(0, users.Count - 1)];
            }

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

        private static Member GetMemberWithIllegal(List<Member> users)
        {
            Member next = null;
            foreach(Member item in users)
            {
                if (item.IllegalMembers.Count > 0)
                {
                    next = item;
                    break;
                }
            }

            return next;
        }

        private static void Shuffle<T>(this IList<T> list)
        {
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = shuffleRnd.Next(n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }
    }
}
