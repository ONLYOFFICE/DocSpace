// (c) Copyright Ascensio System SIA 2010-2022
//
// This program is a free software product.
// You can redistribute it and/or modify it under the terms
// of the GNU Affero General Public License (AGPL) version 3 as published by the Free Software
// Foundation. In accordance with Section 7(a) of the GNU AGPL its Section 15 shall be amended
// to the effect that Ascensio System SIA expressly excludes the warranty of non-infringement of
// any third-party rights.
//
// This program is distributed WITHOUT ANY WARRANTY, without even the implied warranty
// of MERCHANTABILITY or FITNESS FOR A PARTICULAR  PURPOSE. For details, see
// the GNU AGPL at: http://www.gnu.org/licenses/agpl-3.0.html
//
// You can contact Ascensio System SIA at Lubanas st. 125a-25, Riga, Latvia, EU, LV-1021.
//
// The  interactive user interfaces in modified source and object code versions of the Program must
// display Appropriate Legal Notices, as required under Section 5 of the GNU AGPL version 3.
//
// Pursuant to Section 7(b) of the License you must retain the original Product logo when
// distributing the program. Pursuant to Section 7(e) we decline to grant you any rights under
// trademark law for use of our trademarks.
//
// All the Product's GUI elements, including illustrations and icon sets, as well as technical writing
// content are licensed under the terms of the Creative Commons Attribution-ShareAlike 4.0
// International. See the License terms at http://creativecommons.org/licenses/by-sa/4.0/legalcode

namespace ASC.Common.Radicale;

[Singletone]
public class RadicaleClient
{
    private readonly ILogger<RadicaleClient> _logger;

    public RadicaleClient(ILogger<RadicaleClient> logger)
    {
        _logger = logger;
    }

    public async Task<DavResponse> CreateAsync(DavRequest davRequest)
    {
        davRequest.Method = "MKCOL";
        var response = await RequestAsync(davRequest);
        return GetDavResponse(response);
    }

    public async Task<DavResponse> GetAsync(DavRequest davRequest)
    {
        davRequest.Method = "GET";
        var response = await RequestAsync(davRequest);
        var davResponse = new DavResponse()
        {
            StatusCode = (int)response.StatusCode
        };

        if (response.StatusCode == HttpStatusCode.OK)
        {
            davResponse.Completed = true;
            davResponse.Data = await response.Content.ReadAsStringAsync();
        }
        else
        {
            davResponse.Completed = false;
            davResponse.Error = response.ReasonPhrase;
        }

        return davResponse;
    }


    public async Task<DavResponse> UpdateItemAsync(DavRequest davRequest)
    {
        davRequest.Method = "PUT";
        var response = await RequestAsync(davRequest);
        return GetDavResponse(response);
    }

    public async Task<DavResponse> UpdateAsync(DavRequest davRequest)
    {
        davRequest.Method = "PROPPATCH";
        var response = await RequestAsync(davRequest);
        return GetDavResponse(response);
    }

    public async Task RemoveAsync(DavRequest davRequest)
    {
        davRequest.Method = "DELETE";
        await RequestAsync(davRequest);
    }

    private async Task<HttpResponseMessage> RequestAsync(DavRequest davRequest)
    {
        try
        {
            using var hc = new HttpClient();

            hc.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(Encoding.UTF8.GetBytes(davRequest.Authorization)));
            if (!string.IsNullOrEmpty(davRequest.Header))
            {
                hc.DefaultRequestHeaders.Add("X_REWRITER_URL", davRequest.Header);
            }

            var method = new HttpMethod(davRequest.Method);
            var request = new HttpRequestMessage(method, davRequest.Url);

            if (davRequest.Data != null)
            {
                request.Content = new StringContent(davRequest.Data);
            }

            return await hc.SendAsync(request).ConfigureAwait(false);
        }
        catch (AggregateException ex)
        {
            throw new RadicaleException(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.ErrorWithException(ex);
            throw new RadicaleException(ex.Message);
        }
    }


    private DavResponse GetDavResponse(HttpResponseMessage response)
    {
        if (response.IsSuccessStatusCode)
        {
            return new DavResponse()
            {
                Completed = true,
                Data = response.IsSuccessStatusCode ? response.RequestMessage.RequestUri.ToString() : response.ReasonPhrase,
            };

        }
        else
        {
            return new DavResponse()
            {
                Completed = false,
                StatusCode = (int)response.StatusCode,
                Error = response.ReasonPhrase
            };
        }

    }
}
