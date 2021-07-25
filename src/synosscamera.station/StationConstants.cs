namespace synosscamera.station
{
    /// <summary>
    /// Station constants
    /// </summary>
    public static class StationConstants
    {
        /// <summary>
        /// Defaults and common info
        /// </summary>
        public static class ApiDefaults
        {
            /// <summary>
            /// Token header name
            /// </summary>
            public const string TokenHeaderName = "X-SYNO-TOKEN";
        }
        /// <summary>
        /// Api names
        /// </summary>
        public static class ApiName
        {



            /// <summary>
            /// Retrieve Surveillance Station-related general information
            /// </summary>
            public const string StationInfo = "SYNO.SurveillanceStation.Info";
            /// <summary>
            /// Retrieve camera-related information
            /// </summary>
            public const string StationCamera = "SYNO.SurveillanceStation.Camera";
            /// <summary>
            /// Perform camera PTZ actions
            /// </summary>
            public const string StationPTZ = "SYNO.SurveillanceStation.PTZ";
            /// <summary>
            /// Control external recording of cameras
            /// </summary>
            public const string StationExternalRecording = "SYNO.SurveillanceStation.ExternalRecording";
            /// <summary>
            /// Query recording information
            /// </summary>
            public const string StationRecording = "SYNO.SurveillanceStation.Recording";
            /// <summary>
            /// Get information of defined E-Maps.
            /// </summary>
            public const string StationEmap = "SYNO.SurveillanceStation.Emap";
            /// <summary>
            /// Get Image of defined E-Maps.
            /// </summary>
            public const string StationEmapImage = "SYNO.SurveillanceStation.Emap.Image";
            /// <summary>
            /// Get authorized token of DS.
            /// </summary>
            public const string StationNotifications = "SYNO.SurveillanceStation.Notifications";
        }

        /// <summary>
        /// Station API data
        /// </summary>
        public static class Api
        {
            /// <summary>
            /// Api Info
            /// </summary>
            public static class ApiInfo
            {
                /// <summary>
                /// Discover all API information
                /// </summary>
                public const string Name = "SYNO.API.Info";

                /// <summary>
                /// Methods of api
                /// </summary>
                public static class Methods
                {
                    /// <summary>
                    /// Query data
                    /// </summary>
                    public const string Query = "query";
                }

                /// <summary>
                /// Api specific error codes
                /// </summary>
                public static class ErrorCodes
                {
                }
            }

            /// <summary>
            /// Api Info
            /// </summary>
            public static class ApiAuto
            {
                /// <summary>
                /// Perform session login and logout
                /// </summary>
                public const string Auth = "SYNO.API.Auth";

                /// <summary>
                /// Methods of api
                /// </summary>
                public static class Methods
                {
                    /// <summary>
                    /// Do session login
                    /// </summary>
                    public const string Login = "login";

                    /// <summary>
                    /// Do session logout
                    /// </summary>
                    public const string Logout = "logout";
                }

                /// <summary>
                /// Api specific error codes
                /// </summary>
                public static class ErrorCodes
                {
                    /// <summary>
                    /// The account parameter is not specified.
                    /// </summary>
                    public const int AccountNotSpecified = 101;
                    /// <summary>
                    /// Invalid password.
                    /// </summary>
                    public const int InvalidPassword = 400;
                    /// <summary>
                    /// Guest or disabled account.
                    /// </summary>
                    public const int GGuestOrDisabledAccount = 401;
                    /// <summary>
                    /// Permission denied.
                    /// </summary>
                    public const int PermissionDeined = 402;
                    /// <summary>
                    /// One time password not specified.
                    /// </summary>
                    public const int OneTimePasswordNotSpecified = 403;
                    /// <summary>
                    /// One time password authenticate failed.
                    /// </summary>
                    public const int OneTimePasswordAuthFailed = 404;
                    /// <summary>
                    /// App portal incorrect.
                    /// </summary>
                    public const int AppPortalIncorrect = 405;
                    /// <summary>
                    /// OTP code enforced.
                    /// </summary>
                    public const int OTPCodeEnforced = 406;
                    /// <summary>
                    /// Max Tries (if auto blocking is set to true).
                    /// </summary>
                    public const int MaxTriesReached = 407;
                    /// <summary>
                    /// Password Expired Can not Change.
                    /// </summary>
                    public const int PasswordExpiredCannotChange = 408;
                    /// <summary>
                    /// Password Expired.
                    /// </summary>
                    public const int PasswordExpired = 409;
                    /// <summary>
                    /// Password must change (when first time use or after reset password by admin).
                    /// </summary>
                    public const int PasswordMustBeChanged = 410;
                    /// <summary>
                    /// Account Locked (when account max try exceed).
                    /// </summary>
                    public const int AccountLocked = 411;
                }
            }
        }


        /// <summary>
        /// Error codes
        /// </summary>
        public static class ErrorCodes
        {
            /// <summary>
            /// Common codes
            /// </summary>
            public static class Common
            {
                /// <summary>
                /// Unknown error occured
                /// </summary>
                public const int UnknownError = 100;
                /// <summary>
                /// Invalid parameter and/or value supplied
                /// </summary>
                public const int InvalidParameters = 101;
                /// <summary>
                /// The requested api does not exist
                /// </summary>
                public const int ApiDoesNotExist = 102;
                /// <summary>
                /// The called method does not exist
                /// </summary>
                public const int MethodDoesNotExist = 103;
                /// <summary>
                /// This version of the api is not supported
                /// </summary>
                public const int ApiVersionNotSupported = 104;
                /// <summary>
                /// User has insufficient priviliges
                /// </summary>
                public const int InsufficientUserPriviliges = 105;
                /// <summary>
                /// Connection timed out
                /// </summary>
                public const int ConnectionTimeout = 106;
                /// <summary>
                /// Multiple logins of this user detected
                /// </summary>
                public const int MultipleLoginDetected = 107;
            }
        }

    }
}
