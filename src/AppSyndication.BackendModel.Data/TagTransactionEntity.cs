using System;
using Microsoft.WindowsAzure.Storage.Table;

namespace AppSyndication.BackendModel.Data
{
    public enum TagTransactionOperation
    {
        Unknown,
        Error,
        Create,
        Update,
        Delete,
    }

    public class TagTransactionEntity : TableEntity
    {
        private static int Uniquifier = 0;

        public TagTransactionEntity() { }

        public TagTransactionEntity(TagTransactionOperation operation, string channel, string alias, string username)
        {
            if (String.IsNullOrEmpty(alias)) throw new ArgumentNullException(nameof(alias));
            if (String.IsNullOrEmpty(username)) throw new ArgumentNullException(nameof(username));

            channel = String.IsNullOrEmpty(channel) ? "@" : channel;

            var now = DateTime.UtcNow.ToString("yyyy-MMdd-HHmm-ss");

            var uniquifier = Uniquifier++;

            var transactionId = $"{username}|{now}-{uniquifier % 10000}";

            this.PartitionKey = TagTransactionEntity.CalculatePartitionKey(channel, transactionId);
            this.RowKey = TagTransactionEntity.CalculateRowKey();

            this.Channel = channel;
            this.Alias = alias;
            this.Username = username;
            this.Id = transactionId;

            this.Operation = operation.ToString();
            this.StagedBlobUri = $"{channel}/{alias}/{transactionId}";
        }

        public string Channel { get; set; }

        public string Alias { get; set; }

        public string Username { get; set; }

        public string Id { get; set; }

        public string Operation { get; set; }

        public string OriginalFilename { get; set; }

        public string StagedBlobUri { get; set; }

        public DateTime? Stored { get; set; }

        public DateTime? Updated { get; set; }

        public TagTransactionOperation OperationValue
        {
            get
            {
                TagTransactionOperation operation;
                return Enum.TryParse<TagTransactionOperation>(this.Operation, out operation) ? operation : TagTransactionOperation.Unknown;
            }
        }

        public bool TryUpdateOperation(TagTransactionOperation operation)
        {
            if (this.OperationValue != operation)
            {
                this.Operation = operation.ToString();
                return true;
            }

            return false;
        }

        internal static string CalculatePartitionKey(string channel, string transactionId)
        {
            return channel + "|" + transactionId;
        }

        internal static string CalculateRowKey()
        {
            return String.Empty;
        }

        /// <summary>
        /// Creates a tag transaction entity that can be used to update the entity in table storage
        /// as an error, without updating any other fields of the table entity. This is useful when
        /// there were changes made to the entity before an error was found and you do not want to
        /// push all the changes (just mark the transaction entity as an error).
        /// </summary>
        /// <returns></returns>
        public TagTransactionEntity AsError()
        {
            return new TagTransactionEntity
            {
                PartitionKey = this.PartitionKey,
                RowKey = this.RowKey,
                ETag = "*",
                Operation = TagTransactionOperation.Error.ToString(),
            };
        }
    }
}
