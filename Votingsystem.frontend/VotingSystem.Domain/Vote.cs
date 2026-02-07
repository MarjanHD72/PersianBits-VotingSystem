using System;
using System.Collections.Generic;
using System.Text;

namespace VotingSystem.Domain
{
    public class Vote
    {
        public int Id { get; set; }
        public int PollId { get; set; }
        public int UserId { get; set; }
        public string Answer { get; set; }
    }
}
