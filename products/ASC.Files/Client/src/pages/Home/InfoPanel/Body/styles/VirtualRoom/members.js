import styled, { css } from "styled-components";
import { Base } from "@appserver/components/themes";

const StyledUserTypeHeader = styled.div`
  display: flex;
  align-items: center;
  justify-content: space-between;
  padding: 24px 0 16px;

  .title {
    font-weight: 600;
    font-size: 13px;
    line-height: 20px;
    color: #a3a9ae;
  }

  .icon {
    path,
    rect {
      fill: ${(props) => props.theme.infoPanel.members.iconColor};
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
    opacity: ${(props) => (props.isExpect ? 0.5 : 1)};
    min-width: 32px;
    min-height: 32px;
  }

  .name {
    opacity: ${(props) => (props.isExpect ? 0.5 : 1)};
    font-weight: 600;
    font-size: 14px;

    white-space: nowrap;
    overflow: hidden;
    text-overflow: ellipsis;
  }

  .me-label {
    font-weight: 600;
    font-size: 14px;
    color: #a3a9ae;
    margin-left: -8px;
  }

  .role-wrapper {
    padding-left: 8px;
    margin-left: auto;

    font-weight: 600;
    font-size: 13px;
    line-height: 20px;
    white-space: nowrap;

    .role {
      a {
        padding-right: ${(props) => (props.canEditRole ? "16px" : "0")};
        margin-right: ${(props) => (props.canEditRole ? "-6px" : "0")};
        span {
          color: ${(props) => (props.canEditRole ? "#555f65" : "#A3A9AE")};
        }
        &:hover {
          text-decoration: none;
        }

        svg {
          ${(props) => !props.canEditRole && "display: none"};
          path {
            fill: #a3a9ae;
          }
        }
      }
    }
  }
`;

StyledUserTypeHeader.defaultProps = { theme: Base };

export { StyledUserTypeHeader, StyledUserList, StyledUser };
