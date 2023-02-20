import styled from "styled-components";
import Text from "@docspace/components/text";

const StyledText = styled(Text)`
  display: inline-block;
  margin-right: 12px;
`;

const StyledEmptyRoomTitle = styled.div`
  width: 13px;
  height: 2px;
  margin-left: -12px;
  background-color: #a3a9ae;
`;

const StyledAuthorCell = styled.div`
  display: flex;
  width: 100%;
  overflow: hidden;

  .author-avatar-cell {
    width: 16px;
    min-width: 16px;
    height: 16px;
    margin-right: 8px;
  }
`;

export { StyledText, StyledAuthorCell, StyledEmptyRoomTitle };
