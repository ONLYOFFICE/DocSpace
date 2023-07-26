import React, { CSSProperties } from "react";
import styled, { css } from "styled-components";

import Text from "@docspace/components/text";
import IconButton from "@docspace/components/icon-button";

import { ControlBtn } from "../../StyledComponents";

import ViewerMediaCloseSvgUrl from "PUBLIC_DIR/images/viewer.media.close.svg?url";

type DesktopDetailsProps = {
  title: string;
  onMaskClick: VoidFunction;
  className?: string;
};

const DesktopDetailsContainer = styled.div`
  padding-top: 21px;
  height: 64px;
  width: 100%;
  background: linear-gradient(
    0deg,
    rgba(0, 0, 0, 0) 0%,
    rgba(0, 0, 0, 0.8) 100%
  );

  position: fixed;
  top: 0;
  left: 0;
  z-index: 307;

  .title {
    text-align: center;
    white-space: nowrap;
    overflow: hidden;
    font-size: 20px;
    font-weight: 600;
    text-overflow: ellipsis;
    width: calc(100% - 50px);
    padding-left: 16px;
    box-sizing: border-box;
    color: ${(props) => props.theme.mediaViewer.titleColor};
  }
`;

function DesktopDetails({
  onMaskClick,
  title,
  className,
}: DesktopDetailsProps) {
  return (
    <DesktopDetailsContainer className={className}>
      <Text isBold fontSize="14px" className="title">
        {title}
      </Text>
      <ControlBtn onClick={onMaskClick} className="mediaPlayerClose">
        <IconButton
          color={"#fff"}
          iconName={ViewerMediaCloseSvgUrl}
          size={28}
          isClickable
        />
      </ControlBtn>
    </DesktopDetailsContainer>
  );
}

export default DesktopDetails;
