import React from "react";
import styled from "styled-components";
import Loaders from "@appserver/common/components/Loaders";

const StyledLoader = styled.div`
  .header {
    display: none;
  }

  .description {
    width: 100%;
    padding-bottom: 12px;
  }

  .title {
    width: ${(props) => (props.portalRenaming ? "49px" : "63.7px")};
    height: 20px;
    padding-bottom: 4px;
  }

  .title-long {
    display: block;
    width: 68px;
    height: 20px;
    padding-bottom: 4px;
  }

  .combo-box {
    display: block;
    height: 32px;
    width: 100%;
    padding-bottom: 24px;
  }

  .field-container {
    height: 20px;
    width: 100%;
    padding-bottom: 12px;
  }

  .save-cancel-buttons {
    display: block;
    position: absolute;
    bottom: 0;
    left: 0;
    width: calc(100% - 32px);
    padding: 0 0 16px 16px;
  }

  @media (min-width: 600px) {
    .header {
      display: block;
      width: ${(props) =>
        props.lngTZSettings
          ? "283px"
          : props.welcomePage
          ? "201px"
          : props.portalRenaming
          ? "150px"
          : 0};
      height: 22px;
      padding-bottom: 16px;
    }

    .description {
      display: none;
    }

    .title {
      display: block;
      width: ${(props) =>
        props.lngTZSettings
          ? "65px"
          : props.welcomePage
          ? "31px"
          : props.portalRenaming
          ? "113px"
          : 0};
      height: 20px;
    }

    .combo-box {
      display: block;
      width: 350px;
    }

    .field-container {
      display: block;
      height: 20px;
      width: 350px;
    }

    .save-cancel-buttons {
      display: block;
      position: static;
      width: 350px;
      height: 32px;
      padding: 0 0 24px 0;
    }
  }
`;

const loaderCustomization = ({
  lngTZSettings,
  portalRenaming,
  welcomePage,
}) => {
  return (
    <StyledLoader
      lngTZSettings={lngTZSettings}
      portalRenaming={portalRenaming}
      welcomePage={welcomePage}
    >
      <Loaders.Rectangle className="header" />

      {portalRenaming && (
        <Loaders.Rectangle height="80px" className="description" />
      )}

      <Loaders.Rectangle className="title" />
      <Loaders.Rectangle className="combo-box" />
      {lngTZSettings && (
        <>
          <Loaders.Rectangle className="field-container" />
          <Loaders.Rectangle className="title-long" />
          <Loaders.Rectangle className="combo-box" />
        </>
      )}
      <Loaders.Rectangle height="40px" className="save-cancel-buttons" />
    </StyledLoader>
  );
};

export default loaderCustomization;
