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

/// <summary>
/// </summary>
[Transient]
[ProtoContract(IgnoreUnknownSubTypes = true)]
public class DistributedTaskProgress : DistributedTask
{
    [ProtoMember(1)]
    private double _percentage;

    /// <summary>Progress percentage</summary>
    /// <type>System.Double, System</type>
    public double Percentage
    {
        get => Math.Min(100.0, Math.Max(0, _percentage));
        set => _percentage = value;
    }

    /// <summary>Specifies if the process is completed or not</summary>
    /// <type>System.Boolean, System</type>
    [ProtoMember(2)]
    public bool IsCompleted { get; set; }

    /// <summary>Number of steps</summary>
    /// <type>System.Int32, System</type>
    [ProtoMember(3)]
    protected int StepCount { get; set; }

    public virtual async Task RunJob(DistributedTask _, CancellationToken cancellationToken)
    {
        Percentage = 0;
        Status = DistributedTaskStatus.Running;

        await DoJob();
    }

    protected virtual Task DoJob() { return Task.CompletedTask; }

    protected void StepDone()
    {
        if (StepCount > 0)
        {
            Percentage += 100.0 / StepCount;
        }

        PublishChanges();
    }
}