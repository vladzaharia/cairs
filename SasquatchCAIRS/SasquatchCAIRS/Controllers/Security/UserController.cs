using System;
using System.Collections.Generic;
using System.Configuration;
using System.DirectoryServices;
using System.Linq;
using System.Web.Hosting;

namespace SasquatchCAIRS.Controllers.Security {
    public class UserController {
        private const string USER_DISPLAY_NAME = "displayName";
        private const string USER_EMAIL = "mail";

        private CAIRSDataContext _db = new CAIRSDataContext();

        /// <summary>
        /// Get the User Profile for the entered username.
        /// </summary>
        /// <param name="username">Username to look for.</param>
        /// <returns>The UserProfile for the user.</returns>
        public UserProfile getUserProfile(string username) {
            UserProfile user = _db.UserProfiles.FirstOrDefault(u => 
                    u.UserName.ToLower() == username.ToLower());
                // Check if user already exists
                if (user == null) {
                    // Get User Info
                    var adUser = getADInformation(username.ToLower());

                    // Insert name into the profile table
                    _db.UserProfiles.InsertOnSubmit(new UserProfile {
                        UserName = username,
                        UserFullName = adUser[0],
                        UserEmail = adUser[1],
                        UserStatus = true
                    });
                    _db.SubmitChanges();
                }

                return _db.UserProfiles.FirstOrDefault(u => 
                    u.UserName.ToLower() == username.ToLower());
        }

        public IEnumerable<UserGroup> getUserGroups(string username) {
            var profile = getUserProfile(username);

            return profile.UserGroups.Select(grps => grps.UserGroup).ToList();
        }

        /// <summary>
        /// Get the Active Directory information for the user.
        /// </summary>
        /// <param name="loginUsername">Username to search for.</param>
        /// <returns>An array containing the user information from AD.</returns>
        private String[] getADInformation(string loginUsername) {
            String[] adInfo = new String[2];
            using (HostingEnvironment.Impersonate()) {
                using (DirectoryEntry de = new DirectoryEntry(
                    ConfigurationManager.ConnectionStrings["ADConn"].ConnectionString)) {

                    using (DirectorySearcher adSearch = new DirectorySearcher(de)) {
                        adSearch.PropertiesToLoad.Add(USER_DISPLAY_NAME);
                        adSearch.PropertiesToLoad.Add(USER_EMAIL);
                        String username = loginUsername.Split('\\')[1];

                        adSearch.Filter = "(sAMAccountName=" + username + ")";
                        SearchResult adSearchResult = adSearch.FindOne();

                        // Return Array with Blank Values if user not found
                        if (adSearchResult == null) {
                            adInfo[0] = "";
                            adInfo[1] = "";
                            return adInfo;
                        }

                        if (adSearchResult
                            .Properties[USER_DISPLAY_NAME]
                            .Count == 0) {
                            adInfo[0] = "";
                        } else {
                            adInfo[0] = adSearchResult
                                .Properties[USER_DISPLAY_NAME][0]
                                .ToString();
                        }

                        if (adSearchResult
                            .Properties[USER_EMAIL]
                            .Count == 0) {
                            adInfo[1] = "";
                        } else {
                            adInfo[1] = adSearchResult
                                .Properties[USER_EMAIL][0]
                                .ToString();
                        }

                        return adInfo;
                    }
                }
            }
        }

    }
}