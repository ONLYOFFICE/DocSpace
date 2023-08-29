import styled from "styled-components";

import { Base } from "@docspace/components/themes";

const StyledUserTypeHeader = styled.div`
  display: flex;
  align-items: center;
  justify-content: space-between;
  padding-top: ${(props) => (props.isExpect ? "20px" : "8px")};
  padding-bottom: 12px;

  .title {
    font-weight: 600;
    font-size: 14px;
    line-height: 20px;
    color: ${(props) => props.theme.infoPanel.members.subtitleColor};
  }

  .icon {
    cursor: pointer;

    path,
    rect {
      fill: ${(props) => props.theme.infoPanel.members.iconColor};
    }

    &:hover {
      path,
      rect {
        fill: ${(props) => props.theme.infoPanel.members.iconHoverColor};
      }
    }
  }
`;

const StyledUserList = styled.div`
  display: flex;
  flex-direction: column;
`;

const StyledUser = styled.div`
  display: flex;
  align-items: center;
  gap: 8px;
  padding: 8px 0;

  .avatar {
    min-width: 32px;
    min-height: 32px;
  }

  .name {
    font-weight: 600;
    font-size: 14px;
    line-height: 16px;
    white-space: nowrap;
    overflow: hidden;
    text-overflow: ellipsis;
    ${(props) =>
      props.isExpect && `color: ${props.theme.infoPanel.members.isExpectName}`};
  }

  .me-label {
    font-weight: 600;
    font-size: 14px;
    line-height: 16px;
    color: ${(props) => props.theme.infoPanel.members.meLabelColor};
    margin-left: -8px;
  }

  .role-wrapper {
    padding-left: 8px;
    margin-left: auto;
    font-weight: 600;
    font-size: 13px;
    line-height: 20px;
    white-space: nowrap;

    .disabled-role-combobox {
      color: ${(props) =>
        props.theme.infoPanel.members.disabledRoleSelectorColor};
    }
  }

  .role-view_remove-icon {
    cursor: pointer;
    svg {
      path {
        fill: ${(props) => props.theme.iconButton.color};
      }
    }

    :hover {
      svg {
        path {
          fill: ${(props) => props.theme.iconButton.hoverColor};
        }
      }
    }
  }
`;

StyledUserTypeHeader.defaultProps = { theme: Base };
export { StyledUserTypeHeader, StyledUserList, StyledUser };
