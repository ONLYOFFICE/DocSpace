import styled from "styled-components";
import TileContent from "./sub-components/TileContent";

export const SimpleTileContent = styled(TileContent)`
  .rowMainContainer {
    height: auto;
    max-width: 100%;
    align-self: flex-end;

    a {
      word-break: break-word;
    }
  }

  .mainIcons {
    align-self: flex-end;
  }

  .badge-ext {
    margin-left: -8px;
    margin-right: 8px;
  }

  .badge {
    margin-right: 8px;
  }
  .new-items {
    position: absolute;
    right: 29px;
    top: 19px;
  }
  .item-about {
    display: none;
  }
  .badges {
    display: flex;
    align-items: center;
  }

  .share-icon {
    margin-top: -4px;
    padding-right: 8px;
  }

  @media (max-width: 1024px) {
    display: inline-flex;
    height: auto;

    & > div {
      margin-top: 0;
    }
  }
`;
