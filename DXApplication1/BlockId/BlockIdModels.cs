using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace DXApplication1.BlockId
{
    /// <summary>
    /// Envelope chuẩn CloudEvents 1.0 cho BlockId Ingestion Protocol
    /// </summary>
    public class BlockIdCloudEvent<T>
    {
        [JsonProperty("specversion")]
        public string SpecVersion { get; set; } = "1.0";

        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("source")]
        public string Source { get; set; }

        [JsonProperty("subject")]
        public string Subject { get; set; }

        [JsonProperty("time", NullValueHandling = NullValueHandling.Ignore)]
        public string Time { get; set; }

        [JsonProperty("data")]
        public T Data { get; set; }
    }

    /// <summary>
    /// Dữ liệu gửi từng barcode (Cách 1: vn.blockid.production.record.v1)
    /// </summary>
    public class BlockIdRecordData
    {
        [JsonProperty("barcode")]
        public string Barcode { get; set; }

        [JsonProperty("lot", NullValueHandling = NullValueHandling.Ignore)]
        public string Lot { get; set; }

        [JsonProperty("values")]
        public List<BlockIdParamValue> Values { get; set; } = new List<BlockIdParamValue>();
    }

    /// <summary>
    /// Dữ liệu gửi theo khay / xe trolley (Cách 2: vn.blockid.production.tray.v1)
    /// </summary>
    public class BlockIdTrayData
    {
        [JsonProperty("trayId", NullValueHandling = NullValueHandling.Ignore)]
        public string TrayId { get; set; }

        [JsonProperty("lot", NullValueHandling = NullValueHandling.Ignore)]
        public string Lot { get; set; }

        [JsonProperty("barcodes")]
        public List<string> Barcodes { get; set; } = new List<string>();

        [JsonProperty("values")]
        public List<BlockIdParamValue> Values { get; set; } = new List<BlockIdParamValue>();
    }

    /// <summary>
    /// Từng thông số kỹ thuật (code do BlockId cấp, value định dạng chuẩn)
    /// </summary>
    public class BlockIdParamValue
    {
        [JsonProperty("code")]
        public string Code { get; set; }

        [JsonProperty("value")]
        public object Value { get; set; }

        public BlockIdParamValue() { }

        public BlockIdParamValue(string code, object value)
        {
            this.Code = code;
            this.Value = value;
        }
    }

    /// <summary>
    /// Kết quả phản hồi từ BlockId API
    /// </summary>
    public class BlockIdResponse
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("total")]
        public int? Total { get; set; }

        [JsonProperty("accepted")]
        public int? Accepted { get; set; }

        [JsonProperty("duplicates")]
        public List<BlockIdDuplicateItem> Duplicates { get; set; }

        [JsonProperty("message")]
        public string Message { get; set; }
    }

    public class BlockIdDuplicateItem
    {
        [JsonProperty("barcode")]
        public string Barcode { get; set; }

        [JsonProperty("reason")]
        public string Reason { get; set; }
    }
}
