using System.Text.Json;
using BudgetCalculator.Controllers;
using BudgetCalculator.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BudgetCalculator.Tests;

public class BudgetControllerTests
{
    [Fact]
    public void Calculate_WithoutInsurance_ReturnsOkAnd500()
    {
        var controller = new BudgetController();
        var request = new BudgetRequest
        {
            DailyCost = 100m,
            Days = 5,
            IncludeInsurance = false
        };

        var result = controller.Calculate(request);

        var response = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(StatusCodes.Status200OK, response.StatusCode);
        var body = JsonSerializer.SerializeToElement(response.Value);
        Assert.Equal(500m, body.GetProperty("TotalAmount").GetDecimal());
    }

    [Fact]
    public void Calculate_WithInsurance_ReturnsOkAnd525()
    {
        var controller = new BudgetController();
        var request = new BudgetRequest
        {
            DailyCost = 100m,
            Days = 5,
            IncludeInsurance = true
        };

        var result = controller.Calculate(request);

        var response = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(StatusCodes.Status200OK, response.StatusCode);
        var body = JsonSerializer.SerializeToElement(response.Value);
        Assert.Equal(525m, body.GetProperty("TotalAmount").GetDecimal());
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(0)]
    public void Calculate_NonPositiveDays_ReturnsBadRequest(int days)
    {
        var controller = new BudgetController();
        var request = new BudgetRequest
        {
            DailyCost = 100m,
            Days = days,
            IncludeInsurance = false
        };

        var result = controller.Calculate(request);

        var response = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal(StatusCodes.Status400BadRequest, response.StatusCode);
        Assert.Equal("Количество дней должно быть больше нуля.", response.Value);
    }

    [Fact]
    public void Calculate_ZeroDailyCost_ReturnsOkAndZero()
    {
        var controller = new BudgetController();
        var request = new BudgetRequest
        {
            DailyCost = 0m,
            Days = 5,
            IncludeInsurance = false
        };

        var result = controller.Calculate(request);

        var response = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(StatusCodes.Status200OK, response.StatusCode);
        var body = JsonSerializer.SerializeToElement(response.Value);
        Assert.Equal(0m, body.GetProperty("TotalAmount").GetDecimal());
    }

    [Fact]
    public void Calculate_FractionalDailyCost_ReturnsOkAnd301Point50()
    {
        var controller = new BudgetController();
        var request = new BudgetRequest
        {
            DailyCost = 100.50m,
            Days = 3,
            IncludeInsurance = false
        };

        var result = controller.Calculate(request);

        var response = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(StatusCodes.Status200OK, response.StatusCode);
        var body = JsonSerializer.SerializeToElement(response.Value);
        Assert.Equal(301.50m, body.GetProperty("TotalAmount").GetDecimal());
    }
}
