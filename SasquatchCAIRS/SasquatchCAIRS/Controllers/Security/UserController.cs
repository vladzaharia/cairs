using System;
using System.Collections.Generic;
using System.Configuration;
using System.DirectoryServices;
using System.Linq;
using System.Web.Hosting;
using Microsoft.Web.WebPages.OAuth;
using WebMatrix.WebData;

namespace SasquatchCAIRS.Controllers.Security {
    public class UserController {
        private const string USER_DISPLAY_NAME = "displayName";
        private const string USER_EMAIL = "mail";

        private CAIRSDataContext _db = new CAIRSDataContext();

        /// <summary>
        /// Log into the system and return the user profile for the user.
        /// </summary>
        /// <param name="username">The username to login with</param>
        /// <returns>The UserProfile of the user</returns>
        public UserProfile loginAndGetUserProfile(string username) {
            // Check if the user exists, and create them otherwise.
            if (!WebSecurity.UserExists(username)) {
                // Register the User as a local user in the database w/ AD info
                string[] adUser = getADInformation(username.ToLower());
                WebSecurity.CreateUserAndAccount(username, username, new {
                    UserFullName = adUser[0],
                    UserEmail = adUser[1],
                    UserStatus = true
                });
            } else if (!WebSecurity.IsAuthenticated) {
                // Update User Information in Database on Login
                string[] adUser = getADInformation(username.ToLower());
                UserProfile user = _db.UserProfiles.FirstOrDefault(u =>
                                                u.UserName.ToLower() ==
                                                username.ToLower());

                if (user != null) {
                    user.UserFullName = adUser[0];
                    user.UserEmail = adUser[1];
                    _db.SubmitChanges();
                }
            }

            // Login to the system properly
            WebSecurity.Login(username, username, true);

            return getUserProfile(username);
        }

        /// <summary>
        ///     Get the User Profile for the entered username.
        /// </summary>
        /// <param name="username">Username to look for.</param>
        /// <returns>The UserProfile for the user.</returns>
        public UserProfile getUserProfile(string username) {
            return _db.UserProfiles.FirstOrDefault(u =>
                                                   u.UserName.ToLower() ==
                                                   username.ToLower());
        }

        /// <summary>
        /// Gets the groups for the username specified.
        /// </summary>
        /// <param name="username">Username to get groups for.</param>
        /// <returns>An IEnumerable for the UserGroups</returns>
        public IEnumerable<UserGroup> getUserGroups(string username) {
            if (String.IsNullOrEmpty(username)) {
                return new List<UserGroup>();
            } 

            UserProfile profile = getUserProfile(username);

            return profile.UserGroups.Select(grps => grps.UserGroup).ToList();
        }

        /// <summary>
        ///     Get the Active Directory information for the user.
        /// </summary>
        /// <param name="loginUsername">Username to search for.</param>
        /// <returns>An array containing the user information from AD.</returns>
        private String[] getADInformation(string loginUsername) {
            var adInfo = new String[2];
            using (HostingEnvironment.Impersonate()) {
                using (var de = new DirectoryEntry(
                    ConfigurationManager.ConnectionStrings["ADConn"]
                        .ConnectionString)) {
                    using (var adSearch = new DirectorySearcher(de)) {
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