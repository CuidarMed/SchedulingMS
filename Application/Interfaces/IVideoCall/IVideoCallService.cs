using Application.DTOs;

namespace Application.Interfaces.IVideoCall
{
    public interface IVideoCallService
    {
        Task<VideoCallRoomResponse> CreateOrGetRoomAsync(long appointmentId, long doctorId, long patientId, string? userName = null, string? userType = null);
        Task<string> GetTokenAsync(string roomName, string userId, bool isOwner);
    }
}
