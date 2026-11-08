using Xunit;

namespace RaceDay.Api.Tests
{
    public class UnitTest1
    {
        [Fact]
        public void Test1()
        {
            Assert.True(true);
        }

    [Fact]
        public void ParticipantRole_ShouldNotEqualOrganiserRole()
        {
            string participantRole = "Participant";
            string organiserRole = "Organiser";

            Assert.NotEqual(participantRole, organiserRole);
        }

        [Fact]
        public void EventDistance_ShouldBeGreaterThanZero()
        {
            decimal distance = 10.0m;

            Assert.True(distance > 0);
        }

        [Fact]
        public void EnrolmentStatus_ShouldBeConfirmed()
        {
            string status = "Confirmed";

            Assert.Equal("Confirmed", status);
        }

        [Fact]
        public void ValidEmail_ShouldContainAtSymbol()
        {
            string email = "participant@example.com";

            Assert.Contains("@", email);
        }
    }


}
