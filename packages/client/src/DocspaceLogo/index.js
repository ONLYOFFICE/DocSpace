import React from "react";
import styled from "styled-components";
import { ReactSVG } from "react-svg";
import { hugeMobile } from "@docspace/components/utils/device";
import { inject, observer } from "mobx-react";
import { getLogoFromPath } from "@docspace/common/utils";

const StyledWrapper = styled.div`
  .logo-wrapper {
    width: 386px;
    height: 44px;
  }

  @media ${hugeMobile} {
    display: none;
  }
`;

const DocspaceLogo = (props) => {
  const { className, whiteLabelLogoUrls, theme } = props;

  const logo = getLogoFromPath(
    !theme.isBase
      ? whiteLabelLogoUrls[1]?.path?.dark
      : whiteLabelLogoUrls[1]?.path?.light
  );

  return (
    <StyledWrapper>
      {logo ? (
        <ReactSVG src={logo} className={`logo-wrapper ${className}`} />
      ) : (
        <></>
      )}
    </StyledWrapper>
  );
};

export default inject(({ auth }) => {
  const { settingsStore } = auth;
  const { whiteLabelLogoUrls, theme } = settingsStore;

  return {
    whiteLabelLogoUrls,
    theme,
  };
})(observer(DocspaceLogo));
