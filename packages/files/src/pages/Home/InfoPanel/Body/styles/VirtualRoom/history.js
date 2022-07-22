import styled, { css } from "styled-components";
import { Base } from "@docspace/components/themes";
import { mobile, smallTablet, tablet } from "@docspace/components/utils/device";

const StyledHistoryList = styled.div`
  display: flex;
  flex-direction: column;
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
        color: #333333;
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

    .block-content {
      .appointing {
        .appointing-user {
          font-weight: 600;
          font-size: 13px;
        }
      }

      .users {
        .user-list {
          .space {
            width: 4px;
            display: inline-block;
          }
        }
      }

      .files-list {
        margin-top: 10px;
        display: flex;
        flex-direction: column;
        padding: 8px 0;
        background: #f8f9f9;
        border-radius: 3px;
        .file {
          padding: 4px 16px;
          display: flex;
          gap: 8px;
          flex-direction: row;
          align-items: center;
          justify-content: start;

          .icon {
            min-width: 24px;
          }

          .file-title {
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
        }
      }
    }
  }
`;

const StyledFileAction = styled.div``;

StyledHistoryBlock.defaultProps = { theme: Base };

export { StyledHistoryList, StyledUserNameLink, StyledHistoryBlock };
