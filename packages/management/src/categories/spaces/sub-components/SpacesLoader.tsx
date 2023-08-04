import React from "react";
import Loaders from "@docspace/common/components/Loaders";
import { isMobileOnly } from "react-device-detect";
import styled, { css } from "styled-components";

const StyledLoader = styled.div`
  max-width: 700px;
  display: flex;
  flex-direction: column;

  .button {
    margin: 20px 0;
    max-width: 100px;
    ${isMobileOnly &&
    css`
      max-width: 100%;
    `}
  }

  .portals {
    margin-bottom: 24px;
  }

  .domain-header {
    max-width: 130px;
    margin-bottom: 16px;
  }

  .configuration-header {
    max-width: 225px;
    margin-top: 20px;
    margin-bottom: 8px;
  }

  .input {
    max-width: 350px;
  }

  .configuration-input {
    max-width: 350px;
    margin-top: 16px;
  }
`;

export const SpacesLoader = ({ isConfigurationSection }) => {
  return (
    <StyledLoader>
      <Loaders.Rectangle height="22px" className="subheader" />
      {isConfigurationSection ? (
        <>
          <Loaders.Rectangle height="22px" className="configuration-header" />
          <Loaders.Rectangle height="48px" className="section-title" />
          <Loaders.Rectangle height="52px" className="configuration-input" />
          <Loaders.Rectangle height="52px" className="configuration-input" />
        </>
      ) : (
        <>
          <Loaders.Rectangle height="23px" className="button" />
          <Loaders.Rectangle height="160px" className="portals" />
          <Loaders.Rectangle height="22px" className="domain-header" />
          <Loaders.Rectangle height="52px" className="input" />
        </>
      )}

      <Loaders.Rectangle height="22px" className="button" />
    </StyledLoader>
  );
};
