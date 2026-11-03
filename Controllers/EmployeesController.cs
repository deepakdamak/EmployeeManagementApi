
using EmployeeManagementApi.Data;
using EmployeeManagementApi.DTOs;
using EmployeeManagementApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

namespace EmployeeManagementApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EmployeesController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly ILogger<EmployeesController> _logger;

    public EmployeesController(
        AppDbContext context,
        ILogger<EmployeesController> logger)
    {
        _context = context;
        _logger = logger;
    }

    // GET: api/employees?page=1&pageSize=10
    [HttpGet]
    public async Task<IActionResult> GetAll(
        int page = 1, int pageSize = 10)
    {
        if (page < 1 || pageSize < 1)
            return BadRequest("Page and pageSize must be positive.");

        pageSize = Math.Min(pageSize, 100);

        var employees = await _context.Employees
            .AsNoTracking()
            .OrderBy(e => e.EmployeeId)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return Ok(employees);
    }

    // GET: api/employees/1
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var employee = await _context.Employees
            .AsNoTracking()
            .FirstOrDefaultAsync(e => e.EmployeeId == id);

        if (employee == null)
            return NotFound(new { message = "Employee not found." });

        return Ok(employee);
    }

    // POST: api/employees
    [Authorize]
    [HttpPost]
    public async Task<IActionResult> Create(
        EmployeeRequestDto dto)
    {
        var emailExists = await _context.Employees
            .AnyAsync(e => e.Email == dto.Email);

        if (emailExists)
            return Conflict(new { message = "Email already exists." });

        var employee = new Employee
        {
            Name = dto.Name.Trim(),
            Email = dto.Email.Trim(),
            Department = dto.Department.Trim(),
            Salary = dto.Salary,
            CreatedAt = DateTime.UtcNow
        };

        _context.Employees.Add(employee);

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException ex)
        {
            _logger.LogWarning(ex, "Employee creation failed.");
            return Conflict(new
            {
                message = "Could not create employee. Check whether the email already exists."
            });
        }

        return CreatedAtAction(
            nameof(GetById),
            new { id = employee.EmployeeId },
            employee);
    }

    // PUT: api/employees/1
    [Authorize]
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(
        int id, EmployeeRequestDto dto)
    {
        var employee = await _context.Employees
            .FirstOrDefaultAsync(e => e.EmployeeId == id);

        if (employee == null)
            return NotFound(new { message = "Employee not found." });

        var emailExists = await _context.Employees
            .AnyAsync(e => e.Email == dto.Email
                        && e.EmployeeId != id);

        if (emailExists)
            return Conflict(new { message = "Email already exists." });

        employee.Name = dto.Name.Trim();
        employee.Email = dto.Email.Trim();
        employee.Department = dto.Department.Trim();
        employee.Salary = dto.Salary;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException ex)
        {
            _logger.LogWarning(ex, "Employee update failed for ID {Id}", id);
            return Conflict(new
            {
                message = "Could not update employee. Check email uniqueness."
            });
        }

        return Ok(employee);
    }

    // DELETE: api/employees/1
    [Authorize]
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var employee = await _context.Employees
            .FirstOrDefaultAsync(e => e.EmployeeId == id);

        if (employee == null)
            return NotFound(new { message = "Employee not found." });

        _context.Employees.Remove(employee);
        await _context.SaveChangesAsync();

        return NoContent();
    }


    [HttpGet("department-count")]
    public async Task<IActionResult> GetDepartmentCount()
    {
        var result = await _context.Employees
            .GroupBy(e => e.Department)
            .Select(g => new
            {
                Department = g.Key,
                EmployeeCount = g.Count()
            })
            .OrderBy(x => x.Department)
            .ToListAsync();

        return Ok(result);
    }

}
