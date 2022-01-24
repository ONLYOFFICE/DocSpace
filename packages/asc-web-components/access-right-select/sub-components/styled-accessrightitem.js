import styled from "styled-components";
import Base from "../../themes/base";

const StyledAccessRightItem = styled.div`
  width: 424px;

  display: flex;
  align-items: flex-start;
  justify-content: flex-start;
  align-content: center;

  padding: 7px 0px;

  line-height: 16px;
  font-style: normal;
`;

const StyledAccessRightDescriptionItem = styled.div`
  margin: 1px 0px;

  font-size: 13px;
  font-style: normal;
  font-weight: 400;
  line-height: 16px;
  color: #a3a9ae;
`;

StyledAccessRightItem.defaultProps = { theme: Base };
export { StyledAccessRightItem, StyledAccessRightDescriptionItem };
