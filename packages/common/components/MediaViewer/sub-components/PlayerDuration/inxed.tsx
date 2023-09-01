import { mobile } from "@docspace/components/utils/device";
import React, { forwardRef, useState } from "react";
import styled, { css } from "styled-components";
import { formatTime } from "../../helpers";

const PlayerDurationWrapper = styled.div`
  width: 102px;
  color: #fff;
  user-select: none;
  font-size: 12px;
  font-weight: 700;

  ${props =>
    props.theme.interfaceDirection === "rtl"
      ? css`
          margin-right: 10px;
        `
      : css`
          margin-left: 10px;
        `}

  @media ${mobile} {
    ${props =>
      props.theme.interfaceDirection === "rtl"
        ? css`
            margin-right: 0;
          `
        : css`
            margin-left: 0;
          `}
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
