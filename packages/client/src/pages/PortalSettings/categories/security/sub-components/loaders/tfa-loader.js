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

const TfaLoader = () => {
  return (
    <StyledLoader>
      <Loaders.Rectangle className="header" height="37px" />
      <Loaders.Rectangle className="description" height="40px" />
      <div className="link">
        <Loaders.Rectangle height="20px" width="57px" />
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

export default TfaLoader;
