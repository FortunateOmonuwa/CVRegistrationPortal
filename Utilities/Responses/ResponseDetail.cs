namespace Utilities.Responses
{
    public class ResponseDetail<T>
    {
        public bool IsSuccessful { get; set; }
        public string Message { get; set; } = string.Empty;
        public T Result { get; set; } = default;
        public int? ResultCode { get; set; }

        public ResponseDetail<T> FailedResultData(T res)
        {
            var r = new ResponseDetail<T>
            {
                Message = "Operation was not successful",
                IsSuccessful = false,
                Result = res,
                ResultCode = 400
            };

            return r;
        }

        public ResponseDetail<T> FailedResultData(T res, string message)
        {
            var r = new ResponseDetail<T>
            {
                Message = message,
                IsSuccessful = false,
                Result = res,
                ResultCode = 400
            };

            return r;
        }

        public ResponseDetail<T> FailedResultData(T res, string message, int code)
        {
            var r = new ResponseDetail<T>
            {
                Message = message,
                IsSuccessful = false,
                Result = res,
                ResultCode = code
            };

            return r;
        }
        public ResponseDetail<T> FailedResultData(string message, int code)
        {
            var r = new ResponseDetail<T>
            {
                Message = message,
                IsSuccessful = false,
                ResultCode = code


            };

            return r;
        }
        public ResponseDetail<T> FailedResultData(string message)
        {
            var r = new ResponseDetail<T>
            {
                Message = message,
                IsSuccessful = false,
                ResultCode = 400


            };

            return r;
        }

        public ResponseDetail<T> SuccessResultData(T result)
        {
            var r = new ResponseDetail<T>
            {
                Message = "Operation Successful",
                IsSuccessful = true,
                Result = result,
                ResultCode = 200
            };

            return r;
        }
        public ResponseDetail<T> SuccessResultData(T result, string message)
        {
            var r = new ResponseDetail<T>
            {
                Message = message,
                IsSuccessful = true,
                Result = result,
                ResultCode = 200
            };

            return r;
        }
        public ResponseDetail<T> SuccessResultData(T result, int code, string message)
        {
            var r = new ResponseDetail<T>
            {
                Message = message,
                IsSuccessful = true,
                Result = result,
                ResultCode = code
            };

            return r;
        }


        public ResponseDetail<T> SuccessResultData(string message)
        {
            var r = new ResponseDetail<T>
            {
                Message = message,
                IsSuccessful = true,
                ResultCode = 200
            };

            return r;
        }

    }
}
