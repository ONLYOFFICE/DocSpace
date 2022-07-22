import React from "react";
import styled from "styled-components";
import Loaders from "@docspace/common/components/Loaders";
import { tablet } from "@docspace/components/utils/device";

const StyledLoader = styled.div`
  hr {
    margin: 24px 0;
    border: none;
    border-top: 1px solid #eceef1;
  }

  .submenu {
    width: 296px;
    height: 29px;
    margin-bottom: 14px;

    @media (${tablet}) {
      width: 184px;
      height: 37px;
    }
  }

  .header {
    display: flex;
    margin-bottom: 22px;

    .header-item {
      width: 72px;
      margin-right: 20px;
    }
  }

  .description {
    width: 591px;
    margin-bottom: 20px;

    @media (${tablet}) {
      width: 100%;
    }
  }

  .buttons {
    width: 192px;
    height: 32px;

    @media (${tablet}) {
      height: 40px;
    }
  }

  .password-settings {
    .header {
      width: 132px;
      margin-bottom: 16px;
    }

    .subheader {
      width: 171px;
      margin-bottom: 16px;
    }

    .slider {
      display: flex;
      gap: 16px;
      align-items: center;
      margin-bottom: 16px;
    }

    .checkboxs {
      display: flex;
      flex-direction: column;
      gap: 8px;
      margin-bottom: 24px;
    }
  }

  .tfa-settings {
    .header {
      width: 227px;
      margin-bottom: 16px;
    }

    .radio-buttons {
      display: flex;
      flex-direction: column;
      gap: 8px;
      margin-bottom: 24px;
    }
  }

  .domain-settings {
    .header {
      width: 132px;
      margin-bottom: 16px;
    }

    .radio-buttons {
      display: flex;
      flex-direction: column;
      gap: 8px;
      margin-bottom: 11px;
    }

    .inputs {
      display: flex;
      flex-direction: column;
      gap: 8px;
      margin-bottom: 16px;

      .input {
        display: flex;
        gap: 8px;
        align-items: center;
      }
    }

    .button {
      width: 85px;
    }
  }
`;

const SecurityLoader = () => {
  return (
    <StyledLoader>
      <Loaders.Rectangle className="submenu" height="100%" />
      <div className="header">
        <Loaders.Rectangle className="header-item" height="28px" />
        <Loaders.Rectangle className="header-item" height="28px" />
        <Loaders.Rectangle className="header-item" height="28px" />
        <Loaders.Rectangle className="header-item" height="28px" />
      </div>
      <Loaders.Rectangle className="description" height="20px" />

      <div className="password-settings">
        <Loaders.Rectangle className="header" height="22px" />
        <Loaders.Rectangle className="subheader" height="16px" />
        <div className="slider">
          <Loaders.Rectangle height="24px" width="160px" />
          <Loaders.Rectangle height="20px" width="75px" />
        </div>
        <div className="checkboxs">
          <Loaders.Rectangle height="20px" width="133px" />
          <Loaders.Rectangle height="20px" width="83px" />
          <Loaders.Rectangle height="20px" width="159px" />
        </div>
        <Loaders.Rectangle className="buttons" height="100%" />
      </div>
      <hr />
      <div className="tfa-settings">
        <Loaders.Rectangle className="header" height="22px" />
        <div className="radio-buttons">
          <Loaders.Rectangle height="20px" width="69px" />
          <Loaders.Rectangle height="20px" width="69px" />
          <Loaders.Rectangle height="20px" width="152px" />
        </div>
        <Loaders.Rectangle className="buttons" height="100%" />
      </div>
      <hr />
      <div className="domain-settings">
        <Loaders.Rectangle className="header" height="22px" />
        <div className="radio-buttons">
          <Loaders.Rectangle height="20px" width="77px" />
          <Loaders.Rectangle height="20px" width="103px" />
          <Loaders.Rectangle height="20px" width="127px" />
        </div>
        <div className="inputs">
          <div className="input">
            <Loaders.Rectangle height="32px" width="350px" />
            <Loaders.Rectangle height="16px" width="16px" />
          </div>
          <div className="input">
            <Loaders.Rectangle height="32px" width="350px" />
            <Loaders.Rectangle height="16px" width="16px" />
          </div>
          <div className="input">
            <Loaders.Rectangle height="32px" width="350px" />
            <Loaders.Rectangle height="16px" width="16px" />
          </div>
          <Loaders.Rectangle className="button" height="20px" />
        </div>
      </div>
    </StyledLoader>
  );
};

export default SecurityLoader;
