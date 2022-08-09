import React from "react";
import styled, { css } from "styled-components";
import { isMobileOnly } from "react-device-detect";

import { mobile } from "@docspace/components/utils/device";

import MainButtonMobile from "@docspace/components/main-button-mobile";

const StyledMainButtonMobile = styled(MainButtonMobile)`
  position: fixed;

  z-index: 200;

  right: 24px;
  bottom: 24px;

  @media ${mobile} {
    right: 16px;
    bottom: 16px;
  }

  ${isMobileOnly &&
  css`
    right: 16px;
    bottom: 16px;
  `}
`;

const MobileView = ({
  actionOptions,
  buttonOptions,
  sectionWidth,
  labelProps,
}) => {
  const [isOpenButton, setIsOpenButton] = React.useState(false);

  const openButtonToggler = () => {
    setIsOpenButton((prevState) => !prevState);
  };

  return (
    <StyledMainButtonMobile
      sectionWidth={sectionWidth}
      actionOptions={actionOptions}
      buttonOptions={buttonOptions}
      isOpenButton={isOpenButton}
      onUploadClick={openButtonToggler}
      onClose={openButtonToggler}
      title={labelProps}
      percent={0}
      withButton={true}
    />
  );
};

export default React.memo(MobileView);
