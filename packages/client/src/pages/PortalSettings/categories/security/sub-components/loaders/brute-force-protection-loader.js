import styled from "styled-components";
import Loaders from "@docspace/common/components/Loaders";

const StyledLoader = styled.div`
  padding-right: 8px;

  .header {
    margin-bottom: 12px;
  }

  .content {
    display: flex;
    flex-direction: column;
    gap: 2px;
  }

  .buttons {
    width: calc(100% - 32px);
    position: absolute;
    bottom: 16px;
  }
`;

const BruteForceProtectionLoader = () => {
  return (
    <StyledLoader>
      <Loaders.Rectangle className="header" height="80px" />

      <div className="content">
        <div>
          <Loaders.Rectangle width="140px" height="20px" />
          <Loaders.Rectangle height="32px" />
        </div>

        <div>
          <Loaders.Rectangle width="117px" height="20px" />
          <Loaders.Rectangle height="32px" />
        </div>

        <div>
          <Loaders.Rectangle width="117px" height="20px" />
          <Loaders.Rectangle height="32px" />
        </div>
      </div>

      <Loaders.Rectangle className="buttons" height="40px" />
    </StyledLoader>
  );
};

export default BruteForceProtectionLoader;
