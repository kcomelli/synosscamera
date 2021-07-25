namespace synosscamera.core.Model.Dto.Camera
{
    /// <summary>
    /// Camera status
    /// </summary>
    public enum CameraState
    {
        Normal = 1,
        Deleted = 2,
        Disconnected = 3,
        Unavailable = 4,
        Ready = 5,
        Inaccessible = 6,
        Disabled = 7,
        Unrecognized = 8,
        Setting = 9,
        ServerDisconnected = 10,
        Migrating = 11,
        Others = 12,
        StorageRemoved = 13,
        Stopping = 14,
        ConnectHistFailed = 15,
        Unauthorized = 16,
        RtspError = 17,
        NoVideo = 18
    }
}
