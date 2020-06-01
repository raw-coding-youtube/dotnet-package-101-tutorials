using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public static class Response
    {
        public static Response<T> Fail<T>(string message, T data = default) =>
            new Response<T>(data, message, true);
        
        public static Response<T> Ok<T>(T data, string message) =>
            new Response<T>(data, message, false);
    }

    public class Response<T>
    {
        public Response(T data, string msg, bool error)
        {
            Data = data;
            Message = msg;
            Error = error;
        }

        public T Data { get; set; }
        public string Message { get; set; }
        public bool Error { get; set; }
    }
}
