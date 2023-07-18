import styled, { css } from "styled-components";
import CrossReactSvg from "PUBLIC_DIR/images/cross.react.svg";
import commonIconsStyles from "@docspace/components/utils/common-icons-style";

const StyledPublicRoomBar = styled.div`
  display: flex;
  background-color: #f8f9f9;
  color: #333;
  font-size: 12px;
  padding: 12px 16px;
  border-radius: 6px;
  margin-bottom: 10px;

  .text-container {
    width: 100%;
    display: flex;
    flex-direction: column;
  }

  .header-body {
    display: flex;
    height: fit-content;
    width: 100%;
    gap: 8px;
    font-weight: 600;
  }

  .body-container {
    color: #555f65;
    font-weight: 400;
  }

  .close-icon {
    margin: -5px -17px 0 0;

    svg {
      weight: 8px;
      height: 8px;
    }
  }
`;

const StyledCrossIcon = styled(CrossReactSvg)`
  ${commonIconsStyles}

  g {
    path {
      fill: #657077;
    }
  }

  path {
    fill: #999976;
  }
`;

const LinksBlock = styled.div`
  display: flex;
  justify-content: space-between;
  padding: 8px 0px 12px 0px;

  .link-to-viewing-icon {
    svg {
      weight: 16px;
      height: 16px;
    }
  }
`;

const StyledLinkRow = styled.div`
  display: flex;
  align-items: center;
  justify-content: space-between;
  gap: 8px;
  padding: 8px 0px;

  .external-row-link {
    white-space: nowrap;
    overflow: hidden;
    text-overflow: ellipsis;
  }

  .external-row-icons {
    margin-left: auto;
    display: flex;
    gap: 16px;
  }

  .avatar_role-wrapper {
    ${({ isExpired }) =>
      isExpired &&
      css`
        svg {
          path {
            fill: #f98e86;
          }
        }
      `}
  }
`;

export { StyledPublicRoomBar, StyledCrossIcon, LinksBlock, StyledLinkRow };
