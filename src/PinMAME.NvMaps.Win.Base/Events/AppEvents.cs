using Prism.Events;

namespace WPFPrismTemplate.Base.Events
{
    /// <summary>
    /// Tell the Main shell to run high score search
    /// </summary>
    public class FindHighScoresEvent : PubSubEvent { }

    /// <summary>
    /// Tell the Main shell to run latest score search
    /// </summary>
    public class FindLatestScoresEvent : PubSubEvent { }

    public class PreviewOutputEvent : PubSubEvent<string> { }

    /// <summary>
    /// Tell the Main shell to run mode champions search
    /// </summary>
    public class FindModeChampsEvent : PubSubEvent { }
    
}
