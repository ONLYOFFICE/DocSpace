import React from "react";
import styled from "styled-components";
import { inject, observer } from "mobx-react";

import { isMobile } from "react-device-detect";
import { tablet } from "@appserver/components/utils/device";

import Headline from "@appserver/common/components/Headline";

import IconButton from "@appserver/components/icon-button";

const StyledContainer = styled.div`
  width: 100%;
  height: 53px;
  display: flex;
  align-items: center;
`;

const StyledHeadline = styled(Headline)`
  font-weight: 700;
  font-size: ${isMobile ? "21px !important" : "18px"};
  line-height: ${isMobile ? "28px !important" : "24px"};

  margin-right: 16px;

  @media ${tablet} {
    font-size: 21px;
    line-height: 28px;
  }
`;

const SectionHeaderContent = ({ createRoom }) => {
  return (
    <StyledContainer>
      <StyledHeadline>Virtual Rooms</StyledHeadline>
      <IconButton
        zIndex={402}
        className="create-button"
        directionX="right"
        iconName="images/plus.svg"
        size={15}
        isFill
        onClick={createRoom}
        isDisabled={false}
      />
    </StyledContainer>
  );
};

export default inject(({ roomsStore }) => {
  const { createRoom } = roomsStore;

  return { createRoom };
})(observer(SectionHeaderContent));
