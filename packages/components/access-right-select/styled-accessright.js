import styled from "styled-components";
import Base from "../themes/base";

const StyledWrapper = styled.div`
  display: flex;
  align-items: center;

  .combo-button {
    padding-left: 16px;
    padding-right: 8px;
  }
`;

StyledWrapper.defaultProps = { theme: Base };

const StyledItem = styled.div`
  width: auto;

  display: flex;
  align-items: flex-start;
  justify-content: flex-start;
  align-content: center;

  padding: 7px 0px;

  line-height: 16px;
  font-style: normal;
`;

StyledItem.defaultProps = { theme: Base };

const StyledItemDescription = styled.div`
  margin: 1px 0px;

  font-size: 13px;
  font-style: normal;
  font-weight: 400;
  line-height: 16px;
  color: #a3a9ae;
`;

const StyledItemIcon = styled.img`
  margin-right: 8px;
`;

const StyledItemContent = styled.div`
  width: 100%;
  white-space: normal;
`;

const StyledItemTitle = styled.div`
  display: flex;
  align-items: center;
  gap: 8px;
`;

export {
  StyledItemTitle,
  StyledItemContent,
  StyledItemIcon,
  StyledItemDescription,
  StyledItem,
  StyledWrapper,
};
