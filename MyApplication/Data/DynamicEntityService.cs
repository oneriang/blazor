// DynamicEntityService.cs

namespace MyApplication.Data
{
    public class DynamicEntityService
    {
        private DynamicEntity1 dynamicEntity;

        public DynamicEntity1 GetDynamicEntity()
        {
            return dynamicEntity;
        }

        public void SetDynamicEntity(DynamicEntity1 newDynamicEntity)
        {
            dynamicEntity = newDynamicEntity;
        }

        private string _tableName = "";

        public string TableName { get => _tableName; set => _tableName = value; }
    }
}
