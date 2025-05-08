namespace courseworkTMAS
{
    internal class Program
    {
        static void Main(string[] args)
        {
            BuildingTable buildingTable1 = new BuildingTable(new DataSet(5.00, 4.00, 5, 11, 1));
            buildingTable1.BuildTable();
          
            BuildingTable buildingTable2 = new BuildingTable(new DataSet(5.00, 4.00, 1, 51, 1));
            buildingTable2.BuildTable();
            
            BuildingTable buildingTable3 = new BuildingTable(new DataSet(5.00, 4.00, 5, 11, 3));
            buildingTable3.BuildTable();
           
            BuildingTable buildingTable4 = new BuildingTable(new DataSet(5.00, 4.00, 1, 51, 3));
            buildingTable4.BuildTable();
        }
    }
}
