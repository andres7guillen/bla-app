using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Commands.Tasks.CreateTask;

public sealed record CreateTaskCommand(
    Guid UserId,
    string Title,
    string Description,
    DateTime DueDate);
