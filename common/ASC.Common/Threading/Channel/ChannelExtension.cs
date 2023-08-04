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

namespace ASC.Common.Threading;
public static class ChannelExtension
{   
    public static IList<ChannelReader<T>> Split<T>(this ChannelReader<T> ch, int n, Func<int, int, T, int> selector = null, CancellationToken cancellationToken = default)
    {
        var outputs = new Channel<T>[n];

        for (var i = 0; i < n; i++)
        {
            outputs[i] = Channel.CreateUnbounded<T>();
        }

        Task.Run(async () =>
        {
            try
            {
                cancellationToken.ThrowIfCancellationRequested();

                var index = 0;

                await foreach (var item in ch.ReadAllAsync(cancellationToken))
                {
                    if (selector == null)
                    {
                        index = (index + 1) % n;
                    }
                    else
                    {
                        index = selector(n, index, item);
                    }

                    await outputs[index].Writer.WriteAsync(item, cancellationToken);
                }
            }
            catch (OperationCanceledException)
            {
                // TODO: catch error via additional channel
                //  var error = Channel.CreateUnbounded<T>();
                //  await error.Writer.WriteAsync(ex);
            }
            finally
            {
                foreach (var ch in outputs)
                {
                    ch.Writer.Complete();
                }
            }
        });

        return outputs.Select(ch => ch.Reader).ToArray();
    }
    
    public static ChannelReader<T> Merge<T>(this IEnumerable<ChannelReader<T>> inputs, CancellationToken cancellationToken = default)
    {
        var output = Channel.CreateUnbounded<T>();

        Task.Run(async () =>
        {
            try
            {
                cancellationToken.ThrowIfCancellationRequested();

                async Task Redirect(ChannelReader<T> input)
                {
                    await foreach (var item in input.ReadAllAsync(cancellationToken))
                    {
                        await output.Writer.WriteAsync(item, cancellationToken);
                    }
                }

                await Task.WhenAll(inputs.Select(i => Redirect(i)).ToArray());
            }
            catch (OperationCanceledException)
            {
                // TODO: catch error via additional channel
                //  var error = Channel.CreateUnbounded<T>();
                //  await error.Writer.WriteAsync(ex);
            }
            finally
            {
                output.Writer.Complete();
            }
        });

        return output;
    }

}
