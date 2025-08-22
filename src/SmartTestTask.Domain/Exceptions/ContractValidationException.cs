namespace SmartTestTask.Domain.Exceptions;

public class ContractValidationException : DomainException
{
    public ContractValidationException(string message) : base(message) { }
}