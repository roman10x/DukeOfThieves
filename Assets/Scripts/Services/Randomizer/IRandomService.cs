namespace DukeOfThieves.Services.Randomizer
{
    public interface IRandomService: IService
    {
        int Next(int minValue, int maxValue);
    }
}