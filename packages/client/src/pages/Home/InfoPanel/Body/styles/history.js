import styled from "styled-components";

import { Base } from "@docspace/components/themes";
import { mobile } from "@docspace/components/utils/device";

const StyledHistoryList = styled.div`
  display: flex;
  flex-direction: column;
`;

const StyledHistorySubtitle = styled.div`
  padding: 8px 0 12px;
  font-weight: 600;
  font-size: 13px;
  line-height: 20px;
  color: #a3a9ae;
`;

const StyledUserNameLink = styled.span`
  white-space: normal;
  margin: 1px 0;
  .username {
    font-size: 13px;
    font-weight: 600;
    display: inline-block;
    text-underline-offset: 2px;
  }
`;

const StyledHistoryBlock = styled.div`
  width: 100%;
  display: flex;
  gap: 8px;
  padding: 16px 0;
  border-bottom: ${(props) => `solid 1px ${props.theme.infoPanel.borderColor}`};

  .avatar {
    min-width: 32px;
  }

  .info {
    width: calc(100% - 40px);
    max-width: calc(100% - 40px);
    display: flex;
    flex-direction: column;
    gap: 2px;

    .title {
      display: flex;
      flex-direction: row;
      gap: 4px;
      .name {
        font-weight: 600;
        font-size: 14px;
        text-overflow: ellipsis;
        white-space: nowrap;
        overflow: hidden;
      }
      .secondary-info {
        white-space: nowrap;
        font-weight: 400;
        font-size: 14px;
        color: #a3a9ae;
        @media ${mobile} {
          display: none;
        }
      }
      .date {
        white-space: nowrap;
        display: inline-block;
        margin-left: auto;
        font-weight: 600;
        font-size: 12px;
        color: #a3a9ae;
      }
    }
  }
`;

const StyledHistoryBlockMessage = styled.div`
  font-weight: 400;
  font-size: 13px;
  line-height: 20px;
`;

const StyledHistoryBlockFilesList = styled.div`
  margin-top: 10px;
  display: flex;
  flex-direction: column;
  padding: 8px 0;
  background: ${(props) => props.theme.infoPanel.history.fileBlockBg};
  border-radius: 3px;

  .show_more-link {
    cursor: pointer;
    margin: 10px 0 3px 20px;
    font-weight: 600;
    font-size: 13px;
    line-height: 15px;
    text-decoration: underline;
    text-decoration-style: dashed;
    text-underline-offset: 2px;
  }
`;

const StyledHistoryBlockFile = styled.div`
  padding: 4px 16px;
  display: flex;
  gap: 8px;
  flex-direction: row;
  align-items: center;
  justify-content: start;

  .icon {
    width: 24px;
    height: 24px;
    svg {
      width: 24px;
      height: 24px;
    }
  }

  .item-title {
    font-weight: 600;
    font-size: 14px;
    display: flex;
    min-width: 0;
    gap: 0;

    .name {
      text-overflow: ellipsis;
      white-space: nowrap;
      overflow: hidden;
    }

    .exst {
      flex-shrink: 0;
      color: #a3a9ae;
    }
  }

  .location-btn {
    margin-left: auto;
    min-width: 16px;
  }
`;

StyledHistoryBlock.defaultProps = { theme: Base };

export {
  StyledHistoryList,
  StyledHistorySubtitle,
  StyledUserNameLink,
  StyledHistoryBlock,
  StyledHistoryBlockMessage,
  StyledHistoryBlockFilesList,
  StyledHistoryBlockFile,
};
