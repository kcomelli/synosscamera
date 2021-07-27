namespace synosscamera.station.Model.ApiInfo
{
    /// <summary>
    /// Recording status of camera
    /// </summary>
    public enum RecordingStatus
    {
        /// <summary>
        /// No recording actrive
        /// </summary>
        None = 0,
        /// <summary>
        /// Camera is continously recording
        /// </summary>
        ContinouesRecording = 1,
        /// <summary>
        /// Camera records because of motion detection
        /// </summary>
        MotionDetectionRecording = 2,
        /// <summary>
        /// Camera records in case of alarm
        /// </summary>
        AlarmRecording = 3,
        /// <summary>
        /// Custom recoring is active
        /// </summary>
        CustomRecording = 4,
        /// <summary>
        /// Manual recording is active
        /// </summary>
        ManualRecording = 5,
        /// <summary>
        /// External recording is active
        /// </summary>
        ExternalRecording = 6,
        /// <summary>
        /// Analytic recording is active
        /// </summary>
        AnalyticsRecording = 7,
        /// <summary>
        /// Edge recording is active
        /// </summary>
        EdgeRecording = 8,
        /// <summary>
        /// An action rule triggered recording
        /// </summary>
        ActionRuleRecording = 9,
        /// <summary>
        /// Advances continous recording is active
        /// </summary>
        AdvancedContinouesRecording = 10
    }
}
