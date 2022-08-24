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
    margin-bottom: 15px;
  }

  .add-button {
    width: 85px;
    margin-bottom: 16px;
  }

  .block {
    display: flex;
    flex-direction: column;
    gap: 8px;
  }

  .buttons {
    width: calc(100% - 32px);
    position: absolute;
    bottom: 16px;
  }
`;

const IpSecurityLoader = () => {
  return (
    <StyledLoader>
      <Loaders.Rectangle className="header" height="37px" />
      <Loaders.Rectangle className="description" height="80px" />

      <div className="checkboxs">
        <Loaders.Rectangle height="20px" />
        <Loaders.Rectangle height="20px" />
      </div>

      <Loaders.Rectangle className="add-button" height="20px" />

      <div className="block">
        <Loaders.Rectangle height="22px" width="72px" />
        <Loaders.Rectangle height="64px" />
      </div>

      <Loaders.Rectangle className="buttons" height="40px" />
    </StyledLoader>
  );
};

export default IpSecurityLoader;
