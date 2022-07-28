import React from "react";
import styled from "styled-components";
import Loaders from "@docspace/common/components/Loaders";

const StyledLoader = styled.div`
  padding-right: 8px;

  .header {
    width: 273px;
    margin-bottom: 16px;
  }

  .description {
    margin-bottom: 8px;
  }

  .link {
    margin-bottom: 20px;
  }

  .subheader {
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
    width: 50px;
  }

  .buttons {
    width: calc(100% - 32px);
    position: absolute;
    bottom: 16px;
  }
`;

const PasswordLoader = () => {
  return (
    <StyledLoader>
      <Loaders.Rectangle className="header" height="37px" />
      <Loaders.Rectangle className="description" height="80px" />
      <div className="link">
        <Loaders.Rectangle height="20px" width="57px" />
      </div>
      <Loaders.Rectangle className="subheader" height="16px" width="171px" />
      <div className="slider">
        <Loaders.Rectangle height="24px" width="160px" />
        <Loaders.Rectangle height="20px" width="75px" />
      </div>
      <div className="checkboxs">
        <Loaders.Rectangle height="20px" />
        <Loaders.Rectangle height="20px" />
        <Loaders.Rectangle height="20px" />
      </div>

      <Loaders.Rectangle className="buttons" height="40px" />
    </StyledLoader>
  );
};

export default PasswordLoader;
