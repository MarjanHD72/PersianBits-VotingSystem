using System;
using System.Collections.Generic;
using System.Text;

namespace VotingSystem.Domain
{
    public class Poll
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public bool IsActive { get; set; }
    }
}
