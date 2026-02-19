using System;
using System.Collections.Generic;
using System.Text;

namespace VotingSystem.Application.Services
{
    public interface IPollService
    {
        void CreatePoll(string title);
        void Vote(int pollId, int userId);
    }
}
