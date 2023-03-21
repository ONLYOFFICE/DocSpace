import { mobile } from "@docspace/components/utils/device";
import React, { forwardRef, useState } from "react";
import styled from "styled-components";
import { formatTime } from "../../helpers";

const PlayerDurationWrapper = styled.div`
  width: 102px;
  color: #fff;
  user-select: none;
  font-size: 12px;
  font-weight: 700;

  margin-left: 10px;

  @media ${mobile} {
    margin-left: 0;
  }
`;

type PlayerDurationProps = {
  currentTime: number;
  duration: number;
};

function PlayerDuration({ currentTime, duration }: PlayerDurationProps) {
  return (
    <PlayerDurationWrapper>
      <time>{formatTime(currentTime)}</time> /{" "}
      <time>{formatTime(duration)}</time>
    </PlayerDurationWrapper>
  );
}

export default PlayerDuration;
