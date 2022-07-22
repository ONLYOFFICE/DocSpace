import React from "react";
import styled from "styled-components";
import Loaders from "@docspace/common/components/Loaders";
import { tablet } from "@docspace/components/utils/device";

const StyledLoader = styled.div`
  .header {
    width: 296px;
    height: 29px;
    margin-bottom: 14px;

    @media (${tablet}) {
      width: 184px;
      height: 37px;
    }

    @media (max-width: 428px) {
      width: 273px;
      height: 37px;
      margin-bottom: 18px;
    }
  }

  .submenu {
    display: flex;
    gap: 20px;
    margin-bottom: 22px;
  }

  .owner {
    width: 700px;
    display: flex;
    flex-direction: column;
    gap: 20px;
    margin-bottom: 40px;

    @media (max-width: 428px) {
      width: 100%;
    }

    .header {
      height: 40px;
      @media (${tablet}) {
        height: 60px;
      }
    }
  }

  .admins {
    display: flex;
    flex-direction: column;
    gap: 8px;

    .description {
      width: 700px;
      @media (${tablet}) {
        width: 100%;
      }
    }
  }
`;

const AccessLoader = () => {
  return (
    <StyledLoader>
      <Loaders.Rectangle className="header" height="100%" />
      <div className="submenu">
        <Loaders.Rectangle height="28px" width="72px" />
        <Loaders.Rectangle height="28px" width="72px" />
        <Loaders.Rectangle height="28px" width="72px" />
        <Loaders.Rectangle height="28px" width="72px" />
      </div>
      <div className="owner">
        <Loaders.Rectangle className="header" height="100%" />
        <Loaders.Rectangle height="82px" />
      </div>
      <div className="admins">
        <Loaders.Rectangle height="22px" width="77px" />
        <Loaders.Rectangle height="20px" width="56px" />
        <Loaders.Rectangle className="description" height="40px" />
      </div>
    </StyledLoader>
  );
};

export default AccessLoader;
