// Application/Mappings/ChequeBookMapper.cs
using MotorStores.Application.DTOs;
using MotorStores.Domain.Entities;

namespace MotorStores.Application.Mappings;

public static class ChequeBookMapper
{
    public static ChequeBookDto ToDto(ChequeBook chequeBook) => new ChequeBookDto
    {
        Id = chequeBook.Id,
        BankAccountId = chequeBook.BankAccountId,
        AccountNo = chequeBook.BankAccount?.AccountNo ?? "Unknown",
        ChequeBookNo = chequeBook.ChequeBookNo,
        StartChequeNo = chequeBook.StartChequeNo,
        EndChequeNo = chequeBook.EndChequeNo,
        CurrentChequeNo = chequeBook.CurrentChequeNo,
        Status = chequeBook.Status.ToString(),
        IssuedDate = chequeBook.IssuedDate
    };
}