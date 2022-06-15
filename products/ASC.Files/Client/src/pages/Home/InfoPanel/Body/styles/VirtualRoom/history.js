import styled, { css } from "styled-components";
import { Base } from "@appserver/components/themes";

const StyledHistoryList = styled.div`
  display: flex;
  flex-direction: column;
`;

const StyledHistoryBlock = styled.div`
  width: 100%;
  display: flex;
  gap: 8px;
  padding: 16px 0;
  border-bottom: ${(props) => `solid 1px ${props.theme.infoPanel.borderColor}`};

  .info {
    width: 100%;
    display: flex;
    flex-direction: column;

    .title {
      width: 100%;
      display: flex;
      flex-direction: row;
      gap: 4px;
      .name {
        font-weight: 600;
        font-size: 14px;
        color: #333333;
      }
      .secondary-info {
        font-weight: 400;
        font-size: 14px;
        color: #a3a9ae;
      }
      .date {
        margin-left: auto;
        font-weight: 600;
        font-size: 12px;
        color: #a3a9ae;
      }
    }
  }
`;

const StyledFileAction = styled.div`

`;

StyledHistoryBlock.defaultProps = { theme: Base };

export { StyledHistoryList, StyledHistoryBlock };
