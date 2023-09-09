namespace DataAccess;

public interface IDataLoader<T>
{
    ICollection<T> LoadData();
}