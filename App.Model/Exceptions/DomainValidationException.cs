using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace App.Model.Exceptions
{
    public class DomainValidationException : Exception
    {
        public DomainValidationException(IEnumerable<ValidationResult> validators)
        {
            Validators = validators;
        }

        public IEnumerable<ValidationResult> Validators { get; private set; }
    }
}
