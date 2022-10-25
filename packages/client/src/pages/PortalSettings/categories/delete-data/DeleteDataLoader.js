import React from "react";
import styled from "styled-components";
import Loaders from "@docspace/common/components/Loaders";
import { hugeMobile } from "@docspace/components/utils/device";

const StyledLoader = styled.div`
  display: flex;
  flex-direction: column;

  .header {
    width: 155px;
    height: 22px;
    margin-bottom: 8px;

    @media (${hugeMobile}) {
      width: 171px;
    }
  }

  .subheader {
    width: 297px;
    height: 16px;
    margin-bottom: 16px;

    @media (${hugeMobile}) {
      width: 100%;
    }
  }

  .body {
    width: 100%;
    max-width: 700px;
  }

  .submenu {
    display: flex;
    gap: 20px;
    margin-bottom: 22px;
  }
`;

const DeleteDataLoader = () => {
  return (
    <StyledLoader>
      <div className="submenu">
        <Loaders.Rectangle height="28px" width="72px" />
        <Loaders.Rectangle height="28px" width="72px" />
      </div>
      <Loaders.Rectangle className="header" />
      <Loaders.Rectangle className="subheader" />
      <Loaders.Rectangle className="body" height="100px" />
    </StyledLoader>
  );
};

export default DeleteDataLoader;
