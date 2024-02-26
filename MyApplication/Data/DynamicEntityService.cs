// DynamicEntityService.cs

namespace MyApplication.Data
{
    public class DynamicEntityService
    {
        // private DynamicEntity1 dynamicEntity;

        // public DynamicEntity1 GetDynamicEntity()
        // {
        //     return dynamicEntity;
        // }

        // public void SetDynamicEntity(DynamicEntity1 newDynamicEntity)
        // {
        //     dynamicEntity = newDynamicEntity;
        // }

        private DynamicEntity1 _dynamicEntity;
        public DynamicEntity1 DynamicEntity
        {
            get => _dynamicEntity;
            set => _dynamicEntity = value;
        }

        private string _tableName = "";

        public string TableName { get => _tableName; set => _tableName = value; }

        private TableInfo _myTableInfo;

        public TableInfo MyTableInfo { get => _myTableInfo; set => _myTableInfo = value; }

        private Dictionary<string, object> _selectedItem1 = null;

        public Dictionary<string, object> SelectedItem1 { get => _selectedItem1; set => _selectedItem1 = value; } 

    }
}
