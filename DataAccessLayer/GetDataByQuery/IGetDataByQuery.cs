namespace AIDBDataFeatch.DataAccessLayer.GetDataByQuery
{
    public interface IGetDataByQuery
    {
        Task GetDatafromDB(string query);
    }
}
