using FluentAssertions;
using IssueBoard.Domain.Common;
using IssueBoard.Domain.Entities;
using IssueBoard.Domain.Enums;

namespace IssueBoard.UnitTests.Domain;

public sealed class IssueTests
{
    private static readonly Guid ProjectId = Guid.Parse("11111111-1111-1111-1111-111111111111");
    private static readonly Guid CreatorUserId = Guid.Parse("22222222-2222-2222-2222-222222222222");
    private static readonly Guid ActorUserId = Guid.Parse("33333333-3333-3333-3333-333333333333");
    private static readonly Guid AssigneeUserId = Guid.Parse("44444444-4444-4444-4444-444444444444");

    [Fact]
    public void Create_ShouldCreateIssueWithBacklogStatus()
    {
        // Arrange
        DateTime dueDateUtc = DateTime.UtcNow.AddDays(7);

        // Act
        Issue issue = Issue.Create(
            ProjectId,
            number: 1,
            title: "Implement login",
            description: "Create user login endpoint.",
            priority: IssuePriority.High,
            createdByUserId: CreatorUserId,
            dueDateUtc: dueDateUtc);

        // Assert
        issue.Id.Should().NotBe(Guid.Empty);
        issue.ProjectId.Should().Be(ProjectId);
        issue.Number.Should().Be(1);
        issue.Title.Should().Be("Implement login");
        issue.Description.Should().Be("Create user login endpoint.");
        issue.Status.Should().Be(IssueStatus.Backlog);
        issue.Priority.Should().Be(IssuePriority.High);
        issue.CreatedByUserId.Should().Be(CreatorUserId);
        issue.AssigneeUserId.Should().BeNull();
        issue.DueDateUtc.Should().Be(dueDateUtc);

        issue.Comments.Should().BeEmpty();
        issue.Labels.Should().BeEmpty();

        issue.Activities.Should().ContainSingle();
        issue.Activities.Single().Action.Should().Be("Issue created");
    }

    [Fact]
    public void Create_ShouldTrimTitleAndDescription()
    {
        // Act
        Issue issue = Issue.Create(
            ProjectId,
            number: 1,
            title: "  Implement issue board  ",
            description: "  Create kanban columns.  ",
            priority: IssuePriority.Medium,
            createdByUserId: CreatorUserId);

        // Assert
        issue.Title.Should().Be("Implement issue board");
        issue.Description.Should().Be("Create kanban columns.");
    }

    [Fact]
    public void Create_ShouldThrowDomainException_WhenTitleIsEmpty()
    {
        // Act
        Action act = () => Issue.Create(
            ProjectId,
            number: 1,
            title: " ",
            description: "Valid description",
            priority: IssuePriority.Medium,
            createdByUserId: CreatorUserId);

        // Assert
        act.Should()
            .Throw<DomainException>()
            .WithMessage("title cannot be empty.");
    }

    [Fact]
    public void Create_ShouldThrowDomainException_WhenIssueNumberIsNotPositive()
    {
        // Act
        Action act = () => Issue.Create(
            ProjectId,
            number: 0,
            title: "Invalid issue",
            description: null,
            priority: IssuePriority.Medium,
            createdByUserId: CreatorUserId);

        // Assert
        act.Should()
            .Throw<DomainException>()
            .WithMessage("Issue number must be greater than zero.");
    }

    [Fact]
    public void ChangeStatus_ShouldUpdateStatusAndAddActivity()
    {
        // Arrange
        Issue issue = CreateValidIssue();

        // Act
        issue.ChangeStatus(IssueStatus.InProgress, ActorUserId);

        // Assert
        issue.Status.Should().Be(IssueStatus.InProgress);
        issue.UpdatedAtUtc.Should().NotBeNull();

        issue.Activities.Should().HaveCount(2);

        IssueActivity activity = issue.Activities.Last();
        activity.ActorUserId.Should().Be(ActorUserId);
        activity.Action.Should().Be("Status changed");
        activity.OldValue.Should().Be(IssueStatus.Backlog.ToString());
        activity.NewValue.Should().Be(IssueStatus.InProgress.ToString());
    }

    [Fact]
    public void ChangeStatus_ShouldNotAddActivity_WhenStatusIsTheSame()
    {
        // Arrange
        Issue issue = CreateValidIssue();

        // Act
        issue.ChangeStatus(IssueStatus.Backlog, ActorUserId);

        // Assert
        issue.Status.Should().Be(IssueStatus.Backlog);
        issue.Activities.Should().ContainSingle();
    }

    [Fact]
    public void ChangePriority_ShouldUpdatePriorityAndAddActivity()
    {
        // Arrange
        Issue issue = CreateValidIssue();

        // Act
        issue.ChangePriority(IssuePriority.Critical, ActorUserId);

        // Assert
        issue.Priority.Should().Be(IssuePriority.Critical);

        IssueActivity activity = issue.Activities.Last();
        activity.ActorUserId.Should().Be(ActorUserId);
        activity.Action.Should().Be("Priority changed");
        activity.OldValue.Should().Be(IssuePriority.Medium.ToString());
        activity.NewValue.Should().Be(IssuePriority.Critical.ToString());
    }

    [Fact]
    public void AssignTo_ShouldSetAssigneeAndAddActivity()
    {
        // Arrange
        Issue issue = CreateValidIssue();

        // Act
        issue.AssignTo(AssigneeUserId, ActorUserId);

        // Assert
        issue.AssigneeUserId.Should().Be(AssigneeUserId);
        issue.UpdatedAtUtc.Should().NotBeNull();

        IssueActivity activity = issue.Activities.Last();
        activity.ActorUserId.Should().Be(ActorUserId);
        activity.Action.Should().Be("Assignee changed");
        activity.OldValue.Should().BeNull();
        activity.NewValue.Should().Be(AssigneeUserId.ToString());
    }

    [Fact]
    public void AssignTo_ShouldNotAddActivity_WhenAssigneeIsTheSame()
    {
        // Arrange
        Issue issue = CreateValidIssue();
        issue.AssignTo(AssigneeUserId, ActorUserId);

        int activityCountAfterFirstAssignment = issue.Activities.Count;

        // Act
        issue.AssignTo(AssigneeUserId, ActorUserId);

        // Assert
        issue.AssigneeUserId.Should().Be(AssigneeUserId);
        issue.Activities.Should().HaveCount(activityCountAfterFirstAssignment);
    }

    [Fact]
    public void Unassign_ShouldClearAssigneeAndAddActivity()
    {
        // Arrange
        Issue issue = CreateValidIssue();
        issue.AssignTo(AssigneeUserId, ActorUserId);

        // Act
        issue.Unassign(ActorUserId);

        // Assert
        issue.AssigneeUserId.Should().BeNull();

        IssueActivity activity = issue.Activities.Last();
        activity.ActorUserId.Should().Be(ActorUserId);
        activity.Action.Should().Be("Assignee changed");
        activity.OldValue.Should().Be(AssigneeUserId.ToString());
        activity.NewValue.Should().BeNull();
    }

    [Fact]
    public void AddComment_ShouldAddCommentAndActivity()
    {
        // Arrange
        Issue issue = CreateValidIssue();

        // Act
        IssueComment comment = issue.AddComment("This needs backend validation.", CreatorUserId);

        // Assert
        comment.Id.Should().NotBe(Guid.Empty);
        comment.IssueId.Should().Be(issue.Id);
        comment.AuthorUserId.Should().Be(CreatorUserId);
        comment.Content.Should().Be("This needs backend validation.");

        issue.Comments.Should().ContainSingle();
        issue.Comments.Single().Should().Be(comment);

        IssueActivity activity = issue.Activities.Last();
        activity.ActorUserId.Should().Be(CreatorUserId);
        activity.Action.Should().Be("Comment added");
        activity.OldValue.Should().BeNull();
        activity.NewValue.Should().Be(comment.Id.ToString());
    }

    [Fact]
    public void AddComment_ShouldThrowDomainException_WhenContentIsEmpty()
    {
        // Arrange
        Issue issue = CreateValidIssue();

        // Act
        Action act = () => issue.AddComment(" ", CreatorUserId);

        // Assert
        act.Should()
            .Throw<DomainException>()
            .WithMessage("content cannot be empty.");

        issue.Comments.Should().BeEmpty();
        issue.Activities.Should().ContainSingle();
    }

    private static Issue CreateValidIssue()
    {
        return Issue.Create(
            ProjectId,
            number: 1,
            title: "Implement issues",
            description: "Create core issue functionality.",
            priority: IssuePriority.Medium,
            createdByUserId: CreatorUserId);
    }
}
