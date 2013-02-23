using SasquatchCAIRS.Models;
using System;
using System.DirectoryServices;
using System.Linq;
using System.Web.Hosting;

namespace SasquatchCAIRS.Controllers {
    public class UserProfileManager {
        public const string USER_DISPLAY_NAME = "displayName";
        public const string USER_EMAIL = "mail";

        public UserProfile getUserProfile(string username) {
            using (UsersContext db = new UsersContext()) {
                UserProfile user = db.UserProfiles.FirstOrDefault(u => 
                    u.UserName.ToLower() == username.ToLower());
                // Check if user already exists
                if (user == null) {
                    // Get User Info
                    String[] ADUser = getADInformation(username.ToLower());

                    // Insert name into the profile table
                    db.UserProfiles.Add(new UserProfile {
                        UserName = username,
                        UserFullName = ADUser[0],
                        UserEmail = ADUser[1],
                        UserStatus = true
                    });
                    db.SaveChanges();
                }

                return db.UserProfiles.FirstOrDefault(u => 
                    u.UserName.ToLower() == username.ToLower());
            }
        }

        public String[] getADInformation(string loginUsername) {
            String[] adInfo = new String[2];
            using (HostingEnvironment.Impersonate()) {
                using (DirectoryEntry de = new DirectoryEntry(
                    "LDAP://sasquatch.cloudapp.net/" +
                    "CN=Users,DC=sasquatch,DC=cloudapp,DC=net")) {

                    using (DirectorySearcher adSearch = new DirectorySearcher(de)) {
                        adSearch.PropertiesToLoad.Add(UserProfileManager.USER_DISPLAY_NAME);
                        adSearch.PropertiesToLoad.Add(UserProfileManager.USER_EMAIL);
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
                            .Properties[UserProfileManager.USER_DISPLAY_NAME]
                            .Count == 0) {
                            adInfo[0] = "";
                        } else {
                            adInfo[0] = adSearchResult
                                .Properties[UserProfileManager.USER_DISPLAY_NAME][0]
                                .ToString();
                        }

                        if (adSearchResult
                            .Properties[UserProfileManager.USER_EMAIL]
                            .Count == 0) {
                            adInfo[1] = "";
                        } else {
                            adInfo[1] = adSearchResult
                                .Properties[UserProfileManager.USER_EMAIL][0]
                                .ToString();
                        }

                        return adInfo;
                    }
                }
            }
        }

    }
}