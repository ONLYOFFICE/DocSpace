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
    margin-bottom: 12px;
  }

  .checkboxs {
    display: flex;
    flex-direction: column;
    gap: 8px;
    width: 50px;
    margin-bottom: 16px;
  }

  .input {
    display: flex;
    flex-direction: column;
    gap: 4px;
  }

  .buttons {
    width: calc(100% - 32px);
    position: absolute;
    bottom: 16px;
  }
`;

const SessionLifetimeLoader = () => {
  return (
    <StyledLoader>
      <Loaders.Rectangle className="header" height="37px" />
      <Loaders.Rectangle className="description" height="20px" />

      <div className="checkboxs">
        <Loaders.Rectangle height="20px" />
        <Loaders.Rectangle height="20px" />
      </div>

      <div className="input">
        <Loaders.Rectangle height="20px" width="95px" />
        <Loaders.Rectangle height="32px" />
      </div>

      <Loaders.Rectangle className="buttons" height="40px" />
    </StyledLoader>
  );
};

export default SessionLifetimeLoader;
