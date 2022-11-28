import React from "react";
import styled from "styled-components";
import { ReactSVG } from "react-svg";
import { hugeMobile } from "@docspace/components/utils/device";
import { isMobileOnly } from "react-device-detect";
import { inject, observer } from "mobx-react";

const StyledWrapper = styled.div`
  .logo-wrapper {
    width: 100%;
    height: 46px;

    @media ${hugeMobile} {
      display: none;
    }
  }
`;

const DocspaceLogo = (props) => {
  const { className, whiteLabelLogoUrls, userTheme } = props;

  if (isMobileOnly) return <></>;

  const logo =
    userTheme === "Dark"
      ? whiteLabelLogoUrls[1].path.dark
      : whiteLabelLogoUrls[1].path.light;

  return (
    <StyledWrapper>
      <ReactSVG src={logo} className={`logo-wrapper ${className}`} />
    </StyledWrapper>
  );
};

export default inject(({ auth }) => {
  const { settingsStore, userStore } = auth;
  const { whiteLabelLogoUrls } = settingsStore;
  const { userTheme } = userStore;

  return {
    whiteLabelLogoUrls,
    userTheme,
  };
})(observer(DocspaceLogo));
