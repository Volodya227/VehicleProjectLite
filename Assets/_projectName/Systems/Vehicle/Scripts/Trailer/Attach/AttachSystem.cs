using System.Collections.Generic;
using UnityEngine;
namespace Systems.Vehicle.Trailer.Attach.Core
{
    public class AttachSystem
    {
        private readonly Rigidbody _body;
        private readonly List<Attachment> Attachments;
        public AttachSystem(Rigidbody body)
        {
            _body = body;
            Attachments = new List<Attachment>();
        }
        public void AddAttach(Attachment item) {
            item.SetData(new Data.DataAttach(_body, item.transform.localPosition));
            Attachments.Add(item);
        }
        public void SetInput(Input.TrailerInput input)
        {
            foreach (Attachment attachment in Attachments)
            {
                attachment.SetInput(input);
            }
        }
    }
}