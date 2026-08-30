using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs;

public sealed record TaskHistoryResponse(
    Guid Id,
    Guid TaskId,
    string? PreviousStatus,
    string NewStatus,
    Guid UserId,
    DateTime ChangedAt);
