namespace TFG_Salty.Client.Services.AddressService
{
    public interface IAddressService
    {
        Task<Address> GetAddress();
        Task<Address> AddOrUpdate(Address address);
    }
}
