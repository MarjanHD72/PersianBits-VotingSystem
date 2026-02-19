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
            if (string.IsNullOrWhiteSpace(title))
            {
                _logging.LogWarning("Attempt to create poll with empty title.");
                return;
            }

            try
            {
                

                _logging.LogInfo($"Poll created successfully: {title}");
            }
            catch (Exception ex)
            {
                _logging.LogError($"Error while creating poll '{title}': {ex.Message}");
                throw;
            }
        }

        public void Vote(int pollId, int userId)
        {
            _logging.LogInfo($"User {userId} voted in poll {pollId}");
        }
    }

}
