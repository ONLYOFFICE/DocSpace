import React from "react";
import styled from "styled-components";
import Loaders from "@docspace/common/components/Loaders";

const StyledLoader = styled.div`
  .header {
    width: 273px;
    margin-bottom: 18px;
  }

  .submenu {
    display: flex;
    gap: 20px;
    margin-bottom: 30px;
  }

  .category {
    display: flex;
    flex-direction: column;
    gap: 8px;
    margin-bottom: 30px;
  }
`;

const MobileSecurityLoader = () => {
  return (
    <StyledLoader>
      <Loaders.Rectangle className="header" height="37px" />
      <div className="submenu">
        <Loaders.Rectangle height="28px" width="72px" />
        <Loaders.Rectangle height="28px" width="72px" />
        <Loaders.Rectangle height="28px" width="72px" />
        <Loaders.Rectangle height="28px" width="72px" />
      </div>

      <div className="category">
        <Loaders.Rectangle height="22px" width="236px" />
        <Loaders.Rectangle height="60px" />
      </div>

      <div className="category">
        <Loaders.Rectangle height="22px" width="227px" />
        <Loaders.Rectangle height="120px" />
      </div>

      <div className="category">
        <Loaders.Rectangle height="22px" />
        <Loaders.Rectangle height="40px" />
      </div>

      <div className="category">
        <Loaders.Rectangle height="22px" width="101px" />
        <Loaders.Rectangle height="40px" />
      </div>
    </StyledLoader>
  );
};

export default MobileSecurityLoader;
