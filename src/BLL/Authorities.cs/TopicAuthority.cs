using BOL.Models;

namespace BLL.Authorities
{
    public class TopicAuthority : BaseAuthority
    {
        // Topic against which authorities should be checked
        protected readonly Topic topic;

        public TopicAuthority(User currentUser, Topic currentTopic) : base(currentUser)
        {
            topic = currentTopic;
        }

        public bool IsUserAllowed(string action)
        {
            /** Role: Supervisor
                ----------------
                # 1. Can create new topic
                # 2. Can read any topic
                # 3. Can update only topics he/she created
                # 4. Can delete only topics he/she created
            **/
            if(user.IsSupervisor())
            {
                // # 1
                if(action == Operation.Create)
                    return true;
        
                // # 2
                else if(action == Operation.ReadList || action == Operation.ReadDetail)
                    return true;

                // # 3
                else if(action == Operation.Update && topic.CreatedById == user.Id)
                    return true;

                // # 4
                else if(action == Operation.Delete && topic.CreatedById == user.Id)
                    return true;
            }


            /** Role: Student
                -------------
                # 1. Can read only topics he/she is assigned to 
            **/
            if(user.IsStudent())
            {
                // # 1
                if(action == Operation.ReadList)
                    return true;
                else if(action == Operation.ReadDetail) // **TODO: Add this check on the right once EF core adds lazy loading** && topic.TopicUsers.Find(tu => tu.UserId == user.Id) != null)
                    return true;
            }

            return false;
        }
        
    }
}