using System;
using System.IO;
using System.Net;
using System.Text;
using DXApplication1.Common;
using Newtonsoft.Json;

namespace DXApplication1.BlockId
{
    public class BlockIdApiResult
    {
        public bool IsSuccess { get; set; }
        public bool IsDuplicate { get; set; }
        public bool IsTemporaryError { get; set; }
        public bool IsFatalError { get; set; }
        public int StatusCode { get; set; }
        public string ErrorMessage { get; set; }
        public string ResponseBody { get; set; }
        public BlockIdResponse ResponseData { get; set; }
    }

    public class BlockIdApiClient
    {
        private static readonly int TimeoutMs = 10000; // 10 giây theo tài liệu

        public static BlockIdApiResult SendEvent(string url, string apiKey, string jsonPayload)
        {
            var result = new BlockIdApiResult();
            HttpWebRequest request = null;

            try
            {
                request = (HttpWebRequest)WebRequest.Create(url);
                request.Method = "POST";
                request.ContentType = "application/cloudevents+json; charset=utf-8";
                request.Headers.Add("X-Api-Key", apiKey ?? "");
                request.Timeout = TimeoutMs;
                request.ReadWriteTimeout = TimeoutMs;
                request.KeepAlive = true;

                byte[] postBytes = Encoding.UTF8.GetBytes(jsonPayload);
                request.ContentLength = postBytes.Length;

                using (Stream stream = request.GetRequestStream())
                {
                    stream.Write(postBytes, 0, postBytes.Length);
                }

                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    result.StatusCode = (int)response.StatusCode;
                    using (StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8))
                    {
                        result.ResponseBody = reader.ReadToEnd();
                    }

                    if (!string.IsNullOrEmpty(result.ResponseBody))
                    {
                        try
                        {
                            result.ResponseData = JsonConvert.DeserializeObject<BlockIdResponse>(result.ResponseBody);
                        }
                        catch { }
                    }

                    // HTTP 202: Nhận thành công
                    // HTTP 200: Đã nhận trước đó ("status": "duplicate")
                    if (result.StatusCode == 202)
                    {
                        result.IsSuccess = true;
                    }
                    else if (result.StatusCode == 200)
                    {
                        result.IsSuccess = true;
                        if (result.ResponseData?.Status == "duplicate")
                        {
                            result.IsDuplicate = true;
                        }
                    }
                }
            }
            catch (WebException webEx)
            {
                if (webEx.Response is HttpWebResponse errResponse)
                {
                    result.StatusCode = (int)errResponse.StatusCode;
                    try
                    {
                        using (StreamReader reader = new StreamReader(errResponse.GetResponseStream(), Encoding.UTF8))
                        {
                            result.ResponseBody = reader.ReadToEnd();
                        }
                        if (!string.IsNullOrEmpty(result.ResponseBody))
                        {
                            result.ResponseData = JsonConvert.DeserializeObject<BlockIdResponse>(result.ResponseBody);
                        }
                    }
                    catch { }

                    // Phân loại mã lỗi theo tài liệu BlockId:
                    // 400 - 404: Lỗi dữ liệu sai, API key sai, subject không tồn tại -> Không tự động retry
                    // 429, 500, 502, 503, 504: Lỗi quá tải, server lỗi tạm thời -> Cho phép retry
                    if (result.StatusCode >= 400 && result.StatusCode <= 404)
                    {
                        result.IsFatalError = true;
                        result.IsTemporaryError = false;
                        result.ErrorMessage = $"BlockId Client Error ({result.StatusCode}): {result.ResponseBody}";
                    }
                    else
                    {
                        result.IsTemporaryError = true;
                        result.ErrorMessage = $"BlockId Server Error ({result.StatusCode}): {result.ResponseBody}";
                    }
                }
                else
                {
                    // Lỗi timeout, mất mạng, không phân giải được DNS -> Lỗi tạm thời (retry)
                    result.IsTemporaryError = true;
                    result.StatusCode = 0;
                    result.ErrorMessage = $"Network/Timeout Error: {webEx.Message}";
                }
            }
            catch (Exception ex)
            {
                result.IsTemporaryError = true;
                result.StatusCode = -1;
                result.ErrorMessage = $"Unexpected Error: {ex.Message}";
            }

            return result;
        }
    }
}
