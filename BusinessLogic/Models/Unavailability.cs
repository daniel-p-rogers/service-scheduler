using DataAccess.Models;

namespace BusinessLogic.Models;

public record Unavailability(DateOnly Date, ServiceTime? Time);