﻿using AutoMapper;

using Convience.Entity.Data;
using Convience.Entity.Entity.WorkFlows;
using Convience.EntityFrameWork.Repositories;
using Convience.Model.Models.WorkFlowManage;

using DnsClient.Internal;

using Microsoft.Extensions.Logging;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Convience.Service.WorkFlowManage
{
    public interface IWorkFlowFlowService
    {
        WorkFlowFlowResult GetWorkFlowFlow(int workflowId);

        Task<bool> AddOrUpdateWorkFlowFlow(WorkFlowFlowViewModel viewModel);
    }

    public class WorkFlowFlowService : IWorkFlowFlowService
    {
        private readonly ILogger<WorkFlowFlowService> _logger;

        private readonly IRepository<WorkFlowLink> _linkRepository;

        private readonly IRepository<WorkFlowNode> _nodeRepository;

        private readonly IUnitOfWork<SystemIdentityDbContext> _unitOfWork;

        private IMapper _mapper;

        public WorkFlowFlowService(ILogger<WorkFlowFlowService> logger,
            IRepository<WorkFlowLink> linkRepository,
            IRepository<WorkFlowNode> nodeRepository,
            IUnitOfWork<SystemIdentityDbContext> unitOfWork,
            IMapper mapper)
        {
            _logger = logger;
            _linkRepository = linkRepository;
            _nodeRepository = nodeRepository;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<bool> AddOrUpdateWorkFlowFlow(WorkFlowFlowViewModel viewModel)
        {
            try
            {
                foreach (var link in viewModel.WorkFlowLinkViewModels)
                {
                    link.WorkFlowId = viewModel.WorkFlowId;
                }
                foreach (var node in viewModel.WorkFlowNodeViewModels)
                {
                    node.WorkFlowId = viewModel.WorkFlowId;
                }

                await _linkRepository.RemoveAsync(f => f.WorkFlowId == viewModel.WorkFlowId);
                await _linkRepository.AddAsync(_mapper.Map<IEnumerable<WorkFlowLink>>(viewModel.WorkFlowLinkViewModels));
                await _nodeRepository.RemoveAsync(f => f.WorkFlowId == viewModel.WorkFlowId);
                await _nodeRepository.AddAsync(_mapper.Map<IEnumerable<WorkFlowNode>>(viewModel.WorkFlowNodeViewModels));
                await _unitOfWork.SaveAsync();
                return true;
            }
            catch (Exception e)
            {
                _logger.LogError(e.StackTrace);
                return false;
            }
        }

        public WorkFlowFlowResult GetWorkFlowFlow(int workflowId)
        {
            var links = _linkRepository.Get(f => f.WorkFlowId == workflowId).ToArray();
            var nodes = _nodeRepository.Get(f => f.WorkFlowId == workflowId).ToArray();
            return new WorkFlowFlowResult
            {
                WorkFlowLinkResults = _mapper.Map<IEnumerable<WorkFlowLinkResult>>(links),
                WorkFlowNodeResults = _mapper.Map<IEnumerable<WorkFlowNodeResult>>(nodes)
            };
        }
    }
}