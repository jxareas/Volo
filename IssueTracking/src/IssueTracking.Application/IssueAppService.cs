using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IssueTracking.Application.Contracts.Issues;
using IssueTracking.Domain.Issues;
using Microsoft.AspNetCore.Authorization;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Guids;
using Volo.Abp.Specifications;
using Volo.Abp.Users;

namespace IssueTracking.Application
{
  public class IssueAppService : ApplicationService, IIssueAppService
  {
    private readonly IIssueRepository _issueRepository;
    private readonly IGuidGenerator _guidGenerator;

    public IssueAppService(IIssueRepository issueRepository, IGuidGenerator guidGenerator)
    {
      _issueRepository = issueRepository;
      _guidGenerator = guidGenerator;
    }

    public async Task<IssueDto> CreateAsync(CreateIssueDto input)
    {
      var issue = new Issue(_guidGenerator.Create(), input.RepositoryId, input.Title, input.Text);
      await _issueRepository.InsertAsync(issue);
      return ObjectMapper.Map<Issue, IssueDto>(issue);
    }

    public async Task<IssueDto> GetAsync(Guid id)
    {
      var issue = await _issueRepository.GetAsync(id);
      return ObjectMapper.Map<Issue, IssueDto>(issue);
    }

    public async Task<PagedResultDto<IssueDto>> GetListAsync(GetIssueListDto input)
    {
      if (input.Sorting.IsNullOrWhiteSpace())
      {
        input.Sorting = nameof(Issue.Title);
      }

      var issues = new List<Issue>();
      if (input.ShowInActiveIssues.HasValue && input.ShowInActiveIssues == true && input.MileStoneId != Guid.Empty)
      {
        issues = await AsyncExecuter.ToListAsync(_issueRepository.Where(
           new InActiveIssueSpecification()
           .And(new MileStoneSpecification(input.MileStoneId)).ToExpression()));
      }
      else if (input.ShowInActiveIssues.HasValue && input.ShowInActiveIssues == false && input.MileStoneId != Guid.Empty)
      {
        issues = await AsyncExecuter.ToListAsync(_issueRepository.Where(
            new MileStoneSpecification(input.MileStoneId)));
      }
      else if (input.ShowInActiveIssues.HasValue && input.ShowInActiveIssues == true)
      {
        issues = await AsyncExecuter.ToListAsync(_issueRepository.Where(
          new InActiveIssueSpecification()));
      }
      else
      {
        issues = await _issueRepository.GetPagedListAsync(input.SkipCount, input.MaxResultCount, input.Sorting, includeDetails: true);
      }

      var totalCount = await AsyncExecuter.CountAsync(_issueRepository.WhereIf(!input.Filter.IsNullOrWhiteSpace(), issue => issue.Title.Contains(input.Filter)));

      List<IssueDto> items = ObjectMapper.Map<List<Issue>, List<IssueDto>>(issues);

      return new PagedResultDto<IssueDto>(totalCount, items);
    }

    public async Task DeleteAsync(Guid id)
    {
      await _issueRepository.DeleteAsync(id);
    }

    public async Task UpdateAsync(Guid id, UpdateIssueDto input)
    {
      var issue = await _issueRepository.GetAsync(id);

      issue.SetTitle(input.Text);
      issue.Text = input.Text;
      issue.AssignedUserId = input.AssignedUserId;

      await _issueRepository.UpdateAsync(issue);
    }

    [Authorize]
    public async Task CreateCommentAsync(CreateCommentDto input)
    {
      var issue = await _issueRepository.GetAsync(input.IssueId);
      issue.AddComment(CurrentUser.GetId(), input.Text);
      await _issueRepository.UpdateAsync(issue, autoSave: false);
    }

    public async Task CloseAsync(Guid id, CloseIssueDto input)
    {
      var issue = await _issueRepository.GetAsync(id);
      issue.Close(input.CloseReason);
    }

    public async Task ReOpenAsync(Guid id)
    {
      var issue = await _issueRepository.GetAsync(id);
      issue.ReOpen();
    }

    public async Task LockAsync(Guid id)
    {
      var issue = await _issueRepository.GetAsync(id);
      issue.Lock();

    }

    public async Task UnlockAsync(Guid id)
    {
      var issue = await _issueRepository.GetAsync(id);
      issue.Unlock();
    }

  }
}