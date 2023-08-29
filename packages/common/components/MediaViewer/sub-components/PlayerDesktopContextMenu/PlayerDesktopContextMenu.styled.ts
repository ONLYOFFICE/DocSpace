import styled from "styled-components";

export const PlayerDesktopContextMenuWrapper = styled.div`
  position: relative;

  display: flex;
  justify-content: center;
  align-items: center;
  min-width: 48px;
  height: 48px;
  &:hover {
    cursor: pointer;
  }

  & > svg {
    padding-left: 19px;
    padding-bottom: 3px;
    width: 18px;
    height: 20px;

    path {
      fill: #fff;
    }

    rect {
      stroke: #fff;
    }
  }
`;

export const DownloadIconWrapper = styled.div`
  display: flex;
  justify-content: center;
  align-items: center;
  min-width: 48px;
  height: 48px;
  &:hover {
    cursor: pointer;
  }

  svg {
    path {
      fill: #fff;
    }
  }
`;
