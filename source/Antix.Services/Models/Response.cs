﻿using System.Collections.Generic;
using System.Linq;

namespace Antix.Services.Models
{
    public class Response
    {
        readonly IReadOnlyCollection<ResponseError> _errors;

        protected Response(
            IEnumerable<ResponseError> errors = null)
        {
            _errors = errors == null ? new ResponseError[] {} : errors.ToArray();
        }

        public IReadOnlyCollection<ResponseError> Errors
        {
            get { return _errors; }
        }

        public static Response<T> Create<T> (T data = default(T), IEnumerable<ResponseError> errors = null)
        {
            return new Response<T>(data, errors);
        }
    }

    public class Response<T> : Response
    {
        readonly T _data;

        internal Response(
            T data,
            IEnumerable<ResponseError> errors = null) :
                base(errors)
        {
            _data = data;
        }

        public T Data
        {
            get { return _data; }
        }
    }
}