import styled, { css } from "styled-components";
import { Base } from "@docspace/components/themes";

const StyledUserTypeHeader = styled.div`
  display: flex;
  align-items: center;
  justify-content: space-between;
  padding: 24px 0 16px;

  .title {
    font-family: "Open Sans";
    font-style: normal;
    font-weight: 600;
    font-size: 13px;
    line-height: 20px;
    color: #a3a9ae;
  }

  .icon {
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
    font-family: "Open Sans";
    font-style: normal;
    font-weight: 600;
    font-size: 14px;

    white-space: nowrap;
    overflow: hidden;
    text-overflow: ellipsis;

    .secondary-info {
      color: #a3a9ae;
    }
  }

  .role {
    margin-left: auto;

    font-family: "Open Sans";
    font-style: normal;
    font-weight: 600;
    font-size: 13px;
    line-height: 20px;
    white-space: nowrap;

    color: #555f65;
  }
`;

export { StyledUserTypeHeader, StyledUserList, StyledUser };
