namespace Application.Interfaces.IVideo
{
    public interface IVideoService
    {
        Task<VideoRoomResponse> CreateOrGetRoomAsync(long appointmentId, long doctorId, long patientId);
        Task<string> GetTokenAsync(string roomName, string userId, bool isOwner);
    }

    public class VideoRoomResponse
    {
        public string RoomUrl { get; set; } = string.Empty;
        public string RoomName { get; set; } = string.Empty;
        public string Token { get; set; } = string.Empty;
    }
}


