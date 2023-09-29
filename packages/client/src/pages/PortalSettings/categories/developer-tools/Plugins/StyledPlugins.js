import styled from "styled-components";
import { isMobileOnly } from "react-device-detect";

import { Base } from "@docspace/components/themes";

const StyledContainer = styled.div`
  width: 100%;
  max-width: 700px;

  display: flex;
  flex-direction: column;

  gap: 20px;
`;

const PluginListContainer = styled.div`
  margin-top: 8px;

  width: 100%;

  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(328px, 1fr));

  gap: 20px;
`;

const StyledPluginItem = styled.div`
  width: 100%;

  height: 135px;
  max-height: 135px;

  display: grid;
  grid-template-rows: 1fr;
  grid-template-columns: 64px 1fr;

  gap: 20px;

  border: 1px solid ${(props) => props.theme.plugins.borderColor};
  border-radius: 12px;

  padding: 24px;

  box-sizing: border-box;

  .plugin-logo {
    width: 64px;
    height: 64px;

    border-radius: 4px;
  }

  .plugin-info {
    width: 100%;
    height: auto;

    display: flex;
    flex-direction: column;

    gap: 8px;

    .plugin-description {
      width: 100%;

      display: -webkit-box;
      -webkit-box-orient: vertical;
      -webkit-line-clamp: 2;
      overflow: hidden;
    }
  }

  ${(props) => !props.description && isMobileOnly && `max-height: 112px;`}
`;

StyledPluginItem.defaultProps = { theme: Base };

const StyledPluginHeader = styled.div`
  width: 100%;
  height: 22px;

  display: flex;
  align-items: center;
  justify-content: space-between;

  .plugin-name {
    margin: 0;
    padding: 0;

    font-weight: 700 !important;
    font-size: 16px !important;
    line-height: 22px;
  }

  .plugin-controls {
    height: 100%;

    display: flex;
    gap: 16px;

    align-items: center;

    .plugin-toggle-button {
      position: relative;

      gap: 0;
    }
  }
`;

export {
  StyledContainer,
  PluginListContainer,
  StyledPluginItem,
  StyledPluginHeader,
};
