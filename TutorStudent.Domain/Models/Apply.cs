using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using JetBrains.Annotations;
using TutorStudent.Domain.Enums;
using TutorStudent.Domain.Implementations;
using Stateless;
using TutorStudent.Domain.ProxyServices;
using TutorStudent.Domain.ProxyServices.Dto;

namespace TutorStudent.Domain.Models
{
    public class Apply: TrackableEntity
    {
        [Required] public Guid TutorId { get; set; }
        [Required] public Student Student { get; set; }
        [Required] public Guid StudentId { get; set; }
        [Required] public string Description { get; set; }
        [Required] public TicketType Ticket { get; set; }
        [Required] public StateType State { get; set; } = StateType.Unseen;
        [CanBeNull] public string Comment { get; set; }
        [CanBeNull] public string TrackingCode { get; set; }
        
        
        
        private ITrackingCode _trackingCodeService;
        
        private const string MyProjectName = "Tutor Student";
        private const string MyPreName = "TS";
        
        public void GetTrackingCode(ITrackingCode trackingCodeService)
        {
            _trackingCodeService = trackingCodeService;
            
            var myTrackingCodeDto = new TrackingCodeProxyData
            {
                PreName = MyPreName,
                ProjectName = MyProjectName
            };
            
            TrackingCode = _trackingCodeService.GetTrackingCode(myTrackingCodeDto).Result;
        }
        
        
        private readonly StateMachine<StateType, TriggerType> _stateMachine;
        public Apply()
        {
            _stateMachine = new StateMachine<StateType, TriggerType>(() => State, state => State = state);
           
            _stateMachine.Configure(StateType.Unseen)
                .Permit(TriggerType.Open, StateType.Seen); 
            _stateMachine.Configure(StateType.Seen)
                .Permit(TriggerType.Reject, StateType.Rejected)
                .Permit(TriggerType.Confirm, StateType.Confirmed);

        }
        
        public IEnumerable<TriggerType> GetActions()
        {
            return _stateMachine.GetPermittedTriggers(State);
        }
        
        
        public async Task FireTrigger(TriggerType trigger , string comment)
        {
            
            try
            {
                await _stateMachine.FireAsync(trigger);

                Comment = comment;
            }
            catch (InvalidOperationException)
            {
                throw new Exception("عملیات مورد نظر امکان پذیر نیست");
            }
            
        }
        
        
    }
}