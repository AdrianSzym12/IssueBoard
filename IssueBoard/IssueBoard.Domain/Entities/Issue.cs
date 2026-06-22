using IssueBoard.Domain.Common;
using IssueBoard.Domain.Enums;

namespace IssueBoard.Domain.Entities;

public sealed class Issue : Entity
{
    private const int TitleMaxLength = 200;
    private const int DescriptionMaxLength = 4000;

    private readonly List<IssueComment> _comments = new();
    private readonly List<IssueActivity> _activities = new();
    private readonly List<IssueLabel> _labels = new();

    private Issue()
    {
    }

    private Issue(
        Guid id,
        Guid projectId,
        int number,
        string title,
        string? description,
        IssuePriority priority,
        Guid createdByUserId,
        DateTime? dueDateUtc)
        : base(id)
    {
        if (number <= 0)
        {
            throw new DomainException("Issue number must be greater than zero.");
        }

        ProjectId = Guard.AgainstEmpty(projectId, nameof(projectId));
        Number = number;
        Title = Guard.AgainstNullOrWhiteSpace(title, nameof(title), TitleMaxLength);
        Description = Guard.OptionalText(description, nameof(description), DescriptionMaxLength);
        Status = IssueStatus.Backlog;
        Priority = Guard.AgainstInvalidEnum(priority, nameof(priority));
        CreatedByUserId = Guard.AgainstEmpty(createdByUserId, nameof(createdByUserId));
        DueDateUtc = dueDateUtc;

        AddActivity(createdByUserId, "Issue created", null, Status.ToString());
    }

    public Guid ProjectId { get; private set; }

    public int Number { get; private set; }

    public string Title { get; private set; } = string.Empty;

    public string? Description { get; private set; }

    public IssueStatus Status { get; private set; }

    public IssuePriority Priority { get; private set; }

    public Guid CreatedByUserId { get; private set; }

    public Guid? AssigneeUserId { get; private set; }

    public DateTime? DueDateUtc { get; private set; }

    public IReadOnlyCollection<IssueComment> Comments => _comments.AsReadOnly();

    public IReadOnlyCollection<IssueActivity> Activities => _activities.AsReadOnly();

    public IReadOnlyCollection<IssueLabel> Labels => _labels.AsReadOnly();

    public static Issue Create(
        Guid projectId,
        int number,
        string title,
        string? description,
        IssuePriority priority,
        Guid createdByUserId,
        DateTime? dueDateUtc = null)
    {
        return new Issue(
            Guid.NewGuid(),
            projectId,
            number,
            title,
            description,
            priority,
            createdByUserId,
            dueDateUtc);
    }

    public void UpdateDetails(string title, string? description, Guid actorUserId)
    {
        Guard.AgainstEmpty(actorUserId, nameof(actorUserId));

        string newTitle = Guard.AgainstNullOrWhiteSpace(title, nameof(title), TitleMaxLength);
        string? newDescription = Guard.OptionalText(description, nameof(description), DescriptionMaxLength);

        bool changed = false;

        if (Title != newTitle)
        {
            AddActivity(actorUserId, "Title changed", Title, newTitle);
            Title = newTitle;
            changed = true;
        }

        if (Description != newDescription)
        {
            AddActivity(actorUserId, "Description changed", Description, newDescription);
            Description = newDescription;
            changed = true;
        }

        if (changed)
        {
            MarkUpdated();
        }
    }

    public void ChangeStatus(IssueStatus status, Guid actorUserId)
    {
        IssueStatus newStatus = Guard.AgainstInvalidEnum(status, nameof(status));
        Guard.AgainstEmpty(actorUserId, nameof(actorUserId));

        if (Status == newStatus)
        {
            return;
        }

        IssueStatus oldStatus = Status;
        Status = newStatus;

        AddActivity(actorUserId, "Status changed", oldStatus.ToString(), newStatus.ToString());
        MarkUpdated();
    }

    public void ChangePriority(IssuePriority priority, Guid actorUserId)
    {
        IssuePriority newPriority = Guard.AgainstInvalidEnum(priority, nameof(priority));
        Guard.AgainstEmpty(actorUserId, nameof(actorUserId));

        if (Priority == newPriority)
        {
            return;
        }

        IssuePriority oldPriority = Priority;
        Priority = newPriority;

        AddActivity(actorUserId, "Priority changed", oldPriority.ToString(), newPriority.ToString());
        MarkUpdated();
    }

    public void AssignTo(Guid assigneeUserId, Guid actorUserId)
    {
        Guard.AgainstEmpty(assigneeUserId, nameof(assigneeUserId));
        Guard.AgainstEmpty(actorUserId, nameof(actorUserId));

        if (AssigneeUserId == assigneeUserId)
        {
            return;
        }

        string? oldAssignee = AssigneeUserId?.ToString();
        AssigneeUserId = assigneeUserId;

        AddActivity(actorUserId, "Assignee changed", oldAssignee, assigneeUserId.ToString());
        MarkUpdated();
    }

    public void Unassign(Guid actorUserId)
    {
        Guard.AgainstEmpty(actorUserId, nameof(actorUserId));

        if (AssigneeUserId is null)
        {
            return;
        }

        string oldAssignee = AssigneeUserId.Value.ToString();
        AssigneeUserId = null;

        AddActivity(actorUserId, "Assignee changed", oldAssignee, null);
        MarkUpdated();
    }

    public void ChangeDueDate(DateTime? dueDateUtc, Guid actorUserId)
    {
        Guard.AgainstEmpty(actorUserId, nameof(actorUserId));

        if (DueDateUtc == dueDateUtc)
        {
            return;
        }

        string? oldDueDate = DueDateUtc?.ToString("O");
        string? newDueDate = dueDateUtc?.ToString("O");

        DueDateUtc = dueDateUtc;

        AddActivity(actorUserId, "Due date changed", oldDueDate, newDueDate);
        MarkUpdated();
    }

    public IssueComment AddComment(string content, Guid authorUserId)
    {
        IssueComment comment = IssueComment.Create(Id, authorUserId, content);

        _comments.Add(comment);
        AddActivity(authorUserId, "Comment added", null, comment.Id.ToString());
        MarkUpdated();

        return comment;
    }

    public void AddLabel(IssueLabel label, Guid actorUserId)
    {
        if (label is null)
        {
            throw new DomainException("Label cannot be null.");
        }

        Guard.AgainstEmpty(actorUserId, nameof(actorUserId));

        if (label.ProjectId != ProjectId)
        {
            throw new DomainException("Label must belong to the same project as the issue.");
        }

        if (_labels.Any(existingLabel => existingLabel.Id == label.Id))
        {
            return;
        }

        _labels.Add(label);
        AddActivity(actorUserId, "Label added", null, label.Name);
        MarkUpdated();
    }

    public void RemoveLabel(Guid labelId, Guid actorUserId)
    {
        Guard.AgainstEmpty(labelId, nameof(labelId));
        Guard.AgainstEmpty(actorUserId, nameof(actorUserId));

        IssueLabel? label = _labels.SingleOrDefault(existingLabel => existingLabel.Id == labelId);

        if (label is null)
        {
            return;
        }

        _labels.Remove(label);
        AddActivity(actorUserId, "Label removed", label.Name, null);
        MarkUpdated();
    }

    private void AddActivity(Guid actorUserId, string action, string? oldValue, string? newValue)
    {
        IssueActivity activity = IssueActivity.Create(
            Id,
            actorUserId,
            action,
            oldValue,
            newValue);

        _activities.Add(activity);
    }
}
