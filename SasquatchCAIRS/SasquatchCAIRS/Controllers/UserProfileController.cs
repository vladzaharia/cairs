using System;
using System.Configuration;
using System.DirectoryServices;
using System.Linq;
using System.Web.Hosting;

namespace SasquatchCAIRS.Controllers {
    public class UserProfileController {
        private const string USER_DISPLAY_NAME = "displayName";
        private const string USER_EMAIL = "mail";

        /// <summary>
        /// Read-only UserProfileController singleton
        /// </summary>
        private static readonly UserProfileController _instance = new UserProfileController();

        private UserProfileController() {}

        public static UserProfileController instance {
            get {
                return _instance;
            }
        }

        public UserProfile getUserProfile(string username) {
            using (CAIRSDataContext db = new CAIRSDataContext()) {
                UserProfile user = db.UserProfiles.FirstOrDefault(u => 
                    u.UserName.ToLower() == username.ToLower());
                // Check if user already exists
                if (user == null) {
                    // Get User Info
                    var adUser = getADInformation(username.ToLower());

                    // Insert name into the profile table
                    db.UserProfiles.InsertOnSubmit(new UserProfile {
                        UserName = username,
                        UserFullName = adUser[0],
                        UserEmail = adUser[1],
                        UserStatus = true
                    });
                    db.SubmitChanges();
                }

                return db.UserProfiles.FirstOrDefault(u => 
                    u.UserName.ToLower() == username.ToLower());
            }
        }

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