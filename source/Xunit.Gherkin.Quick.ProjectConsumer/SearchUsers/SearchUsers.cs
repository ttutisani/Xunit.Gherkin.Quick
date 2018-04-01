using Gherkin.Ast;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit.Abstractions;

namespace Xunit.Gherkin.Quick.ProjectConsumer.SearchUsers
{
    [FeatureFile("./SearchUsers/SearchUsers.feature")]
    public class SearchUsers : Feature
    {
        private readonly List<UserModel> _users = new List<UserModel>();
        private UserModel _userResult;

        public SearchUsers(ITestOutputHelper output) : base(output)
        { }

        [Given(@"there are users:")]
        public void There_are_users(DataTable users)
        {
            foreach(var row in users.Rows.Skip(1)) // Skip the header row
            {
                var cells = row.Cells.ToList();
                _users.Add(new UserModel { Username = cells[0].Value, Email = cells[1].Value });
            }
        }

        [When(@"I search for '([a-zA-Z0-9]+)'")]
        public void I_search_for(string username)
        {
            _userResult = _users.FirstOrDefault(u => u.Username == username);
        }

        [Then(@"the result should have an email of '([a-zA-Z0-9\@\.]+)'")]
        public void The_result_should_have_email(string expectedEmail)
        {
            Assert.Equal(expectedEmail, _userResult?.Email);
        }

        private class UserModel
        {
            public string Username { get; set; }
            public string Email { get; set; }
        }
    }
}
