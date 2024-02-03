namespace Pretech.Software.RConfig.Crypto
{
    public class ExecuteResponse<T>
    {
        #region Properties
        public T Data { get; private set; }
        public bool IsSuccess { get; private set; }
        public string Message { get; private set; }
        #endregion

        #region Constructor
        public ExecuteResponse()
        {

        }
        public ExecuteResponse(T data)
        {
            //this.Data = data ?? throw new Exception("Generic data type is not null !");
            this.Data = data;
        }
        #endregion

        #region Action methods
        public ExecuteResponse<T> Success(T data)
        {
            this.Data = data;
            this.IsSuccess = true;
            this.Message = null;
            return this;
        }
        public ExecuteResponse<T> Success()
        {
            this.IsSuccess = true;
            this.Message = null;
            return this;
        }
        public ExecuteResponse<T> Success(T data, string message)
        {
            this.IsSuccess = true;
            this.Data = data;
            this.Message = message;
            return this;
        }

        public ExecuteResponse<T> Maintance()
        {
            this.IsSuccess = false;
            this.Data = default(T);
            this.Message = null;
            return this;
        }

        public ExecuteResponse<T> Error(string message)
        {
            this.Data = default(T);
            this.IsSuccess = false;
            this.Message = message;
            return this;
        }
        public ExecuteResponse<T> Error(Exception ex)
        {
            this.Data = default(T);
            this.Message = $"Exception;{ex.Message}- Stack : {ex.StackTrace}";
            this.IsSuccess = false;
            return this;
        }
        #endregion
    }
}
