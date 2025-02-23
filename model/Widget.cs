using MongoDB.Bson.Serialization.Attributes;

namespace Trasgo.Shared.Models
{
    public class Widget
    {
        public List<AddLink>? Content {get; set;}
        public object user {get; set;}
    }

    public class Content
    {
        [BsonElement("Text")]
        public string? Text {get; set;}

        [BsonElement("Title")]
        public string? Title {get; set;}

        [BsonElement("Caption")]
        public string? Caption {get; set;}
        
        [BsonElement("Url")]
        public string? Url {get; set;}

        [BsonElement("Contents")]
        public string? Contents {get; set;}

        [BsonElement("Latitude")]
        public float? Latitude {get; set;}

        [BsonElement("Longitude")]
        public float? Longitude {get; set;}

        [BsonElement("Email")]
        public string? Email {get; set;}

        [BsonElement("urlThumbnail")]
        public string? urlThumbnail {get; set;}

        [BsonElement("FullName")]
        public string? FullName {get; set;}

        [BsonElement("Description")]
        public string? Description {get; set;}

        [BsonElement("UrlBanner")]
        public string? UrlBanner {get; set;}

        [BsonElement("UrlImageProfile")]
        public string? UrlImageProfile {get; set;}

        [BsonElement("PhoneNumber")]
        public string? PhoneNumber {get; set;}
        [BsonElement("widgetCarouselAttachment")]
        public List<CarouselItem>? WidgetCarouselAttachment {get; set;}
        public List<ScheduleItem>? WidgetSchedule {get; set;}


        [BsonElement("UrlWebinar")]
        public string? UrlWebinar {get; set;}

        [BsonElement("Notes")]
        public string? Notes {get; set;}

        [BsonElement("Passcode")]
        public string? Passcode {get; set;}

        [BsonElement("StartDate")]
        public DateTime? StartDate {get; set;}

        [BsonElement("EndDate")]
        public DateTime? EndDate {get; set;}

        [BsonElement("Date")]
        public DateTime? Date {get; set;}

        [BsonElement("StartTime")]
        public string? StartTime {get; set;}

        [BsonElement("EndTime")]
        public string? EndTime {get; set;}

    }

    public class ScheduleItem
    {
        [BsonElement("id")]
        public string? id {get; set;}
        [BsonElement("Date")]

        public DateTime? Date { get; set; }
        [BsonElement("StartTime")]

        public string? StartTime { get; set; }
        [BsonElement("EndTime")]

        public string? EndTime { get; set; }

    }

    public class CarouselItem
    {
        [BsonElement("id")]
        public string? id {get; set;}
        [BsonElement("widgetCarouselId")]
        public string? widgetCarouselId {get; set;}
        [BsonElement("attachmentId")]
        public string? attachmentId {get; set;}
    }
    public class AddLink : BaseModelUser 
    {
        [BsonId]
        public string? Id {get; set;}
        [BsonElement("Content")]
        public Content? Content {get; set;}
    }

    public class AddSosmed 
    {
        [BsonId]
        public string? Key {get; set;}

        [BsonElement("Value")]
        public string? Value {get; set;}

        [BsonElement("UserId")]
        public string? UserId {get; set;}
    }

    public class AddText : BaseModelUser
    {
        [BsonId]
        public string? Id {get; set;}
        [BsonElement("Content")]
        public Content? Content {get; set;}
    }

    public class AddImage : BaseModelUser
    {
        [BsonId]
        public string? Id {get; set;}

        [BsonElement("Caption")]
        public string? Caption {get; set;}
        
        [BsonElement("UrlImage")]
        public string? UrlImage {get; set;}
    }

    public class AddProfile : BaseModelUser
    {
        [BsonId]
        public string? Id {get; set;}

        [BsonElement("Caption")]
        public string? Caption {get; set;}
        
        [BsonElement("UrlImage")]
        public string? UrlImage {get; set;}
    }

    public class AddVideo: BaseModelUser
    {
        [BsonId]
        public string? Id {get; set;}

        [BsonElement("Caption")]
        public string? Caption {get; set;}
        
        [BsonElement("UrlVideo")]
        public string? UrlVideo {get; set;}
    }

    public class AddContent : BaseModelUser
    {
        [BsonId]
        public string? Id {get; set;}
        public Content? Content {get; set;}
    }

    public class AddContact : BaseModelUser
    {
        [BsonId]
        public string? Id {get; set;}

        [BsonElement("Email")]
        public string? Email {get; set;}
        
        [BsonElement("PhoneNumber")]
        public string? PhoneNumber {get; set;}
    }

    public class AddCarausel : BaseModelUser
    {
        [BsonId]
        public string? Id {get; set;}

        [BsonElement("Caption")]
        public string? Caption {get; set;}
        
        [BsonElement("Image")]
        public string? Image {get; set;}
    }

    public class AddWebinar : BaseModelUser
    {
        [BsonId]
        public string? Id {get; set;}

        [BsonElement("Title")]
        public string? Title {get; set;}

        [BsonElement("UrlLink")]
        public string? UrlLink {get; set;}

        [BsonElement("Passcode")]
        public string? Passcode {get; set;}
        
        [BsonElement("StartDate")]
        public DateTime? StartDate {get; set;}

        [BsonElement("EndDate")]
        public DateTime? EndDate {get; set;}

        [BsonElement("Desc")]
        public string? Desc {get; set;}
    }

    public class AddBanner : BaseModelUser
    {
        [BsonId]
        public string? Id {get; set;}

        [BsonElement("UrlImage")]
        public string? UrlImage {get; set;}
    }

    public class AddSocial : BaseModelUser
    {
        [BsonId]
        public string? Id {get; set;}

        [BsonElement("UrlLink")]
        public string? UrlLink {get; set;}

        [BsonElement("Model")]
        public int? Model {get; set;}
    }

    public class Attachments : BaseModelUser
    {
        [BsonId]
        public string? Id {get; set;}

        [BsonElement("fileName")]
        public string? fileName {get; set;}

        [BsonElement("type")]
        public string? type {get; set;}

        [BsonElement("path")]
        public string? path {get; set;}
        [BsonElement("size")]
        public long? size {get; set;}
    }
}