namespace CRUDServices.BindingModel
{
    public class ResultBase
    {
        public ResultBase()
        {
            ResultCode = "1000";
            ResultMessage = "Success";
            ResultCountData = 0;
        }

        public string ResultCode { get; set; }
        public string ResultMessage { get; set; }
        public int? ResultCountData { get; set; }
    }

    public class ResultBase<T> : ResultBase
    {
        
        public T? Data { get; set; }
        
    }

    public class ResultBasePaginated<T> : ResultBase<T>
    {
        public Paginated? Pagination { get; set; }

        public class Paginated
        {
            public int Page { get; set; }
            public int Size { get; set; }
            public long Total { get; set; }
            public int TotalPage { get; set; }
        }
    }
}
