using System;
using System.Collections.Generic;
using System.Text;
using VotingSystem.Application.Services.Logging;

namespace VotingSystem.Application.Services
{
    public class PollService : IPollService
    {
        private readonly ILoggingService _logging;

        public PollService(ILoggingService logging)
        {
            _logging = logging;
        }

        public void CreatePoll(string title)
        {
            _logging.LogInfo($"Poll created: {title}");
        }

        public void Vote(int pollId, int userId)
        {
            _logging.LogInfo($"User {userId} voted in poll {pollId}");
        }
    }

}
